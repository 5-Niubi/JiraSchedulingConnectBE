namespace AlgorithmServiceServer.Services.Interfaces
{
    public interface IEstimateWorkforceService
    {

        public void PrepareDataFromDB(int projectId);
        public Task<EstimatedResultDTO> Execute(int projectId);
        public Task<WorkforceWithMilestoneDTO> ExecuteOverall(int projectId);
    }
}

