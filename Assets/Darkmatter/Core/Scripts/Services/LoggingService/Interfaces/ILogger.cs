using System;

namespace Darkmatter.Core.Services.LoggingService.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogException(Exception exception);
        void LogTopic(string message, string callerFilePath = "", string callerMemberName = "");
    }
}