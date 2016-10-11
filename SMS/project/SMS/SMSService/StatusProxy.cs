using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SMS.Model;
using SMSInterface;
using StatusReportService;

namespace SMSService
{
    public class StatusProxy
    {
        public static IStatusReportService GetStatusReportService()
        {
            IStatusReportService sms = (IStatusReportService)Activator.GetObject(typeof(IStatusReportService), ConfigurationManager.AppSettings["StatusReportService"]);
            return sms;
        }
    }
}