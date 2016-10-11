using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using SMSPlatformInterface;

namespace DAL
{
    public class ZHSMSProxy
    {

        public static SMSPlatformInterface GetZKD()
        {
            SMSPlatformInterface st = (SMSPlatformInterface)Activator.GetObject(typeof(SMSPlatformInterface), ConfigurationManager.AppSettings["Platform"]);
            return st;
        }

    }
}
