using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace LogClient
{
    public class LogHelper
    {
        readonly static Logger log = new Logger(ConfigurationManager.AppSettings["MQVHost"], ConfigurationManager.AppSettings["MQUrl"], ConfigurationManager.AppSettings["MQName"], ConfigurationManager.AppSettings["MQPassword"], "LOG");

        public static void Log(string level, string thread, string ip, string recorder, string _event, string message)
        {
            log.Log(level, thread, ip, recorder, _event, message);
        }

        public static void LogFatal(string recorder, string _event, string message)
        {
            log.LogFatal(recorder, _event, message);
        }
        public static void LogError(string recorder, string _event, string message)
        {
            log.LogError(recorder, _event, message);
        }

        public static void LogWarn(string recorder, string _event, string message)
        {
            log.LogWarn(recorder, _event, message);
        }

        public static void LogInfo(string recorder, string _event, string message)
        {
            log.LogInfo(recorder, _event, message);
        }

        public static void LogDebug(string recorder, string _event, string message)
        {
            log.LogDebug(recorder, _event, message);
        }

    }
}