using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SMS.Model;

namespace SMSPlatService
{
    public class SMSProxy
    {
        public static ISMS GetPretreatmentService()
        {
            ISMS sms = (ISMS)Activator.GetObject(typeof(ISMS), ConfigurationManager.AppSettings["SMSService"]);
            return sms;
        }
    }
}