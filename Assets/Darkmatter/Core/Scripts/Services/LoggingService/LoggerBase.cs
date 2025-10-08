using System;
using Darkmatter.Core.Services.LoggingService.Interfaces;

namespace Darkmatter.Core.Services.LoggingService
{
    public abstract class LoggerBase : ILogger
    {
        protected LoggerBase()
        {

        }
        public abstract void Log(string message);
        public abstract void LogWarning(string message);
        public abstract void LogError(string message);
        public abstract void LogException(Exception exception);
        public abstract void LogTopic(string message, string callerFilePath = "", string callerMemberName = "");
    }
}