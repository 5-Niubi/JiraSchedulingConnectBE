using ModelLibrary.DBModels;
using ModelLibrary.DTOs.PertSchedule;
using ModelLibrary.DTOs.Tasks;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface ITasksService
    {

        public Task<TaskPertViewDTO> CreateTask(TaskCreatedRequest taskRequest);
        public Task<TaskPertViewDTO> UpdateTask(TaskUpdatedRequest taskRequest);
        
        public Task<TaskPertViewDTO> GetTaskDetail(int Id);
        public Task<List<TaskPertViewDTO>> GetTasksPertChart(int projectId);
        public Task<bool> SaveTasks(TasksSaveRequest taskRequest);

        
        //public Task<List<TaskPrecedenceDTO>> SaveTasksPrecedencesTasks(TasksPrecedencesSaveRequest taskRequest);


    }
}

