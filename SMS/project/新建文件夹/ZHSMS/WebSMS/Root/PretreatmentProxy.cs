using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SMSModel;
using StatusReportInterface;

namespace WebSMS
{
    public class PretreatmentProxy
    {
        public static ISMS GetPretreatment()
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