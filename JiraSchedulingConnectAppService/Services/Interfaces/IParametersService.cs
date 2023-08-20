using ModelLibrary.DTOs.Parameters;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IParametersService
    {
        public Task<ParameterDTO> SaveParams(ParameterRequestDTO paramsRequest);
        public Task<List<WorkforceViewDTOResponse>> GetWorkforceParameter(string project_id);
    }
}

