using ModelLibrary.DBModels;
using ModelLibrary.DTOs.PertSchedule;
using ModelLibrary.DTOs.Tasks;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface ITasksService
    {

        public Task<TaskDetailDTO> CreateTask(TasksPertCreateTask.TaskRequest taskRequest);
        //public Task<TaskDetailDTO> UpdateTask(TasksListCreateTask.Request task);
        //public Task<bool> DeleteTask(int Id);
        public Task<TaskDetailDTO> GetTaskDetail(int Id);
        public Task<List<TaskPertDetailDTO>> GetTasksForPertChartProcessing(int projectId);

    }
}

