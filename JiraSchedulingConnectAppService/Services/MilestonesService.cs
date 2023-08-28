using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DTOs.Milestones;




namespace JiraSchedulingConnectAppService.Services
{
    public class MilestonesService : IMilestonesService
    {

        public const string NotFoundMessage = "Group Not Found: The specified group could not be located.";
        public const string NotValidateDeleteMilestone = "Tasks Exist in Group: Unable to delete this group as there are tasks associated with it.";

        private readonly ModelLibrary.DBModels.WoTaasContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public MilestonesService(ModelLibrary.DBModels.WoTaasContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {

            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<MilestoneDTO> GetMilestoneId(int Id)
        {
            try
            {
                var milestoneResult = await db.Milestones.SingleOrDefaultAsync(s => s.Id == Id && s.IsDelete == false);

                var milestoneDTO = mapper.Map<MilestoneDTO>(milestoneResult);
                return milestoneDTO;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public async Task<MilestoneDTO> CreateMilestone(MilestoneCreatedRequest milestoneRequest)
        {

            try
            {
                var milestone = mapper.Map<ModelLibrary.DBModels.Milestone>(milestoneRequest);


                // validate miletone name
                await _ValidateMileStoneName(-100, milestone.Name, (int)milestone.ProjectId);

                var MilestoneCreatedEntity = await db.Milestones.AddAsync(milestone);
                await db.SaveChangesAsync();

                var milestoneDetailDTO = mapper.Map<MilestoneDTO>(MilestoneCreatedEntity.Entity);
                return milestoneDetailDTO;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }



        async public Task<List<MilestoneDTO>> GetMilestones(int projectId)
        {
            var query = db.Milestones.Where(t => t.ProjectId == projectId)
            .OrderByDescending(e => e.Id);

            var milestonesResult = await query.ToListAsync();

            var milestonesDTO = mapper.Map<List<MilestoneDTO>>(milestonesResult);

            return milestonesDTO;

        }

        public async Task<bool> DeleteMilestone(int Id)
        {
            try
            {
                // validate milestone
                var milestone = await db.Milestones.Include(m => m.Tasks).FirstOrDefaultAsync(s => s.Id == Id && s.IsDelete == false);
                if (milestone == null)
                {
                    throw new Exception(NotFoundMessage);
                }

                else if (milestone.Tasks.Any(t => t.IsDelete == false))
                {
                    throw new Exception(NotValidateDeleteMilestone);
                }


                // Update status isdelete
                milestone.IsDelete = true;
                db.Update(milestone);
                await db.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }



        }


        public async Task<MilestoneDTO> UpdateMilestone(MilestoneDTO milestone)
        {
            try
            {
                // validate milestone
                var Exitedmilestone = await db.Milestones.FirstOrDefaultAsync(s => s.Id == milestone.Id && s.IsDelete == false);
                if (milestone == null)
                {
                    throw new Exception(NotFoundMessage);
                }



                // validate miletone name
                await _ValidateMileStoneName(milestone.Id, milestone.Name, (int)milestone.ProjectId);

                // Update status isdelete
                Exitedmilestone.Name = milestone.Name;
                var milestoneEntity = db.Update(Exitedmilestone);
                await db.SaveChangesAsync();

                var milestoneRes = mapper.Map<MilestoneDTO>(milestoneEntity.Entity);
                return milestoneRes;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }



        }


        private async Task<bool> _ValidateMileStoneName(int miletoneid, string milestoneName, int projectId)
        {

            if (milestoneName == null || milestoneName.Trim() == "")
            {
                throw new Exception($"Milestonne with name  must inlude characters. Not null or empty");
            }

            else if (milestoneName != null && milestoneName.Trim() != "")
            {
                // validate not exited milestone name
                var exitedName = await db.Milestones.FirstOrDefaultAsync(s => s.ProjectId == projectId
                && s.Name == milestoneName.Trim()
                && s.Id != miletoneid
                && s.IsDelete == false);

                if (exitedName != null)
                {
                    throw new Exception($"Milestonne with name {exitedName.Name} exited in this project");
                }

            }

            return true;
        }


    }


}
