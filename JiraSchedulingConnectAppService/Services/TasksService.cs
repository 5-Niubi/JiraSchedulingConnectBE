using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Tasks;

namespace JiraSchedulingConnectAppService.Services
{
    public class TasksService: ITasksService
    {
        public const string NotFoundMessage = "Task Not Found!!!";
        public const string NotUniqueSkillNameMessage = "Task Name Must Unique!!!";

        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public TasksService(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.db = dbContext;
            this.mapper = mapper;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public  async Task<TaskDetailDTO> CreateTask(TasksListCreateTask.Request taskRequest)
        {

            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var task = mapper.Map<ModelLibrary.DBModels.Task>(taskRequest);
                task.CloudId = cloudId;

                // Check Name project's exited
                // if not exited -> insert
                // else throw error
                var existingTask = await db.Tasks.FirstOrDefaultAsync(
                    t => t.Name == task.Name
                    && t.CloudId == cloudId
                    && t.ProjectId == task.ProjectId
                    && t.IsDelete == false);

                if (existingTask != null)
                {
                    throw new Exception(NotUniqueSkillNameMessage); // Or handle the situation accordingly
                }

                // is validate predence tasks
                // TODO
                // is validate DAG
                // TODO


                var taskCreatedEntity = await db.Tasks.AddAsync(task);
                await db.SaveChangesAsync();
                var taskCreatedDTO = mapper.Map<TaskDetailDTO>(taskCreatedEntity.Entity);
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

        public Task<List<TaskPertChartDTO>> GetTasksForPertChartProcessing(int projectId)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();


            var query = db.Tasks.Where(t => t.CloudId == cloudId
                && t.ProjectId == projectId & t.IsDelete == false
                )
                .OrderByDescending(e => e.Id);

            var projectsResult = await query.ToListAsync();
            var projectDTO = mapper.Map<List<ProjectListHomePageDTO>>(projectsResult);

            return taskDetaiDTO;
        }

        //public Task<TaskDetailDTO> UpdateTask(TasksListCreateTask.Request task)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

