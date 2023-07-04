namespace AlgorithmServiceServer.Services.Interfaces
{
    public interface IAccessDataToComputeService
    {
        public Task<OutputToORDTO> GetDataToCompute(int projectId)
    }
}
