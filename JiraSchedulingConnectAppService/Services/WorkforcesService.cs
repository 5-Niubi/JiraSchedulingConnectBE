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

namespace JiraSchedulingConnectAppService.Services
{
    public class WorkforcesService : IWorkforcesService
    {

        public const string WorkforceTyNotVaMessage = "Workforce Typalidatee Is Not Validated!!!";
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


        private async Task<List<SkillRequestErrorDTO>> _ValidateWorkforceSkills(WorkforceRequestDTO WorkforceRequest)
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


            return EffortErrors;
        }

        private async Task<List<SkillRequestErrorDTO>> ValidatesSkillUniqueName(List<NewSkillDTORequest> newSkills)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();
            var SkillErrors = new List<SkillRequestErrorDTO>();
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
            return SkillErrors;
        }

        public async Task<WorkforceDTOResponse> CreateWorkforce(WorkforceRequestDTO? workforceRequest)
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            // TODO: validate email
            // TODO: validate working type
            //validate efforts
            var EffortErrors = await _ValidateWorkforceEfforts(workforceRequest);
            // validate skills
            var SkillErrors = await _ValidateWorkforceSkills(workforceRequest);
            //validate new skills
            var newSkillErrors = await ValidatesSkillUniqueName(workforceRequest.NewSkills);
            if (EffortErrors.Count != 0 || SkillErrors.Count != 0 || newSkillErrors.Count !=0)
            {
                throw new NotSuitableInputException(
                    new WorkforceInputErrorDTO()
                    {
                        Skills = SkillErrors,
                        Efforts = EffortErrors,
                        newSkills = newSkillErrors
                    }
                    );
            }

            //DEFAULT NEW WORKFORCE ID = 0:
            workforceRequest.Id = 0;

            //CHECK DUPILATE EMAIL
            var existingWorkforceWithEmail = await db.Workforces.FirstOrDefaultAsync(w => w.Email == workforceRequest.Email);
            if (existingWorkforceWithEmail != null)
            {
                throw new DuplicateException($"Email '{workforceRequest.Email}' is already in use.");
            }



            //MAPPING PROPERTIES VALUES TO NEW VARIABLE
            var newWorkforce = new Workforce() { };
            newWorkforce.AccountId = workforceRequest.AccountId;
            newWorkforce.Email = workforceRequest.Email;
            newWorkforce.AccountType = workforceRequest.AccountType;
            newWorkforce.Active = 1;
            newWorkforce.CloudId = cloudId;
            newWorkforce.Name = workforceRequest.Name;
            newWorkforce.Avatar = workforceRequest.Avatar;
            newWorkforce.DisplayName = workforceRequest.DisplayName;
            newWorkforce.UnitSalary = workforceRequest.UnitSalary;
            newWorkforce.WorkingType = workforceRequest.WorkingType;
            if (workforceRequest.WorkingType == 1)//THIS WORKFORCE WORKS PART-TIME
            {
                //CALCULATE WORKING EFFORT (HOUR) TO FLOAT - DEFAULT WORKING HOURS PER DAYS IS 8 HOURS (ROUND NUMBER TO 2 DECIMAL PLACES)
                if (workforceRequest.WorkingEfforts != null)
                {
                    for (int i = 0; i < workforceRequest.WorkingEfforts.Count(); i++)
                    {
                        workforceRequest.WorkingEfforts[i] = (float)Math.Round((workforceRequest.WorkingEfforts[i] / 8), 2);
                    }
                }
                //CONVERT ARRAY TO STRING JSON
                string jsonString = JsonConvert.SerializeObject(workforceRequest.WorkingEfforts);
                newWorkforce.WorkingEffort = jsonString;
            }
            else if (workforceRequest.WorkingType == 0) //THIS WORKFORCE WORKS FULL-TIME
            {
                newWorkforce.WorkingEffort = "[1, 1, 1, 1, 1, 1, 1]";
            }
            if (workforceRequest.Skills != null)
            {
                var skillWorkforceInsert = new List<WorkforceSkill>();
                foreach (var item in workforceRequest.Skills)
                {
                    skillWorkforceInsert.Add(new WorkforceSkill
                    {
                        WorkforceId = workforceRequest.Id,
                        SkillId = item.SkillId,
                        Level = item.Level,
                        CreateDatetime = DateTime.Now,
                    });
                }
                //CHANGE WORKFORCESKILLS OF WORKFORCE
                newWorkforce.WorkforceSkills = skillWorkforceInsert;
            }

            //ADD NEW WORKFORCE INTO DB
            var workforceEntity = await db.Workforces.AddAsync(newWorkforce);
            await db.SaveChangesAsync();

            //INSERT NEWS SKILLS WITH PROPERTY NEWSKILLDTOREQUEST(Name, Level)
            var newSkillsInsert = new List<Skill>();
            if (workforceRequest.NewSkills != null)
            {
                foreach (var newSkill in workforceRequest.NewSkills)
                {
                    //INSERT TO DB
                    newSkillsInsert.Add(new Skill
                    {
                        CloudId = cloudId,
                        Name = newSkill.Name,
                        CreateDatetime = DateTime.Now,
                    });
                }
            }
            await db.Skills.AddRangeAsync(newSkillsInsert);
            await db.SaveChangesAsync();

            //USING DICTIONARY TO MAP NAME TO SKILLID
            var insertedSkills = await db.Skills.ToListAsync();
            var skillNameToIdMap = new Dictionary<string, int>();
            var skillNamesSet = new HashSet<string>();

            foreach (var skill in insertedSkills)
            {

                if (!skillNamesSet.Contains(skill.Name))
                {
                    skillNameToIdMap[skill.Name] = skill.Id;
                    skillNamesSet.Add(skill.Name);
                }
            }

            //INSERT NEW WORKFORCE SKILLS
            if (insertedSkills.Any())
            {
                var newWorkforceSkills = new List<WorkforceSkill>();
                if (workforceRequest.NewSkills != null)
                {
                    foreach (var newSkill in workforceRequest.NewSkills)
                    {
                        if (skillNameToIdMap.TryGetValue(newSkill.Name, out var skillId))
                        {

                            newWorkforceSkills.Add(new WorkforceSkill
                            {
                                WorkforceId = workforceEntity.Entity.Id, //GET NEW CREATED WORKFORK ID
                                SkillId = skillId,
                                Level = newSkill.Level,
                                CreateDatetime = DateTime.Now,
                            });
                        }
                    }
                }
                await db.WorkforceSkills.AddRangeAsync(newWorkforceSkills);
                await db.SaveChangesAsync();
            }

            var workforceDTOResponse = mapper.Map<WorkforceDTOResponse>(newWorkforce);
            return workforceDTOResponse;
        }

        public async Task<WorkforceDTOResponse> GetWorkforceById(string workforce_id)
        {
            try
            {
                var workforce = await db.Workforces.Include(s=>s.WorkforceSkills).ThenInclude(s=>s.Skill).Where(e => e.Id.ToString() == workforce_id).FirstOrDefaultAsync();
                var workforceResponse = mapper.Map<WorkforceDTOResponse>(workforce);
                //CONVERT TO WORKING HOURS PER DAYS (ROUND NUMBER TO 2 DECIMAL PLACES)
                if (workforceResponse.WorkingEfforts != null)
                {
                    for (int i = 0; i < workforceResponse.WorkingEfforts.Count(); i++)
                    {
                        workforceResponse.WorkingEfforts[i] = (float)Math.Round((workforceResponse.WorkingEfforts[i] * 8), 2);
                    }
                }
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

                var workforceDB = db.Workforces.Include(s => s.WorkforceSkills).FirstOrDefault(s => s.Id == workforceRequest.Id) ??
                    throw new NotFoundException($"Can not find project :{workforceRequest.Id}");

                //CHECK DUPILATE EMAIL
                var existingWorkforceWithEmail = await db.Workforces.FirstOrDefaultAsync(w => w.Email == workforceRequest.Email);
                if (existingWorkforceWithEmail != null && existingWorkforceWithEmail.Id != workforceRequest.Id)
                {
                    throw new DuplicateException($"Email '{workforceRequest.Email}' is already in use.");
                }

                //CHECKING NAME NEW SKILLS
                ValidatesSkillUniqueName(workforceRequest.NewSkills);

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
                    //CALCULATE WORKING EFFORT (HOUR) TO FLOAT - DEFAULT WORKING HOURS PER DAYS IS 8 HOURS (ROUND NUMBER TO 2 DECIMAL PLACES)
                    if (workforceRequest.WorkingEfforts != null)
                    {
                        for (int i = 0; i < workforceRequest.WorkingEfforts.Count(); i++)
                        {
                            workforceRequest.WorkingEfforts[i] = (float)Math.Round((workforceRequest.WorkingEfforts[i] / 8), 2);
                        }
                    }
                    //CONVERT ARRAY TO STRING JSON
                    string jsonString = JsonConvert.SerializeObject(workforceRequest.WorkingEfforts);
                    workforceDB.WorkingEffort = jsonString;
                }
                else if (workforceRequest.WorkingType == 0) //THIS WORKFORCE WORKS FULL-TIME
                {
                    workforceDB.WorkingEffort = "[1, 1, 1, 1, 1, 1, 1]";
                }
                if (workforceRequest.Skills != null)
                {
                    var skillWorkforceInsert = new List<WorkforceSkill>();
                    foreach (var item in workforceRequest.Skills)
                    {
                        skillWorkforceInsert.Add(new WorkforceSkill
                        {
                            WorkforceId = workforceRequest.Id,
                            SkillId = item.SkillId,
                            Level = item.Level,
                            CreateDatetime = DateTime.Now,
                        });
                    }
                    //CHANGE WORKFORCESKILLS OF WORKFORCE
                    workforceDB.WorkforceSkills = skillWorkforceInsert;
                }

                //UPDATE WORKFORCE INFO INTO WORKFORCES DB
                db.Workforces.Update(workforceDB);
                await db.SaveChangesAsync();

                //INSERT NEWS SKILLS WITH PROPERTY NEWSKILLDTOREQUEST(Name, Level)
                var newSkillsInsert = new List<Skill>();
                if (workforceRequest.NewSkills != null)
                {
                    foreach (var newSkill in workforceRequest.NewSkills)
                    {
                        //CHECKING NAME SKILL DB
                        var exitedName = await db.Skills.FirstOrDefaultAsync(s => s.Name == newSkill.Name && s.CloudId == cloudId && s.IsDelete == false);
                        // Validate unique name skill
                        if (exitedName != null)
                        {
                            throw new Exception(NotUniqueSkillNameMessage);
                        }
                        newSkillsInsert.Add(new Skill
                        {
                            CloudId = cloudId,
                            Name = newSkill.Name,
                            CreateDatetime = DateTime.Now,
                        });
                    }
                }
                await db.Skills.AddRangeAsync(newSkillsInsert);
                await db.SaveChangesAsync();

                //USING DICTIONARY TO MAP NAME TO SKILLID
                var insertedSkills = await db.Skills.ToListAsync();
                var skillNameToIdMap = new Dictionary<string, int>();
                var skillNamesSet = new HashSet<string>();

                foreach (var skill in insertedSkills)
                {
                    if (!skillNamesSet.Contains(skill.Name))
                    {
                        skillNameToIdMap[skill.Name] = skill.Id;
                        skillNamesSet.Add(skill.Name);
                    }
                }

                //INSERT NEW WORKFORCE SKILLS
                if (insertedSkills.Any())
                {
                    var newWorkforceSkills = new List<WorkforceSkill>();
                    if (workforceRequest.NewSkills != null)
                    {
                        foreach (var newSkill in workforceRequest.NewSkills)
                        {
                            if (skillNameToIdMap.TryGetValue(newSkill.Name, out var skillId))
                            {
                                newWorkforceSkills.Add(new WorkforceSkill
                                {
                                    WorkforceId = workforceRequest.Id,
                                    SkillId = skillId,
                                    Level = newSkill.Level,
                                    CreateDatetime = DateTime.Now,
                                });
                            }
                        }
                    }
                    await db.WorkforceSkills.AddRangeAsync(newWorkforceSkills);
                    await db.SaveChangesAsync();
                }

                var workforceDTOResponse = mapper.Map<WorkforceDTOResponse>(workforceDB);
                return workforceDTOResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

}

