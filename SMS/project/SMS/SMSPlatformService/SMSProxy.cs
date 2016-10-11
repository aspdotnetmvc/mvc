using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SMS.Model;
using SMSInterface;

namespace SMSPlatform
{
    public class SMSProxy
    {
        public static ISMSService GetSMSService()
        {
            ISMSService sms = (ISMSService)Activator.GetObject(typeof(ISMSService), ConfigurationManager.AppSettings["SMSService"]);
            return sms;
        }
    }
}