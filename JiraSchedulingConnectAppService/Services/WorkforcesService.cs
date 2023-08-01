using AutoMapper;
using UtilsLibrary;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Invalidation;
using ModelLibrary.DTOs.Parameters;
using Newtonsoft.Json;
using UtilsLibrary.Exceptions;
using jdk.nashorn.tools;
using ModelLibrary.DTOs.Skills;
using org.apache.commons.math3.geometry.spherical.oned;
using static ModelLibrary.DTOs.Export.JiraAPICreateBulkTaskResDTO;
using com.sun.org.apache.bcel.@internal.generic;
using System.Linq;

namespace JiraSchedulingConnectAppService.Services
{
    public class WorkforcesService : IWorkforcesService
    {


        public const string WorkforceTypeNotValidMessage = "Workforce Type Is Not Validated!!!";

        public const string EffortNotValidMessage = "Effort Is Not Validated!!!";
        public const string EffortElementNotValidMessage = "Effort must only have 7 elements!!!";
        public const string SkillNotFoundVaMessage = "Skill Workforce Is Not Found!!!";
        public const string SkillLevelNotValidateMessage = "Skill Level Workforce is not validate!!!";
        public const string NotUniqueSkillNameMessage = "Skill Name Must Unique!!!";

        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;


        public WorkforcesService(ModelLibrary.DBModels.JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {

            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
        }
        public async Task<List<WorkforceDTOResponse>> GetAllWorkforces(List<int>? Ids)
        {
           
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var query = (Ids == null) ?
                await db.Workforces.Where(s => s.CloudId == cloudId).Include(s => s.WorkforceSkills).ThenInclude(s => s.Skill).ToListAsync() : await db.Workforces.Where(
                W => Ids.Contains(W.Id) == true).ToListAsync();

            var queryDTOResponse = mapper.Map<List<WorkforceDTOResponse>>(query);
            return queryDTOResponse;
           
            
        }

        public async Task<List<WorkforceViewDTOResponse>> GetWorkforceScheduleByProject()
        {
           
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var results = await db.Workforces
            .Where(s => s.CloudId == cloudId)
            .Select(s => new WorkforceViewDTOResponse(){
                Id = s.Id,
                Name = s.Name }) // Projection into an anonymous type
            .ToListAsync();

            return results;
           
        }


        private async Task<bool> _ValidateWorkforceSkills(WorkforceRequestDTO WorkforceRequest)
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();
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

            if (SkillErrors.Count > 0)
            {
                throw new NotSuitableInputException(
                        SkillErrors
                    );
            }


            return true;

        }

        private async Task<bool> _ValidateWorkforceEfforts(WorkforceRequestDTO WorkforceRequest)
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
                if (WorkingEfforts[i] < 0 || WorkingEfforts[i] > 8)
                {
                    EffortErrors.Add(new WorkingEffortErrorDTO
                    {
                        DayIndex = i,
                        Effort = WorkingEfforts[i],
                        Message = EffortNotValidMessage
                    });
                }

            }

            if (EffortErrors.Count != 0 )
            {
                throw new NotSuitableInputException(
                    EffortErrors
                    );
            }


