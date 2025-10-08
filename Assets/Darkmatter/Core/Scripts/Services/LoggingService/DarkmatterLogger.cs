using System;
using System.IO;
using UnityEngine;

namespace Darkmatter.Core.Services.LoggingService
{
    public class DarkmatterLogger : LoggerBase
    {
        private const string DebugTopicSuffix = ":: ";
        private const string Dot = ".";
        private const string StampFormat = "[{0}] ";
        private const string TimeStampFormat = "HH:mm:ss:ff";
        private const string DarkmatterTag = "[Darkmatter] ";
        public override void Log(string message)
        {
            Debug.Log(GetTimeStamp() + DarkmatterTag + message);
        }

        public override void LogError(string message)
        {
           Debug.LogError(GetTimeStamp() + DarkmatterTag + message);
        }

        public override void LogException(Exception exception)
        {
            Debug.LogError(GetTimeStamp() + DarkmatterTag + exception.Message);
        }

        public override void LogTopic(string message, string callerFilePath = "", string callerMemberName = "")
        {
            var callerName = GetCallerName(callerFilePath);
            var formattedMessage = $"{callerName}{Dot}{callerMemberName}{DebugTopicSuffix}{message}";
            Debug.Log(GetTimeStamp() + DarkmatterTag + formattedMessage);
        }

        public override void LogWarning(string message)
        {
            Debug.LogWarning(GetTimeStamp() + DarkmatterTag + message);
        }
        private string GetTimeStamp()
        {
            var timeStamp = DateTime.Now.ToString(TimeStampFormat);
            return string.Format(StampFormat, timeStamp);
        }

        private string GetCallerName(string callerFilePath)
        {
            return Path.GetFileNameWithoutExtension(callerFilePath);
        }
    }
}