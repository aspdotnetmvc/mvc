using System;
using log4net;

namespace BXM.Logger
{
    public sealed class Log4Logger
    {
        private static readonly ILog log = LogManager.GetLogger("root");
        static Log4Logger() { /*log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));*/ }

        /// <summary>
        /// 毁灭性的错误
        /// </summary>
        /// <param name="msg"></param>
        public static void Fatal(object msg)
        {
            if (log.IsFatalEnabled)
                log.Fatal(msg);
        }
        /// <summary>
        /// 毁灭性的错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Fatal(object msg, Exception ex)
        {
            if (log.IsFatalEnabled)
                log.Fatal(msg, ex);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(object msg)
        {
            if (log.IsErrorEnabled)
                log.Error(msg);
        }
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Error(object msg, Exception ex)
        {
            if (log.IsErrorEnabled)
                log.Error(msg, ex);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="msg"></param>
        public static void Warn(object msg)
        {
            if (log.IsWarnEnabled)
                log.Warn(msg);
        }
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Warn(object msg, Exception ex)
        {
            if (log.IsErrorEnabled)
                log.Warn(msg, ex);
        }

        /// <summary>
        /// 消息
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(object msg)
        {
            if (log.IsInfoEnabled)
                log.Info(msg);
        }
        /// <summary>
        /// 消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Info(object msg, Exception ex)
        {
            if (log.IsInfoEnabled)
                log.Info(msg, ex);
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Debug(object msg)
        {
            if (log.IsDebugEnabled)
                log.Debug(msg);
        }
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Debug(object msg, Exception ex)
        {
            if (log.IsDebugEnabled)
                log.Debug(msg, ex);
        }
    }
}