            return true;
        }

        private async Task<bool> _ValidateNewSkill(WorkforceRequestDTO WorkforceRequest)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();
            var SkillErrors = new List<SkillRequestErrorDTO>();

            var newSkills = WorkforceRequest.NewSkills;

            foreach (var newSkill in newSkills)
            {
                var exitedName = await db.Skills.FirstOrDefaultAsync(s => s.Name == newSkill.Name && s.CloudId == cloudId && s.IsDelete == false);

                // Validate unique name skill
                if (exitedName != null)
                {
                    SkillErrors.Add(
                        new SkillRequestErrorDTO
                        {
                            Messages = $"Skill name '{newSkill.Name}' is duplicated."
                        });
                }
            }

            if (SkillErrors.Count != 0)
            {
                throw new NotSuitableInputException(
                    new WorkforceInputErrorDTO()
                    {
                        SkillErrors = SkillErrors,
                    }
                    );
            }

            return true;
        }

        public async Task<WorkforceDTOResponse> CreateWorkforce(WorkforceRequestDTO? workforceRequest)
        {

 
            //validate new skills
            await _ValidateCreatedWorkforceProperties(workforceRequest);

            //validate new skills
            await _ValidateNewSkill(workforceRequest);

            //validate efforts
            await _ValidateWorkforceEfforts(workforceRequest);
            // validate skills
            await _ValidateWorkforceSkills(workforceRequest);

         
            // Insert new skill to database
            var newSkills = await _insertSkills(workforceRequest.NewSkills);

            // mapping added new skill -> input skill workforce into workforce input
            foreach(var skill in newSkills)
            {
                workforceRequest.Skills.Add(new SkillRequestDTO
                {
                    SkillId = skill.Id,
                    Level = skill.Level
                });
            }


            var newWorkforce = mapper.Map<Workforce>(workforceRequest);
            newWorkforce.Active = 1;




            var insertedNewWorkforce = db.Workforces.Add(newWorkforce);
            await db.SaveChangesAsync();

            var workforceDTOResponse = mapper.Map<WorkforceDTOResponse>(insertedNewWorkforce.Entity);

            return workforceDTOResponse;
        }

        private async System.Threading.Tasks.Task _ValidateCreatedWorkforceProperties(WorkforceRequestDTO workforceRequest)
        {
            // email not exited
            var existingWorkforceWithEmail = await db.Workforces.FirstOrDefaultAsync(w => w.Email == workforceRequest.Email);
            if (existingWorkforceWithEmail != null)
            {
                throw new DuplicateException($"Email '{workforceRequest.Email}' is already in use.");
            }

            // working type in [0 or 1]
            var ValidatedWorkingTypes = new List<int> { 0, 1 };

            if (!ValidatedWorkingTypes.Contains((int) workforceRequest.WorkingType))
            {
                throw new NotSuitableInputException(
                    new WorkforceInputErrorDTO()
                    {
                        Messages = WorkforceTypeNotValidMessage
                    }
                    ) ;


            }
            
        }


        private async System.Threading.Tasks.Task _ValidateUpdateddWorkforceProperties(WorkforceRequestDTO workforceRequest)
        {
            // email not exited
            var existingWorkforceWithEmail = await db.Workforces.FirstOrDefaultAsync(w => w.Email == workforceRequest.Email && w.Id != workforceRequest.Id);
            if (existingWorkforceWithEmail != null)
            {
                throw new DuplicateException($"Email '{workforceRequest.Email}' is already in use.");
            }

            // working type in [0 or 1]
            var ValidatedWorkingTypes = new List<int> { 0, 1 };
            
            if (workforceRequest.WorkingType != null &&  !ValidatedWorkingTypes.Contains((int)workforceRequest.WorkingType))
            {
                throw new NotSuitableInputException(
                    new WorkforceInputErrorDTO()
                    {
                        Messages = WorkforceTypeNotValidMessage
                    }
                    );


            }

        }


        private async Task<List<NewSkillResponeDTO>> _insertSkills(List<NewSkillRequestDTO> SkillRequests)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            // mapping
            var newSkills = mapper.Map<List<Skill>>(SkillRequests);

            for (var i = 0; i < SkillRequests.Count; i++)
            {
                newSkills[i].CloudId = cloudId;
                newSkills[i].IsDelete = false;


            }

            await db.Skills.AddRangeAsync(newSkills);
            await db.SaveChangesAsync();


            var reponse = mapper.Map <List<NewSkillResponeDTO>>(newSkills);
            return reponse;

        }

        public async Task<WorkforceDTOResponse> GetWorkforceById(string workforce_id)
        {
            try
            {
                var workforce = await db.Workforces.Include(s=>s.WorkforceSkills).ThenInclude(s=>s.Skill).Where(e => e.Id.ToString() == workforce_id).FirstOrDefaultAsync();
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
            
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            // validate exited workforce
            var workforceDB = db.Workforces.Include(s => s.WorkforceSkills).FirstOrDefault(s => s.Id == workforceRequest.Id) ??
                throw new NotFoundException($"Can not find project :{workforceRequest.Id}");



            await _ValidateUpdateddWorkforceProperties(workforceRequest);


            //validate new skills
            await _ValidateNewSkill(workforceRequest);

            //validate efforts
            await _ValidateWorkforceEfforts(workforceRequest);

            // validate skills
            await _ValidateWorkforceSkills(workforceRequest);


            // Insert new skill to database
            var newSkills = await _insertSkills(workforceRequest.NewSkills);
            // mapping added new skill -> input skill workforce into workforce input
            foreach (var skill in newSkills)
            {
                workforceRequest.Skills.Add(new SkillRequestDTO
                {
                    SkillId = skill.Id,
                    Level = skill.Level
                });
            }


            // clear skill workforce
            await _ClearSkillsWorkforce(workforceDB.Id);

            // insert new skill workforce
            //await _insertSkills(workforceRequest.Skills);


            workforceDB.WorkingEffort = "[" + string.Join(", ", workforceRequest.WorkingEfforts) + "]";
            workforceDB.Name = workforceRequest.Name ?? workforceDB.Name;
            workforceDB.Email = workforceRequest.Email ?? workforceDB.Email;
            workforceDB.UnitSalary = workforceRequest.UnitSalary ?? workforceDB.UnitSalary;
            workforceDB.WorkingType = workforceRequest.WorkingType ?? workforceDB.WorkingType;
            workforceDB.DisplayName = workforceRequest.DisplayName ?? workforceDB.DisplayName;
            workforceDB.Avatar = workforceRequest.Avatar ?? workforceDB.Avatar;

            workforceDB.WorkforceSkills = mapper.Map<List<WorkforceSkill>>(workforceRequest.Skills);
            // Update 
            var entity  =  db.Workforces.Update(workforceDB);
            await db.SaveChangesAsync();

            // mapping reponse
            var workforceDTOResponse = mapper.Map<WorkforceDTOResponse>(entity.Entity);
            return workforceDTOResponse;
           
        }

        private async System.Threading.Tasks.Task _ClearSkillsWorkforce(int workforceId)
        {

            var skillsWorkforce = db.WorkforceSkills.Where(swf => swf.WorkforceId == workforceId);

            db.WorkforceSkills.RemoveRange(skillsWorkforce);
            await db.SaveChangesAsync();
        }
    }

}

