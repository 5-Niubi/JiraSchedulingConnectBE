using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Skills;




namespace JiraSchedulingConnectAppService.Services
{
    public class SkillsService : ISkillsService
    {

        public const string NotFoundMessage = "Skill Not Found!!!";
        public const string NotUniqueSkillNameMessage = "Skill Name Must Unique!!!";

        private readonly ModelLibrary.DBModels.JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public SkillsService(ModelLibrary.DBModels.JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {

            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
        }



        public async Task<SkillDTOResponse> GetSkillId(int Id)
        {
            //ModelLibrary.DBModels.Skill skill = new ModelLibrary.DBModels.Skill();
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var skillResult = await db.Skills.SingleOrDefaultAsync(s => s.Id == Id && s.CloudId == cloudId && s.IsDelete == false);

                var skillDTO = mapper.Map<SkillDTOResponse>(skillResult);
                return skillDTO;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }


        public async Task<SkillDTOResponse> UpdateNameSkill(SkillDTOResponse skillDTO)
        {

            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var skill = mapper.Map<Skill>(skillDTO);

                var exitedskill = await db.Skills.FirstOrDefaultAsync(s => s.Id == skill.Id && s.CloudId == cloudId && s.IsDelete == false);
                var exitedName = await db.Skills.FirstOrDefaultAsync(s => s.Name == skillDTO.Name && s.CloudId == cloudId && s.IsDelete == false);

                // Validate exited skill
                if (skill == null)
                {
                    throw new Exception(NotFoundMessage);
                }

                // Validate unique name skill
                if (exitedName != null)
                {
                    throw new Exception(NotUniqueSkillNameMessage);
                }

                exitedskill.Name = skillDTO.Name;
                exitedskill.Description = skillDTO.Description;

                // Update
                db.Update(exitedskill);
                await db.SaveChangesAsync();



                return skillDTO;

            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }



        public async Task<SkillDTOResponse> CreateSkill(SkillCreatedRequest skillRequest)
        {

            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var exitedName = await db.Skills.FirstOrDefaultAsync(s => s.Name == skillRequest.Name && s.CloudId == cloudId && s.IsDelete == false);

                // Validate unique name skill
                if (exitedName != null)
                {
                    throw new Exception(NotUniqueSkillNameMessage);
                }

                var skill = mapper.Map<ModelLibrary.DBModels.Skill>(skillRequest);
                skill.CloudId = cloudId;

                var SkillCreatedEntity = await db.Skills.AddAsync(skill);
                await db.SaveChangesAsync();

                var skillDeatailDTO = mapper.Map<SkillDTOResponse>(SkillCreatedEntity.Entity);
                return skillDeatailDTO;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }



        async public Task<List<SkillDTOResponse>> GetSkills(string? skillName)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            skillName = skillName ?? string.Empty;

            var query = db.Skills.Where(t => t.CloudId == cloudId
            && (t.Name.Contains(skillName) || t.Name.Equals(string.Empty)))
            .OrderByDescending(e => e.Id);

            var skillsResult = await query.ToListAsync();

            var skillsDTO = mapper.Map<List<SkillDTOResponse>>(skillsResult);

            return skillsDTO;




        }

        async public Task<SkillDTOResponse> GetSkillName(string? skillName)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var result = await db.Skills.SingleOrDefaultAsync(s => s.Name.ToLower() == skillName.ToLower() && s.CloudId == cloudId && s.IsDelete == false);

            var skillDTO = mapper.Map<SkillDTOResponse>(result);

            return skillDTO;
        }

        public async Task<bool> DeleteSkill(int Id)
        {

            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                // validate skill
                var skill = await db.Skills.FirstOrDefaultAsync(s => s.Id == Id && s.CloudId == cloudId && s.IsDelete == false);
                if (skill == null)
                {
                    throw new Exception(NotFoundMessage);
                }

                // delete skill in skill required
                var requiredSkillsToRemove = await db.TasksSkillsRequireds.Where(s => s.SkillId == Id).ToArrayAsync();

                db.RemoveRange(requiredSkillsToRemove);
                await db.SaveChangesAsync();


                // Update status isdelete
                skill.IsDelete = true;
                db.Update(skill);
                await db.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }



        }
    }
}
