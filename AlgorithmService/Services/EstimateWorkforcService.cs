using System;
using System.Numerics;
using AlgorithmServiceServer.DTOs.AlgorithmController;
using AlgorithmServiceServer.Services.Interfaces;

using AlgorithmServiceServer.DTOs.AlgorithmController;
using AlgorithmServiceServer.Services.Interfaces;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.AlgorithmController;
using Microsoft.EntityFrameworkCore;
using RcpspAlgorithmLibrary;
using UtilsLibrary;
using RcpEstimator;
using System.Drawing;

namespace AlgorithmServiceServer.Services
{
    public class EstimateWorkforcService : IEstimateWorkforceService
    {

        private readonly JiraDemoContext db;
        private readonly HttpContext http;
        private List<int> TaskDuration;
        private List<List<int>> TaskExper;
        private List<List<int>> TaskAdjacency;
        public EstimateWorkforcService(JiraDemoContext db, IHttpContextAccessor httpAccessor)
        {
            this.db = db;
            http = httpAccessor.HttpContext;
        }

       

        public void PrepareDataFromDB(int projectId)
        {


            var taskFromDB = db.Tasks.Where(t => t.ProjectId == projectId)
                .Include(t => t.TaskPrecedenceTasks).Include(t => t.TasksSkillsRequireds);


            int taskSize = taskFromDB.Count();

            TaskAdjacency = Enumerable.Repeat(Enumerable.Repeat(0, taskSize).ToList(), taskSize).ToList();
            TaskDuration = Enumerable.Repeat(0, taskSize).ToList();

            TaskExper = Enumerable.Repeat(Enumerable.Repeat(0, taskSize).ToList(), taskSize).ToList();

            foreach (var task in taskFromDB)
            {
                var duration = task.Duration;
                var taskId = task.Id;
                var milestoneId = task.MilestoneId;
                var tasksSkillsRequireds = task.TasksSkillsRequireds;
                var taskPrecedences = task.TaskPrecedenceTasks;
            }
        }



        public async Task<EstimatedResultDTO> Execute(int projectId)
        {



            List<int> WorkforceOutputList;

            //var cloudId = new JWTManagerService(http).GetCurrentCloudId();
            var cloudId = "ea48ddc7-ed56-4d60-9b55-02667724849d"; // DEBUG
            var inputTo = new InputToORDTO();



            var projectFromDB = await db.Projects
                .Where(p => p.CloudId == cloudId)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync();

            var skillFromDB = db.Skills.Where(s => s.CloudId == cloudId).ToList();
            var taskFromDB = db.Tasks.Where(t => t.ProjectId == projectId)
                .Include(t => t.TaskPrecedenceTasks).Include(t => t.TasksSkillsRequireds).ToList();


            var inputToEstimator = new InputToEstimatorDTO();
            inputToEstimator.SkillList = skillFromDB;
            inputToEstimator.TaskList = taskFromDB;

            var converter = new EstimatorConverter(inputToEstimator);
            var outputToEstimator = converter.ToEs();

            var TaskDuration = outputToEstimator.TaskDuration;
            var TaskExper = outputToEstimator.TaskExper;
            var TaskAdjacency = outputToEstimator.TaskAdjacency;


            ScheduleEstimator estimator = new ScheduleEstimator(TaskDuration, TaskExper, TaskAdjacency);
            estimator.ForwardMethod();
            List<int[]> ResultMatrix = estimator.Fit();


            return converter.FromEs(ResultMatrix);

        }

    }
}

