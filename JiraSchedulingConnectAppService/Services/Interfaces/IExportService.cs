using ModelLibrary.DTOs.Thread;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IExportService
    {
        public Task<ThreadStartDTO> ToJira(int scheduleId);
        public Task<(string, MemoryStream)> ToMSProject(int scheduleId, string? token);
        public Task<string> JiraRequest(dynamic dynamic);
    }
}
