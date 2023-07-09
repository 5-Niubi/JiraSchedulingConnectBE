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
using static ModelLibrary.DTOs.PertSchedule.TasksPertCreateTask;

namespace JiraSchedulingConnectAppService.Services
{
    public class TasksService : ITasksService
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


        private async Task<bool> _ValidatePrecedenceTask(List<PrecedenceRequest> Precedences, int? ProjectId) {

            var precedenceIds = new HashSet<int>(Precedences.Select(p => p.PrecedenceId));
            var exitedPrecedences = await db.Tasks
                .Where(t => precedenceIds.Contains(t.Id) & t.ProjectId == ProjectId & t.IsDelete==false)
                .ToListAsync();

            if (exitedPrecedences.Count != Precedences.Count)
            {
                throw new Exception(PredenceNotExitedMessage);
            }

            return true;

        }

        private async Task<bool> _ValidateMilestoneTask(int MilestoneId, int ProjectId)
        {
            // validate milestone task  project's 
            var existingMilestone = await db.Milestones.FirstOrDefaultAsync(
                t => t.Id == MilestoneId
                & t.ProjectId == ProjectId
                );

            if (existingMilestone == null)
            {
                throw new Exception(MilestoneNotValidMessage);
            }
            return true;
        }
        


        private async Task<bool> _ValidateSkillsRequired(List<SkillRequest> RequiredSkills, string cloudId) {

        

            //validate exited on database
            var RequiredSkillsId = new HashSet<int>(RequiredSkills.Select(p => p.SkillId));
            var exitedSkills = await db.Skills
                .Where(s => RequiredSkillsId.Contains(s.Id) & s.CloudId == cloudId & s.IsDelete == false)
                .ToListAsync();



            if (exitedSkills.Count != RequiredSkillsId.Count)
            {
                throw new Exception(RequiredSkillNotValidMessage);
            }



            // validate level skill
            foreach (var skill in RequiredSkills)
            {
                if (skill.Level < 1 || skill.Level > 5)
                {
                    throw new Exception(RequiredSkillNotValidMessage);
                }
            }

            return true;
       
        }


        public async Task<List<TasksSkillsRequired>> CreateTaskSkillRequired(
            int TaskId,
            List<SkillRequest> RequiredSkills) {

            // define required skills task
            List<TasksSkillsRequired> TaskskillsRequiredList = new List<TasksSkillsRequired>();

            for (int i = 0; i< RequiredSkills.Count; i++)
            {
                var taskSkillRequired = new TasksSkillsRequired();
                taskSkillRequired.SkillId = RequiredSkills[i].SkillId;
                taskSkillRequired.Level = RequiredSkills[i].Level;
                taskSkillRequired.TaskId = TaskId;

                TaskskillsRequiredList.Add(taskSkillRequired);

            }

            // insert required skill task
            db.TasksSkillsRequireds.AddRange(TaskskillsRequiredList);
            await db.SaveChangesAsync();

            return TaskskillsRequiredList;
            }



        private async Task<List<TaskPrecedence>> CreateTaskPrecedenceTasks(List<PrecedenceRequest> precedencesDTO)
        {


            var taskPrecedenceTasks = mapper.Map<List<TaskPrecedence>>(precedencesDTO);
            // insert task predences task
            db.TaskPrecedences.AddRange(taskPrecedenceTasks);
            await db.SaveChangesAsync();

            return taskPrecedenceTasks;
        }


        public  async Task<TaskDetailDTO> CreateTask(TasksPertCreateTask.TaskRequest taskRequest)
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


                // validate milestone task
                await _ValidateMilestoneTask(taskRequest.MilestoneId, taskRequest.ProjectId);

                // validate exited predences in this project
                await _ValidatePrecedenceTask(taskRequest.Precedences, taskRequest.ProjectId);

                // validate required skills task's
                await _ValidateSkillsRequired(taskRequest.SkillRequireds, cloudId) ;

                // validate is DAG graph
                // TODO 


                // define task

                // TODO mapper list object in class
                var task = mapper.Map<ModelLibrary.DBModels.Task>(taskRequest);
                var precedences = mapper.Map<List<TaskPrecedence>>(taskRequest.Precedences);
                var requiredSkills = mapper.Map<List<TasksSkillsRequired>>(taskRequest.SkillRequireds);
                task.TaskPrecedenceTasks = precedences;
                task.TasksSkillsRequireds = requiredSkills;
                task.CloudId = cloudId;

                // insert task
                var taskCreatedEntity = await db.Tasks.AddAsync(task);
                await db.SaveChangesAsync();

                

                //taskCreatedDTO.Precedences = taskRequest.Precedences;
                //taskCreatedDTO.SkillRequireds = taskRequest.SkillRequireds;


                // create task skill required
                CreateTaskSkillRequired(taskCreatedEntity.Entity.Id, taskRequest.SkillRequireds);

                // create precedence tasks
                CreateTaskPrecedenceTasks(taskRequest.Precedences);


                // TODO mapper list object in class    
                var taskCreatedDTO = mapper.Map<TaskDetailDTO>(taskCreatedEntity.Entity);
                taskCreatedDTO.SkillRequireds = mapper.Map<List<SkillRequiredDTO>>(taskCreatedEntity.Entity.TasksSkillsRequireds);
                taskCreatedDTO.Precedences = mapper.Map<List<PrecedenceDTO>>(taskCreatedEntity.Entity.TaskPrecedenceTasks);

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

            var task = await db.Tasks
                .Include(t => t.TaskPrecedenceTasks)
                .Include(t => t.TasksSkillsRequireds)
                .FirstOrDefaultAsync(
                    t => t.Id == Id
                    && t.IsDelete == false);

            if(task == null) {
                throw new Exception(NotFoundMessage);
            }
            var taskDetailDTO = mapper.Map<TaskDetailDTO>(task);
            taskDetailDTO.SkillRequireds = mapper.Map<List<SkillRequiredDTO>>(task.TasksSkillsRequireds);
            taskDetailDTO.Precedences = mapper.Map<List<PrecedenceDTO>>(task.TaskPrecedenceTasks);
            return taskDetailDTO;


        }

        public Task<List<TaskPertDetailDTO>> GetTasksForPertChartProcessing(int projectId)
        {
            throw new NotImplementedException();
        }



        

        //public Task<TaskDetailDTO> UpdateTask(TasksListCreateTask.Request task)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

