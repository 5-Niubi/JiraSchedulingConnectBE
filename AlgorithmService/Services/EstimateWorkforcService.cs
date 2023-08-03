using AlgorithmLibrary;
using AlgorithmServiceServer.DTOs.AlgorithmController;
using AlgorithmServiceServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace AlgorithmServiceServer.Services
{
    public class EstimateWorkforcService : IEstimateWorkforceService
    {


        private const string EmptyTaskInProjectMessage = "Empty Task in this project";

        private readonly JiraDemoContext db;
        private readonly HttpContext http;

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

            var cloudId = new JWTManagerService(http).GetCurrentCloudId();

            var projectFromDB = await db.Projects
                .Where(p => p.CloudId == cloudId)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync();

            var skillFromDB = db.Skills
                .Where(s => s.CloudId == cloudId)
                .ToList();

            var taskFromDB = db.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.TaskPrecedenceTasks)
                .Include(t => t.TasksSkillsRequireds)
                .ToList();


            if (taskFromDB.Count == 0)
            {
                throw new NotSuitableInputException(EmptyTaskInProjectMessage);

            }

            var inputToEstimator = new InputToEstimatorDTO();
            inputToEstimator.SkillList = skillFromDB;
            inputToEstimator.TaskList = taskFromDB;

            // convert from input data (db) -> input estimator's
            var converter = new EstimatorConverter(inputToEstimator);
            var outputToEstimator = converter.ToEs();


            var TaskDuration = outputToEstimator.TaskDuration;
            var TaskExper = outputToEstimator.TaskExper;
            var TaskAdjacency = outputToEstimator.TaskAdjacency;
            var TaskMilestone = outputToEstimator.TaskMilestone;

            // forward BFS method
            ScheduleEstimator estimator = new ScheduleEstimator(TaskDuration, TaskMilestone, TaskExper, TaskAdjacency);
            estimator.ForwardMethod();

            // fit estimate
            Dictionary<int, List<int[]>> Results = estimator.Fit();


            // Post processing
            var estimatedResultDTO = new EstimatedResultDTO();
            List<WorkforceWithMilestoneDTO> WorkforceWithMilestoneList = new List<WorkforceWithMilestoneDTO>();
            foreach (int milestoneId in Results.Keys)
            {
                List<int[]> result = Results[milestoneId];
                WorkforceWithMilestoneList.Add(converter.FromEs(milestoneId, result));
            }

            estimatedResultDTO.WorkforceWithMilestoneList = WorkforceWithMilestoneList;

            return estimatedResultDTO;

        }

        public async Task<EstimatedResultDTO> ExecuteOverall(int projectId)
        {


            var WorkforceOutputList = new List<WorkforceOutputFromEsDTO>();

            var cloudId = new JWTManagerService(http).GetCurrentCloudId();

            var projectFromDB = await db.Projects
                .Where(p => p.CloudId == cloudId)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync();

            var skillFromDB = db.Skills
                .Where(s => s.CloudId == cloudId)
                .ToList();

            var taskFromDB = db.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.TaskPrecedenceTasks)
                .Include(t => t.TasksSkillsRequireds)
                .ToList();


            if (taskFromDB.Count == 0)
            {
                throw new NotSuitableInputException(EmptyTaskInProjectMessage);

            }

            var inputToEstimator = new InputToEstimatorDTO();
            inputToEstimator.SkillList = skillFromDB;
            inputToEstimator.TaskList = taskFromDB;

            // convert from input data (db) -> input estimator's
            var converter = new EstimatorConverter(inputToEstimator);
            var outputToEstimator = converter.ToEs();


            var TaskDuration = outputToEstimator.TaskDuration;
            var TaskExper = outputToEstimator.TaskExper;
            var TaskAdjacency = outputToEstimator.TaskAdjacency;
            var TaskMilestone = outputToEstimator.TaskMilestone;

            // forward BFS method
            ScheduleEstimator estimator = new ScheduleEstimator(TaskDuration, TaskMilestone, TaskExper, TaskAdjacency);
            estimator.ForwardMethod();

            // fit estimate
            Dictionary<int, List<int[]>> Results = estimator.Fit();

            List<int[]> overallResults = new List<int[]>();
            foreach (int milestoneId in Results.Keys)
            {
                List<int[]> result = Results[milestoneId];
                overallResults.AddRange(result);


            }

            // Post processing
            var estimatedResultDTO = new EstimatedResultDTO();

            var newList = new List<WorkforceWithMilestoneDTO>() { };
            newList.Add(converter.FromEs(1, overallResults));

            estimatedResultDTO.WorkforceWithMilestoneList = newList;

            return estimatedResultDTO;


        }
    }
}

