using MQHelper;
using Newtonsoft.Json;
using SMS.DB;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatusReportService
{
    public class StatusReportHelper
    {
        public RabbitMQHelper frReport;

        private static StatusReportHelper _instance;
        public static StatusReportHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StatusReportHelper();
                }
                return _instance;
            }
        }

      

        private bool frReport_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            try
            {
                var sr = JsonConvert.DeserializeObject<List<StatusReport>>(message);
               
                if (sr != null && sr.Count > 0)
                { 
                    MessageTools.MessageHelper.Instance.WirteTest(message);
                    sr.ForEach(s => { AddStatusReport(s); });
                }
            }
            catch (Exception ex)
            {
                LogClient.LogHelper.LogError("StatusReport", "StatusReportHost.frReport_ReceiveMessage", ex.ToString());
            }
            return true;
        }

        public void AddStatusReport(StatusReport sr)
        {

            int status = sr.StatusCode - (sr.StatusCode / 1000) * 1000;
            if (status < 100)  //发送时
            {
                StatusReportDB.AddStatusReport(sr);
                if (!sr.Succeed && sr.StatusReportType > 0)
                {
                    AddStatusReportCache(sr);
                }
            }
            else  //返回时
            {
                StatusReportDB.UpdateStatusReport(sr);
                if (sr.StatusReportType > 0)
                {
                    AddStatusReportCache(sr);
                }
            }
        }

        private void AddStatusReportCache(StatusReport sr)
        {
            StatusReportCache.Instance.AddStatusReportCache(sr); 
        }
        public void StartStatusService()
        {
            frReport = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            frReport.OnSubsribeMessageRecieve += frReport_ReceiveMessage;
            frReport.SubsribeMessage("Report");
            StatusReportCache.Instance.LoadStatusReportCache();
        }
        public void StopStatusService()
        {
            frReport.Close();
        }
    }
}
