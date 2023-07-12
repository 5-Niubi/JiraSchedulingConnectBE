using System;
using AutoMapper;
using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.PertSchedule;

namespace JiraSchedulingConnectAppService.Services
{
	public class ParametersService: IParametersService
    {
        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public ParametersService(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;

        }


        public async Task<ParameterDTO> SaveParams(ParameterRequest parameterRequest)
        {
           
            var parameter = mapper.Map<Parameter>(parameterRequest);


            // validate params
            //var paramsWorkforces = parameter.ParameterResources.Where(P => P.Type == Const.RESOURCE_TYPE.WORKFORCE);

            var paramsWorkforces = parameter.ParameterResources;
            var workforcesSkills =  db.WorkforceSkills.Where(wf => paramsWorkforces.Select(p => p.ResourceId).Contains(wf.WorkforceId)).ToList();


            var Tasks = await db.Tasks
                .Include(t => t.TasksSkillsRequireds)
                .Where(t => t.ProjectId == parameter.ProjectId).ToListAsync();


            foreach(var task in Tasks) {

                var skillsRequireds = task.TasksSkillsRequireds.ToList();
                var isAdapted = true;

                foreach (var rq in skillsRequireds)
                {
                    if (!workforcesSkills.Any(wf => wf.SkillId == rq.SkillId && wf.Level >= rq.Level))
                    {
                        isAdapted = false;
                        break;
                    }
                }

                if (isAdapted == false) {
                    string skillSummary = string.Join(", ", task.TasksSkillsRequireds.Select(rq => $"id: {rq.SkillId}, level: {rq.Level}"));

                    throw new Exception("Not workforce appdated: task = " + task.Id + ", Skills required: "+ skillSummary);
                } 


                
            }


            // validate resource params


            // validate duration

            // validate budget

            var paramsEntity =  await db.Parameters.AddAsync(parameter);
            await db.SaveChangesAsync();


            var parameterDTO  = mapper.Map<ParameterDTO>(paramsEntity.Entity);
            return parameterDTO;


        }
    }
}

