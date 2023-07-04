
using AlgorithmServiceServer.DTOs.AlgorithmController;
using AlgorithmServiceServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using RcpEstimator;
using RcpspAlgorithmLibrary;


namespace AlgorithmServiceServer.Services
{
    public class EstimateWorkerService : IEstimateWorkerService
    {

        private readonly JiraDemoContext db;
        private readonly HttpContext http;
        private List<int> TaskDuration;
        private List<List<int>> TaskExper;
        private List<List<int>> TaskAdjacency;
        public EstimateWorkerService(JiraDemoContext db, IHttpContextAccessor httpAccessor)
        {
            this.db = db;
            http = httpAccessor.HttpContext;
        }

        public List<int> GetTaskDuration(int projectId)
        {
            List<int> TaskDuration = new List<int>();

            // TODO
            // Get data from relate tables: Task
            // Encode to matrix TaskDuration, TaskExper, TaskAdjacency

            TaskDuration = new List<int> { 0, 10, 1, 3, 0 };



            return TaskDuration;
        }


        public List<List<int>> GetTaskExper(int projectId)
        {
            List<List<int>> TaskExper = new List<List<int>>();
            TaskExper = new List<List<int>>()
                        {
                            new List<int> {0,0,0},
                            new List<int> {1,0,2},
                            new List<int> {3,0,3},
                            new List<int> {2,0,4},
                            new List<int> {0,0,0}
                        };


            return TaskExper;
        }

        public List<List<int>> GetTaskAdjacency(int projectId)
        {
            List<List<int>> TaskAdjacency = new List<List<int>>();
            TaskAdjacency = new List<List<int>>()
                                                {
                                                    new List<int> {0, 0, 0, 0, 0},
                                                    new List<int> {1, 0, 0, 0, 0},
                                                    new List<int> {0, 1, 0, 0, 0},
                                                    new List<int> {0, 1, 0, 0, 0},
                                                    new List<int> {0, 0, 1, 1, 0}
                                                };


            return TaskAdjacency;
        }

        public void PrepareDataFromDB(int projectId)
        {


            var taskFromDB = db.Tasks.Where(t => t.ProjectId == projectId)
                .Include(t => t.TaskPrecedencePrecedences).Include(t => t.TasksSkillsRequireds);

            // TODO: Create list unique skills project's map with Id skill database's
            // TODO: Create list unique tasks project's map with Id task database's

            // TODO: Create TaskAdjacency

            // TODO: Create TaskDuration

            // TODO: Create TaskExper

            // TODO: Create TaskMistone

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
                var taskPrecedences = task.TaskPrecedencePrecedences;
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

