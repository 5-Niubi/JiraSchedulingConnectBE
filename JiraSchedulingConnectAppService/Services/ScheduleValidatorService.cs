using AlgorithmServiceServer.DTOs.AlgorithmController;
using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.PertSchedule;
using AlgorithmLibrary;

namespace AlgorithmServiceServer.Services.Interfaces
{
    public class ScheduleValidatorService : IValidatorService
    {


        private readonly JiraDemoContext db;
        private readonly HttpContext httpContext;
        private readonly IWorkforcesService workforceService;


        private DirectedGraph DirectedGraph;

        public ScheduleValidatorService(
            JiraDemoContext db,
            IHttpContextAccessor httpAccessor,
            IWorkforcesService workforceService
            )
        {
            this.db = db;
            httpContext = httpAccessor.HttpContext;
            this.workforceService = workforceService;

        }

        public Task<bool> IsValidDAG(int projectId)
        {
            throw new NotImplementedException();
        }

        //public async Task<bool> IsValidDAG(int projectId)
        //{

        //    // get all task active by project id
        //    var jwt = new JWTManagerService(httpContext);
        //    var cloudId = jwt.GetCurrentCloudId();

        //    var projectFromDB = await db.Projects
        //        .Where(p => p.CloudId == cloudId)
        //        .Include(p => p.Tasks)
        //        .FirstOrDefaultAsync();

        //    var skillFromDB = db.Skills
        //        .Where(s => s.CloudId == cloudId)
        //        .ToList();

        //    var taskFromDB = db.Tasks
        //        .Where(t => t.ProjectId == projectId)
        //        .Include(t => t.TaskPrecedenceTasks)
        //        .Include(t => t.TasksSkillsRequireds)
        //        .ToList();


        //    var inputToEstimator = new InputToEstimatorDTO();
        //    inputToEstimator.SkillList = skillFromDB;
        //    inputToEstimator.TaskList = taskFromDB;

        //    // convert from input data (db) -> input estimator's
        //    var converter = new EstimatorConverter(inputToEstimator);
        //    var outputToEstimator = converter.ToEs();


        //    var TaskAdjacency = outputToEstimator.TaskAdjacency;

        //    // validate DAG tasks in by projectid

        //    int startNode = 0;
        //    DirectedGraph = new DirectedGraph(startNode);
        //    DirectedGraph.LoadData(TaskAdjacency);


        //    return true;

        //}

        //public Task<bool> IsValidDAG(string projectId)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<bool> IsValidRequiredParameters(ParameterRequestDTO parameterRequest)
        {

            // TODO: is validate duration

            //var projectId = parameterRequest.ProjectId;
            //var workforeIds = parameterRequest.ParameterResourceRequest;


            //var listTasks = await db.Tasks
            //    .Include(t => t.TasksSkillsRequireds)
            //    .Where(t => t.ProjectId == projectId).ToListAsync();

            //var WorkforcesSkills = await db.WorkforceSkills.Where(w => workforeIds.Contains(w.SkillId)).ToListAsync();


            //foreach(var task in listTasks) {
            //    foreach(var skill)
            //}
            return true;

            //foreach(var taskSkillRequired in TaskSkillrequireds) {
            //    var task = taskSkillRequired.
            //}
            //var workforces = await this.workforceService.GetAllWorkforces(workforeIds);


            // is validate workforce adapted with task required

        }
    }
}

