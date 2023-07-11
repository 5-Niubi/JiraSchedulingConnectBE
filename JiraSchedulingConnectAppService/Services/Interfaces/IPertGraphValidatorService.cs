using System;
namespace AlgorithmServiceServer.Services.Interfaces
{
	public  interface IPertGraphValidatorService
    {
        public Task<bool> IsValidDAG(int projectId);
    }
}

