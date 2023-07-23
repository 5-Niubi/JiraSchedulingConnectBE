﻿using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Algorithm;
using ModelLibrary.DTOs.Schedules;
using UtilsLibrary;
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

        public async Task<PagingResponseDTO<SchedulesListResDTO>> GetSchedulesByProject(int projectId, int? page)
        {
            var query = db.Schedules.Include(s => s.Parameter)
                .Where(s => s.Parameter.ProjectId == projectId);

            int totalPage = 0, totalRecord = 0;
            if (page != null)
            {
                (query, totalPage, page, totalRecord) = 
                    UtilsLibrary.Utils.MyQuery<Schedule>.Paging(query, (int) page);
            }else
            {
                page = 0;
            }

            var schedule = await query.ToListAsync();
            var scheduleDTO = mapper.Map<List<SchedulesListResDTO>>(schedule);

            var pagingRespone = new PagingResponseDTO<SchedulesListResDTO>()
            {
                Values = scheduleDTO,
                MaxResults = totalRecord,
                PageIndex = (int) page,
                PageSize = Const.PAGING.NUMBER_RECORD_PAGE,
                StartAt = 1,
                Total = totalPage
            };
            return pagingRespone;
        }

        public async Task<PagingResponseDTO<SchedulesListResDTO>> GetSchedules(int parameterId, int? page)
        {
            var query =  db.Schedules.Where(s => s.ParameterId == parameterId);
                
            int totalPage = 0, totalRecord = 0;
            if (page != null)
            {
                (query, totalPage, page, totalRecord) =
                    UtilsLibrary.Utils.MyQuery<Schedule>.Paging(query, (int)page);
            }
            else
            {
                page = 0;
            }

            var schedule = await query.ToListAsync();
            var scheduleDTO = mapper.Map<List<SchedulesListResDTO>>(schedule);

            var pagingRespone = new PagingResponseDTO<SchedulesListResDTO>()
            {
                Values = scheduleDTO,
                MaxResults = totalRecord,
                PageIndex = (int)page,
                PageSize = Const.PAGING.NUMBER_RECORD_PAGE,
                StartAt = 1,
                Total = totalPage
            };
            return pagingRespone;
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
