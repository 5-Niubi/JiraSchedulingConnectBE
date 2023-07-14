using ModelLibrary.DTOs.Export;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IExportService
    {
        public Task<string> ToJira(int scheduleId);
        public Task<MemoryStream> ToMSProject(int scheduleId);
        public Task<string> JiraRequest(dynamic dynamic);
    }
}
