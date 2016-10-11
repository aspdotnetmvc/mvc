using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSService
{
    public class AppConfig
    {
        public static string MQHost = System.Configuration.ConfigurationManager.AppSettings["MQHost"];
        public static string MQVHost = System.Configuration.ConfigurationManager.AppSettings["MQVHost"];
        public static string MQUserName = System.Configuration.ConfigurationManager.AppSettings["MQUserName"];
        public static string MQPassword = System.Configuration.ConfigurationManager.AppSettings["MQPassword"];
        public static int MaxPriority = 10;

    }
}
