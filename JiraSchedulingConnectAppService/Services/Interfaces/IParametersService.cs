﻿using System;
using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.PertSchedule;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IParametersService
	{
        public Task<ParameterDTO> SaveParams(ParameterRequestDTO paramsRequest);
        public Task<List<WorkforceDTOResponse>> GetWorkforceParameter(string project_id);
    }
}

