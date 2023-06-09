﻿using ModelLibrary.DTOs.Export;
using ModelLibrary.DTOs.Thread;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IExportService
    {
        public Task<ThreadStartDTO> ToJira(int scheduleId);
        public Task<MemoryStream> ToMSProject(int scheduleId);
        public Task<string> JiraRequest(dynamic dynamic);
    }
}
