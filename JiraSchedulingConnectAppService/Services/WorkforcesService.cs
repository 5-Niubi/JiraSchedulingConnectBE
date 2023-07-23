using AutoMapper;
using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Invalidation;
using ModelLibrary.DTOs.Parameters;
using UtilsLibrary.Exceptions;


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
                    await db.Workforces.Include(s => s.WorkforceSkills).ThenInclude(s => s.Skill).ToListAsync() : await db.Workforces.Where(
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

        private async Task<List<WorkingEffortErrorDTO>> _ValidateWorkforceEfforts(WorkforceRequestDTO WorkforceRequest)
        {
            var EffortErrors = new List<WorkingEffortErrorDTO>();

            // working effort
            var WorkingEfforts = WorkforceRequest.WorkingEfforts;

            if (WorkingEfforts.Count != 7)
            {

                throw new NotSuitableInputException(
                        new WorkforceInputErrorDTO()
                        {
                            Messages = EffortElementNotValidMessage
                        }
                    );
            }

            for (int i = 0; i < WorkingEfforts.Count; i++)
            {
                if (WorkingEfforts[i] < 0 || WorkingEfforts[i] > 1)
                {
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
            if (EffortErrors.Count != 0 || SkillErrors.Count != 0)
            {
                throw new NotSuitableInputException(
                    new WorkforceInputErrorDTO()
                    {
                        Skills = SkillErrors,
                        Efforts = EffortErrors
                    }
                    );
            }
            // insert workforce
            var workforceEntity = await db.Workforces.AddAsync(workforce);
            // insert workforce skill
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
                if (workforce == null)
                {
                    return null;
                }
                workforce.IsDelete = Const.DELETE_STATE.DELETE;
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

        public async Task<WorkforceDTOResponse> UpdateWorkforce(WorkforceRequestDTO workforceRequest)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var workforceDB = db.Workforces.FirstOrDefault(s => s.Id == workforceRequest.Id) ??
                    throw new NotFoundException($"Can not find project :{workforceRequest.Id}");
                workforceDB.AccountId = workforceRequest.AccountId;
                workforceDB.Email = workforceRequest.Email;
                workforceDB.AccountType = workforceRequest.AccountType;
                workforceDB.Name = workforceRequest.Name;
                workforceDB.Avatar = workforceRequest.Avatar;
                workforceDB.DisplayName = workforceRequest.DisplayName;
                workforceDB.UnitSalary = workforceRequest.UnitSalary;
                workforceDB.WorkingType = workforceRequest.WorkingType;
                if (workforceRequest.WorkingType == 1)//THIS WORKFORCE WORKS PART-TIME
                {
                    workforceDB.WorkingEffort = string.Join(",", workforceRequest.WorkingEfforts);
                }

                //insert workforce
                var workforceEntity = db.Workforces.Update(workforceDB);
                //// Fetch the existing entities from the database that you want to update
                List<WorkforceSkill> skillWorkforceDB = db.WorkforceSkills.Where(e => workforceRequest.Id == e.WorkforceId).ToList();

                ////DELETE WORKFORCE SKILL IN DATABASE WITH WORKFORCE_ID
                db.WorkforceSkills.RemoveRange(db.WorkforceSkills.Where(s => s.WorkforceId == workforceRequest.Id).ToList());

                ////INSERT WORKFORCE SKILL
                var SkillRequest = new List<WorkforceSkill>();
                foreach (var skill in workforceRequest.Skills)
                {
                    SkillRequest.Add(new WorkforceSkill
                    {
                        WorkforceId = workforceRequest.Id,
                        SkillId = skill.SkillId,
                        Level = skill.Level,
                        CreateDatetime = DateTime.Now,
                    });
                }
                db.WorkforceSkills.AddRange(SkillRequest);

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

