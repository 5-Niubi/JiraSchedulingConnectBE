﻿using System;
using System.Linq;
using AutoMapper;
using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Invalidator;
using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.PertSchedule;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
	public class ParametersService: IParametersService
    {
        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;


        private const string NotResourceAdaptivedMessage = "Not Resource (Workfore) adapt required skills task's";

        public ParametersService(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;

        }


        private async Task<bool> _ValidateTasksSkillRequireds(int ProjectId, List<ParameterResourceRequest> parameterResourcesRequest) {


            var Errors = new List<TaskSkillRequiredErrorDTO>();

            var WorkforcesSkills = db.Workforces
                .Include(sk => sk.WorkforceSkills)
                .Where(
                wf => parameterResourcesRequest.Select(
                    p => p.ResourceId).Contains(wf.Id))
                .ToList();


            var Tasks = await db.Tasks
                .Include(t => t.TasksSkillsRequireds)
                .Where(t => t.ProjectId == ProjectId).ToListAsync();


            foreach (var task in Tasks)
            {

                var skillsRequireds = task.TasksSkillsRequireds.ToList();
                var num_adapt = skillsRequireds.Count;
                var isAdapted = false;
                
                foreach (var wfk in WorkforcesSkills) {
                    var listSkills = wfk.WorkforceSkills.ToList();
                    var num_check = 0;
                    foreach (var rqSk in skillsRequireds)
                    {
                        for (int i = 0; i < listSkills.Count; i++)
                        {
                            if (listSkills[i].SkillId == rqSk.SkillId && listSkills[i].Level >= rqSk.Level)
                            {
                                num_check += 1;
                            }
                        }
                    }

                    if (num_check == num_adapt)
                    {
                        isAdapted = true;
                        break;
                    }

                }


                // If skills required task's not adaptive then throw
                if (isAdapted == false)
                {
                    Errors.Add(new TaskSkillRequiredErrorDTO
                    {

                        TaskId = task.Id,
                        SkillRequireds = mapper.Map<List<SkillRequiredDTO>>(skillsRequireds),
                        Messages = NotResourceAdaptivedMessage

                    });

                }

            }

                    

            if(Errors.Count != 0) {
                throw new NotSuitableInputException(Errors);
            }


            return true;



        }


        public async Task<ParameterDTO> SaveParams(ParameterRequest parameterRequest)
        {

            // Is validate Resource parameter minimize adaptive Resource Task
            await _ValidateTasksSkillRequireds(parameterRequest.ProjectId, parameterRequest.ParameterResources);


            var parameterRequestDTO = mapper.Map<ParameterRequestDTO>(parameterRequest);
            var paramsEntity =  await db.Parameters.AddAsync(parameterRequestDTO);

            await db.SaveChangesAsync();

            var parameterDTO  = mapper.Map<ParameterDTO>(paramsEntity.Entity);
            return parameterDTO;


        }
    }
}

