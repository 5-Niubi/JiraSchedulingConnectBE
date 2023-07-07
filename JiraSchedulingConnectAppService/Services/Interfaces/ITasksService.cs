using ModelLibrary.DTOs.Tasks;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface ITasksService
    {

        public Task<TaskDTO> CreateTask(TaskDTO task);
        public Task<TaskDTO> UpdateTask(TaskDTO task);
        public Task<TaskDTO> DeleteTask(TaskDTO task);
        public Task<TaskDetailDTO> GetTaskDetail(int Id);
        public Task<List<TaskPertChartDTO>> GetTasksForPertChartProcessing(int projectId);

    }
}

