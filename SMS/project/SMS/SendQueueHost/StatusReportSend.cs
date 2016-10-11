using MQHelper;
using Newtonsoft.Json;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace SendQueueHost
{
    internal class StatusReportSend
    {
        private volatile static StatusReportSend mng = null;
        private static object lockHelper = new object();
        RabbitMQHelper fqReport;


        private StatusReportSend()
        {
            fqReport = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            fqReport.BindQueue("Report",AppConfig.MaxPriority,"Report");
        }

        internal static StatusReportSend Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new StatusReportSend();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        internal void Send(StatusReport report)
        {
            BatchSend(new List<StatusReport>() { report});
        }
        internal void BatchSend(List<StatusReport> reports)
        {
            fqReport.PublishMessage(JsonConvert.SerializeObject(reports));
        }
    }
}
