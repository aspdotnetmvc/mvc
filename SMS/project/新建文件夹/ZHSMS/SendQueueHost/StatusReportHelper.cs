using MQHelper;
using Newtonsoft.Json;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendQueueHost
{
    public class StatusReportHelper
    {
        static RabbitMQHelper frReport;
        public void StartStatusService()
        {
            frReport = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            frReport.OnSubsribeMessageRecieve += frReport_ReceiveMessage;
            frReport.SubsribeMessage("Report");
        }

        private bool frReport_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            
            try
            {
               var sr = JsonConvert.DeserializeObject<StatusReport>(message);
       
            int status = sr.StatusCode - (sr.StatusCode / 1000) * 1000;
            if (status < 100)
            {
                GatewaySend(sr);
            }
            else if (status < 200)
            {
                GatewayResponse(sr);
            }
     }
            catch (Exception ex)
            {
                LogClient.LogHelper.LogError("StatusReport", "StatusReportHost.frReport_ReceiveMessage", ex.ToString());
            }
            return true;
        }

        private void GatewayResponse(StatusReport sr)
        {
            throw new NotImplementedException();
        }

        private void GatewaySend(StatusReport sr)
        {
            throw new NotImplementedException();
        }
        public void StopStatusService()
        {
            frReport.Close();
        }
    }
}
