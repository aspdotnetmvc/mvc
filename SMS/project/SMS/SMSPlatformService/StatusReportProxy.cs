using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SMS.Model;
using SMSInterface;
using StatusReportService;

namespace SMSPlatform
{
    public class StatusReportProxy
    {
        public static IStatusReportService GetStatusReportService()
        {
            IStatusReportService statusreport = (IStatusReportService)Activator.GetObject(typeof(IStatusReportService), ConfigurationManager.AppSettings["StatusReportService"]);
            return statusreport;
        }
    }
}