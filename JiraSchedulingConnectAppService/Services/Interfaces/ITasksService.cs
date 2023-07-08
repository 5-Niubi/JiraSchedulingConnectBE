using ModelLibrary.DTOs.Tasks;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface ITasksService
    {

        public Task<TaskDetailDTO> CreateTask(TasksListCreateTask.Request taskRequest);
        public Task<TaskDetailDTO> UpdateTask(TasksListCreateTask.Request task);
        public Task<bool> DeleteTask(int Id);
        public Task<TaskDetailDTO> GetTaskDetail(int Id);
        public Task<List<TaskPertChartDTO>> GetTasksForPertChartProcessing(int projectId);

    }
}

