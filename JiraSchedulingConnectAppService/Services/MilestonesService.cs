using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Milestones;




namespace JiraSchedulingConnectAppService.Services
{
    public class MilestonesService : IMilestonesService
    {

        public const string NotFoundMessage = "Milestone Not Found!!!";

        private readonly ModelLibrary.DBModels.JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public MilestonesService(ModelLibrary.DBModels.JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
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
                var milestone = await db.Milestones.FirstOrDefaultAsync(s => s.Id == Id && s.IsDelete == false);
                if (milestone == null)
                {
                    throw new Exception(NotFoundMessage);
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
    }
}
