using System.Net.Http;
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
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
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

        public Task<bool> DeleteTask(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<TaskDetailDTO> GetTaskDetail(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<TaskPertChartDTO>> GetTasksForPertChartProcessing(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<TaskDetailDTO> UpdateTask(TasksListCreateTask.Request task)
        {
            throw new NotImplementedException();
        }
    }
}

