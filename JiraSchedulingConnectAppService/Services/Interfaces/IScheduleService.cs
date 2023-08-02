using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Algorithm;
using ModelLibrary.DTOs.Schedules;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IScheduleService
    {
        public Task<PagingResponseDTO<SchedulesListResDTO>> GetSchedulesByProject(int projectId, int? page);
        public Task<PagingResponseDTO<SchedulesListResDTO>> GetSchedules(int parameterId, int? page);
        public Task<ScheduleResultSolutionDTO> GetSchedule(int scheduleId);
		public Task<ScheduleResultSolutionDTO> SaveScheduleSolution(ScheduleRequestDTO scheduleRequestDTO);

    }
}
