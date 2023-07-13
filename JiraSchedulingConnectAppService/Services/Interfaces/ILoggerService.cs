namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface ILoggerService
    {
        void Log(LogLevel logLevel, Exception exception);
    }
}

