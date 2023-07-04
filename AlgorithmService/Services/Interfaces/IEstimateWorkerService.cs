namespace AlgorithmServiceServer.Services.Interfaces
{
    public interface IEstimateWorkerService
    {
        public List<int> GetTaskDuration(int projectId);
        public List<List<int>> GetTaskExper(int projectId);
        public List<List<int>> GetTaskAdjacency(int projectId);
        public void PrepareDataFromDB(int projectId);
        public Task<EstimatedResultDTO> Execute(int projectId);
    }
}

