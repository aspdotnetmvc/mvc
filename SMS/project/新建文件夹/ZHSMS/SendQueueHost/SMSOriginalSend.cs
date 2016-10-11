using MQHelper;
using Newtonsoft.Json;
using SMS.DB;
using SMS.DTO;
using SMS.Model;

namespace SendQueueHost
{
    internal class SMSOriginalSend
    {
        private volatile static SMSOriginalSend mng = null;
        private static object lockHelper = new object();
        RabbitMQHelper fqSMS;

        private SMSOriginalSend()
        {
            fqSMS = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            fqSMS.BindQueue("SMSOriginal", AppConfig.MaxPriority, "SMSOriginal");
        }

        internal static SMSOriginalSend Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new SMSOriginalSend();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        internal void Send(SMSDTO sms)
        {
            ReportStatisticsDB.AddSMSHistory(new ReportStatistics(sms.Message));
            fqSMS.PublishMessage(JsonConvert.SerializeObject(sms),sms.Message.SMSLevel);
        }
    }
}

