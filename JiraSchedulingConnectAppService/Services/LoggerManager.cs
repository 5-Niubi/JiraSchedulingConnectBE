using JiraSchedulingConnectAppService.Services.Interfaces;
using NLog;
using NLog.Web;

namespace JiraSchedulingConnectAppService.Services
{
    public class LoggerManager : ILoggerManager
    {
        private static NLog.ILogger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();


        public void LogDebug(string message)
        {
            logger.Debug(message);
        }
        public void LogError(string message)
        {
            logger.Error(message);
        }
        public void LogInfo(string message)
        {
            logger.Info(message);
        }
        public void LogWarning(string message)
        {
            logger.Warn(message);
        }
    }
}

