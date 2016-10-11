using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SMSModel;
using StatusReportInterface;

namespace ZHSMSServiceHost
{
    public class ZHSMSProxy
    {
        public static ISMS GetPretreatmentService()
        {
            ISMS sms = (ISMS)Activator.GetObject(typeof(ISMS), ConfigurationManager.AppSettings["Pretreatment"]);
            return sms;
        }
        public static IStatusReport GetStatusReportService()
        {
            IStatusReport sr = (IStatusReport)Activator.GetObject(typeof(IStatusReport), ConfigurationManager.AppSettings["StatusReportService"]);
            return sr;
        }
    }
}