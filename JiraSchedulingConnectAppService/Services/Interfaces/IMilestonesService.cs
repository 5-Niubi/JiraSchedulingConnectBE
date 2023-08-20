using ModelLibrary.DTOs.Milestones;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IMilestonesService
    {
        public Task<List<MilestoneDTO>> GetMilestones(int projectId);


        public Task<MilestoneDTO> GetMilestoneId(int Id);

        public Task<bool> DeleteMilestone(int Id);

        public Task<MilestoneDTO> CreateMilestone(MilestoneCreatedRequest milestoneRequest);
        public Task<MilestoneDTO> UpdateMilestone(MilestoneDTO milestoneDTO);
    }
}
