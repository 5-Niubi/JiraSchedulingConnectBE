using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Invalidation;
using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Skills;
using Newtonsoft.Json.Linq;
using UtilsLibrary.Exceptions;
using static ModelLibrary.DTOs.Export.JiraAPICreateBulkTaskResDTO;
using static ModelLibrary.DTOs.Invalidation.WorkforceInputErrorDTO;

namespace JiraSchedulingConnectAppService.Services
{
    public class WorkforcesService : IWorkforcesService
    {

        public const string WorkforceTyNotVaMessage = "Workforce Typalidatee Is Not Validated!!!";

        public const string EffortNotValidMessage = "Effort Is Not Validated!!!";
        public const string EffortElementNotValidMessage = "Effort must only have 7 elements!!!";
        public const string SkillNotFoundVaMessage = "Skill Workforce Is Not Found!!!";
        public const string SkillLevelNotValidateMessage = "Skill Level Workforce is not validate!!!";

        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;


        public WorkforcesService(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
        }
        public async Task<List<WorkforceDTOResponse>> GetAllWorkforces(List<int>? Ids)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var query = (Ids == null) ?
                    await db.Workforces.ToListAsync() : await db.Workforces.Where(
                    W => Ids.Contains(W.Id) == true).ToListAsync();
                var queryDTOResponse = mapper.Map<List<WorkforceDTOResponse>>(query);
                return queryDTOResponse;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }






        private async Task<List<SkillRequestErrorDTO>> _ValidateWorkforceSkills(WorkforceRequestDTO WorkforceRequest)
        {
            

            var jwt = new JWTManagerService(httpContext);
            //var cloudId = jwt.GetCurrentCloudId();
            var cloudId = "ea48ddc7-ed56-4d60-9b55-02667724849d"; // TODO: fix CLOUD ID

            //validate exited on database
            var exitedSkills = await db.Skills
                .Where(s => s.CloudId == cloudId & s.IsDelete == false)
                .ToListAsync();


            var SkillErrors = new List<SkillRequestErrorDTO>();
            foreach (var skill in WorkforceRequest.Skills)
            {

                if (!exitedSkills.Select(s => s.Id).Contains(skill.SkillId))
                {
                    SkillErrors.Add(
                        new SkillRequestErrorDTO
                        {
                            SkillId = skill.SkillId,
                            Messages = SkillNotFoundVaMessage
                        });

                }

                if (skill.Level < 1 || skill.Level > 5)
                {
                    SkillErrors.Add(
                        new SkillRequestErrorDTO
                        {
                            SkillId = skill.SkillId,
                            Level = skill.Level,
                            Messages = SkillLevelNotValidateMessage
                        });

                }

            }


            return SkillErrors;

        }

        private async Task<List<WorkingEffortErrorDTO>> _ValidateWorkforceEfforts(WorkforceRequestDTO WorkforceRequest) {

            var EffortErrors = new List<WorkingEffortErrorDTO>();



            // working effort
            var WorkingEfforts = WorkforceRequest.WorkingEfforts;

            if (WorkingEfforts.Count != 7) {

                throw new NotSuitableInputException(
                        new WorkforceInputErrorDTO()
                        {
                            Messages = EffortElementNotValidMessage
                        }
                    );
                    

            }

            for (int i = 0; i < WorkingEfforts.Count; i++) {
                if (WorkingEfforts[i] < 0 || WorkingEfforts[i] > 1) {
                    EffortErrors.Add(new WorkingEffortErrorDTO
                    {
                        DayIndex = i,
                        Effort = WorkingEfforts[i],
                        Message = EffortNotValidMessage
                    });
                }

            }


            return EffortErrors;
    }


    public async Task<WorkforceDTOResponse> CreateWorkforce(WorkforceRequestDTO? workforceRequest)
        {
           
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();
            var workforce = mapper.Map<Workforce>(workforceRequest);
            


            // TODO: validate email

            // TODO: validate working type



            // validate efforts
            var EffortErrors = await _ValidateWorkforceEfforts(workforceRequest);


            // validate skills
            var SkillErrors = await _ValidateWorkforceSkills(workforceRequest);


            if (EffortErrors.Count != 0 || SkillErrors.Count != 0) {
                throw new NotSuitableInputException(
                    new WorkforceInputErrorDTO()
                    {

                        Skills = SkillErrors,
                        Efforts = EffortErrors

                    }
                    );
                    

            }


            // insert workforce
            // insert workforce skill


            var workforceEntity = await db.Workforces.AddAsync(workforce);
            await db.SaveChangesAsync();
            var workforceDTOResponse = mapper.Map<WorkforceDTOResponse>(workforceEntity.Entity);
            return workforceDTOResponse;
         
            
        }

        public async Task<WorkforceDTOResponse> GetWorkforceById(string workforce_id)
        {
            try
            {
                var workforce = await db.Workforces.Where(e => e.Id.ToString() == workforce_id).FirstOrDefaultAsync();
                var workforceResponse = mapper.Map<WorkforceDTOResponse>(workforce);
                return workforceResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<WorkforceDTOResponse> DeleteWorkforce(string? workforce_id)
        {
            try
            {
                var workforce = await db.Workforces.SingleOrDefaultAsync(x => x.Id.ToString() == workforce_id);
                if(workforce == null) {
                    return null;
                }
                    workforce.IsDelete = true;
                    workforce.DeleteDatetime = DateTime.Now;
                var workforceEntity = db.Workforces.Update(workforce);
                await db.SaveChangesAsync();
                var workforceResponse = mapper.Map<WorkforceDTOResponse>(workforceEntity.Entity);
                return workforceResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<WorkforceDTOResponse> UpdateWorkforce(WorkforceDTORequest workforceRequest)
        {
            try
            {
                var workforce = mapper.Map<Workforce>(workforceRequest);
                var workforceEntity = db.Update(workforce);
                await db.SaveChangesAsync();
                var workforceDTOResponse = mapper.Map<WorkforceDTOResponse>(workforceEntity.Entity);
                return workforceDTOResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

}

