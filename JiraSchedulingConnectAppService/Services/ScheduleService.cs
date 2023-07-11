using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
    public class ScheduleService : IScheduleService
    {

        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        public ScheduleService(JiraDemoContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<ScheduleResultSolutionDTO>> GetSchedules(int parameterId)
        {
            var schedule = await db.Schedules.Where(s => s.ParameterId == parameterId)
                 .ToListAsync();
            var scheduleDTO = mapper.Map<List<ScheduleResultSolutionDTO>>(schedule);
            return scheduleDTO;
        }

        public async Task<ScheduleResultSolutionDTO> GetSchedule(int scheduleId)
        {
            var schedule = await db.Schedules.Where(s => s.Id == scheduleId)
                 .FirstOrDefaultAsync() ??
            throw new NotFoundException($"Can not find schedule: {scheduleId}");
            var scheduleDTO = mapper.Map<ScheduleResultSolutionDTO>(schedule);
            return scheduleDTO;
        }
    }
}
