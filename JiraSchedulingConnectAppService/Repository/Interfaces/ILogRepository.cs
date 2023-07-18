using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Repository.Interfaces
{
    public interface ILogRepository
    {
        public Task Insert(List<LogMessage> logMessages);

    }
}

