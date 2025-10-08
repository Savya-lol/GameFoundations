using System;
using System.Runtime.CompilerServices;
using Darkmatter.Core.Services.LoggingService.Interfaces;

namespace Darkmatter.Core.Services.LoggingService
{
    public static class LogService
    {
        private static ILogger _logger;
        internal static void InjectLogger(ILogger logger)
        {
            _logger = logger;
        }
        public static void Log(string message)
        {
            _logger?.Log(message);
        }
        public static void LogWarning(string message)
        {
            _logger?.LogWarning(message);
        }
        public static void LogError(string message)
        {
            _logger?.LogError(message);
        }
        public static void LogException(Exception exception)
        {
            _logger?.LogException(exception);
        }
        public static void LogTopic(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
        {
            _logger?.LogTopic(message, callerFilePath, callerMemberName);
        }
    }
}