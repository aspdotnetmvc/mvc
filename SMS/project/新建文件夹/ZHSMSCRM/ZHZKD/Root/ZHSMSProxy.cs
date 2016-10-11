using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SMSModel;
using StatusReportInterface;
using ZHSMSPlatInterface;

namespace ZKD.Root
{
    public class ZHSMSProxy
    {
        public static ISMService GetZKD()
        {
            ISMService st = (ISMService)Activator.GetObject(typeof(ISMService), ConfigurationManager.AppSettings["Platform"]);
            return st;
        }
    }
}