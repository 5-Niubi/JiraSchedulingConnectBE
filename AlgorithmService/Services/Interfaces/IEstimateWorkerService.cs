using System;
using ModelLibrary.DTOs.AlgorithmController;

namespace AlgorithmServiceServer.Services.Interfaces
{
	public interface IEstimateWorkerService
	{
        
        public Task<EstimatedResultDTO> Execute(int projectId);
        
    }
}

