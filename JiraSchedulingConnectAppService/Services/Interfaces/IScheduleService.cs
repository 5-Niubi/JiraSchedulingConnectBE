using ModelLibrary.DTOs.Algorithm;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IScheduleService
    {
        public Task<List<ScheduleResultSolutionDTO>> GetSchedules(int parameterId);
        public Task<ScheduleResultSolutionDTO> GetSchedule(int scheduleId);
		public Task<ScheduleResultSolutionDTO> SaveScheduleSolution(ScheduleRequestDTO scheduleRequestDTO);
	}
}
