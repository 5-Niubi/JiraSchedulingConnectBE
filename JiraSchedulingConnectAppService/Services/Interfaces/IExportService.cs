namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IExportService
    {
        public Task<string> ToJira(int scheduleId, string projectJiraId);
        public Task<MemoryStream> ToMSProject(int scheduleId);
    }
}
