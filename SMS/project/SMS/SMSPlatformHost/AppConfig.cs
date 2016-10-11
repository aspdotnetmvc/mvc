using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ZHSMSPlatHost
{
    public class AppConfig
    {
        public static string MQHost = System.Configuration.ConfigurationManager.AppSettings["MQHost"];
        public static string MQVHost = System.Configuration.ConfigurationManager.AppSettings["MQVHost"];
        public static string MQUserName = System.Configuration.ConfigurationManager.AppSettings["MQUserName"];
        public static string MQPassword = System.Configuration.ConfigurationManager.AppSettings["MQPassword"];
        public static string MQChannel = ConfigurationManager.AppSettings["MQChannel"];

        public static int MaxPriority = 10;
    }
}
