using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.PertSchedule;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Tasks;

namespace JiraSchedulingConnectAppService.Services
{
    public class TasksService: ITasksService
    {
        public const string NotFoundMessage = "Task Not Found!";
        public const string NotUniqueTaskNameMessage = "Task Name Must Unique!";
        public const string PredenceNotExitedMessage = "Predence Task  not valid!";
        public const string RequiredSkillNotValidMessage = "Required Skill not valid!";
        public const string MilestoneNotValidMessage = "Milestone Task's not valid!";

        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public TasksService(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.db = dbContext;
            this.mapper = mapper;
            this.httpContext = httpContextAccessor.HttpContext;
        }


        private async Task<bool> _ValidatePrecedenceTask(List<int> PrecedencesId, int? ProjectId ) {
            var exitedPrecedences = await db.Tasks.Where(
            t => PrecedencesId.Contains(t.Id)
                & t.ProjectId == ProjectId
                & t.IsDelete == false
                ).ToListAsync();

            if (PrecedencesId.Count != exitedPrecedences.Count)
            {
                throw new Exception(PredenceNotExitedMessage);
            }

            return true;

        }




        private async Task<bool> _ValidateSkillsRequired(List<int> RequiredSkillsId, List<int> RequiredSkillsLevel, string cloudId) {

        

            //validate exited on database
            var exitedSkills = await db.Skills.Where(
                    s => RequiredSkillsId.Contains(s.Id)
                    & s.CloudId == cloudId
                    & s.IsDelete == false

                    )
                    .ToListAsync();

            if (exitedSkills.Count != RequiredSkillsId.Count)
            {
                throw new Exception(RequiredSkillNotValidMessage);
            }

            // validate level skill
            foreach(int level in RequiredSkillsLevel) {
                if(level < 1 || level > 5){
                    throw new Exception(RequiredSkillNotValidMessage);
                }
            }
            return true;
       
        }



        private async Task<List<TasksSkillsRequired>> _CreateTaskSkillRequired(int TaskId, List<int> RequiredSkillsId, List<int> RequiredSkillsLevel) {
            // define required skills task
            List<TasksSkillsRequired> TaskskillsRequiredList = new List<TasksSkillsRequired>();

            for (int i = 0; i< RequiredSkillsId.Count; i++)
            {
                var skillReqDTO = new TasksSkillsRequired();
                skillReqDTO.SkillId = RequiredSkillsId[i];
                skillReqDTO.Level = RequiredSkillsLevel[i];
                skillReqDTO.TaskId = TaskId;

                TaskskillsRequiredList.Add(skillReqDTO);

            }

            // insert required skill task
            db.TasksSkillsRequireds.AddRange(TaskskillsRequiredList);
            await db.SaveChangesAsync();

            return TaskskillsRequiredList;
            }



        private async Task<List<int>> _CreatePrecedenceTasks(int TaskId, List<int> PrecedencesId)
        {
            // define task precendence tasks
            var taskPrecedences = PrecedencesId.Select(
                precedenceId => new TaskPrecedence
                {
                    TaskId = TaskId,
                    PrecedenceId = precedenceId
                });



            // insert task predences task
            db.TaskPrecedences.AddRange(taskPrecedences);
            await db.SaveChangesAsync();

            return PrecedencesId;
        }


        public  async Task<TaskDetailDTO> CreateTask(TasksPertCreateTask.Request taskRequest)
        {

            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();


                // validate exited name task  project's 
                var existingTask = await db.Tasks.FirstOrDefaultAsync(
                    t => t.Name == taskRequest.Name
                    & t.CloudId == cloudId
                    & t.ProjectId == taskRequest.ProjectId
                    & t.IsDelete == false);

                if (existingTask != null)
                {
                    throw new Exception(NotUniqueTaskNameMessage); 
                }

                // validate exited name task  project's 
                var existingMilestone = await db.Milestones.FirstOrDefaultAsync(
                    t => t.Id == taskRequest.MilestoneId
                    & t.ProjectId == taskRequest.ProjectId
                    );

                if (existingMilestone == null)
                {
                    throw new Exception(MilestoneNotValidMessage);
                }



                // validate exited predences in this project
                await _ValidatePrecedenceTask(taskRequest.PrecedencesId, taskRequest.ProjectId);

                // validate required skills task's
                await _ValidateSkillsRequired(taskRequest.RequiredSkillsId, taskRequest.RequiredSkillsLevel, cloudId) ;

                // validate is DAG graph
                // TODO 

   
                // define task
                var task = mapper.Map<ModelLibrary.DBModels.Task>(taskRequest);

                task.CloudId = cloudId;
                task.IsDelete = false;

                // insert task
                var taskCreatedEntity = await db.Tasks.AddAsync(task);
                await db.SaveChangesAsync();

                var taskCreatedDTO = mapper.Map<TaskDetailDTO>(taskCreatedEntity.Entity);

                // create task skill required
                _CreateTaskSkillRequired(taskCreatedDTO.Id, taskRequest.RequiredSkillsId, taskRequest.RequiredSkillsLevel);


                // create precedence tasks
                _CreatePrecedenceTasks(taskCreatedDTO.Id, taskCreatedDTO.PrecedencesId);


                taskCreatedDTO.PrecedencesId = taskRequest.RequiredSkillsId;
                taskCreatedDTO.RequiredSkillsId = taskRequest.RequiredSkillsLevel;
                taskCreatedDTO.PrecedencesId = taskRequest.PrecedencesId;

                return taskCreatedDTO;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        //public Task<bool> DeleteTask(int Id)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<TaskDetailDTO> GetTaskDetail(int Id)
        {

            var task = await db.Tasks.FirstOrDefaultAsync(
                    t => t.Id == Id
                    && t.IsDelete == false);

            if(task == null) {
                throw new Exception(NotFoundMessage);
            }
            var taskDetaiDTO = mapper.Map<TaskDetailDTO>(task);

            return taskDetaiDTO;


        }

        public Task<List<TaskPertDetailDTO>> GetTasksForPertChartProcessing(int projectId)
        {
            throw new NotImplementedException();
        }

        

        //public Task<List<TaskPertDetailDTO>> GetTasksForPertChartProcessing(int projectId)
        //{
        //    var jwt = new JWTManagerService(httpContext);
        //    var cloudId = jwt.GetCurrentCloudId();


        //    var query = db.Tasks.Where(t => t.CloudId == cloudId
        //        && t.ProjectId == projectId & t.IsDelete == false
        //        )
        //        .OrderByDescending(e => e.Id);

        //    var taskDetaisResult = await query.ToListAsync();
        //    var taskDetaiDTO = mapper.Map<List<TaskPertDetailDTO>>(projectsResult);

        //    return taskDetaiDTO;
        //}

        //public Task<TaskDetailDTO> UpdateTask(TasksListCreateTask.Request task)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

