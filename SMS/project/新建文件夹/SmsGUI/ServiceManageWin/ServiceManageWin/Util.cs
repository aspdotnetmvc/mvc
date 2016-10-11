using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ServiceManageWin
{
    public static class Util
    {
        public static string GetAppSetting(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
        public static string LogDir = string.IsNullOrWhiteSpace(GetAppSetting("LogDir")) ? AppDomain.CurrentDomain.BaseDirectory : GetAppSetting("LogDir");

        /// <summary> 
        /// 使用指定路径中的指定文件作为配置文件         /// </summary> 
        /// <param name="configFileName">配置文件名</param> 
        /// <param name="configFileDirectory">配置文件路径</param> 
        public static void SetServiceConfigFile(string configFileFullName)
        {
            System.AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configFileFullName);
        }

        public static ILog GetLogger(IService.ServiceProp serviceProp)
        {
            LogHelper.SetLevel(serviceProp.Code, serviceProp.LogLevel);
            LogHelper.AddAppender(serviceProp.Code,
                LogHelper.CreateFileAppender(serviceProp.Code + "Appender", System.IO.Path.Combine(Util.LogDir, serviceProp.Directory, serviceProp.Code + ".log")));

            return LogManager.GetLogger(serviceProp.Code);
        }
        public static ILog GetLogger(string log)
        {
            LogHelper.SetLevel(log, GetAppSetting("LogLevel"));
            LogHelper.AddAppender(log,
                LogHelper.CreateFileAppender(log, System.IO.Path.Combine(Util.LogDir, "console/console.log")));

            return LogManager.GetLogger(log);
        }
        public static class LogHelper
        {
            // Set the level for a named logger
            public static void SetLevel(string loggerName, string levelName)
            {
                ILog log = LogManager.GetLogger(loggerName);
                var l = (Logger)log.Logger;

                l.Level = l.Hierarchy.LevelMap[levelName];
            }

            // Add an appender to a logger
            public static void AddAppender(string loggerName, IAppender appender)
            {
                ILog log = LogManager.GetLogger(loggerName);
                var l = (Logger)log.Logger;

                l.AddAppender(appender);

            }

            // Create a new file appender
            public static IAppender CreateFileAppender(string name, string fileName)
            {
                var appender = new RollingFileAppender
                      {
                          Name = name,
                          File = fileName,
                          AppendToFile = true,
                          RollingStyle = RollingFileAppender.RollingMode.Date,
                          DatePattern = "yyyyMMdd",
                          StaticLogFileName = false,
                          ImmediateFlush = true,
                          LockingModel = new FileAppender.MinimalLock()
                      };

                var layout = new PatternLayout();//{ ConversionPattern = "%date [%thread] %level %logger - %message%newline" };
                layout.ConversionPattern = "时间：%date 日志级别：%-5level 类：%logger - 描述：%message%newline";
                layout.ActivateOptions();

                appender.Layout = layout;
                appender.ActivateOptions();


                return appender;
            }
        }
    }
}
