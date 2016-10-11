using MQHelper;
using Newtonsoft.Json;
using SMS.DB;
using SMS.DTO;
using SMS.Model;

namespace SendQueueHost
{
    public class SMSOriginalSend
    {
        public static void Send(SMSDTO sms)
        {
            ReportStatisticsDB.AddSMSHistory(new ReportStatistics(sms.Message));
        }
    }
}

