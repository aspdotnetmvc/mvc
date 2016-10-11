using MQHelper;
using Newtonsoft.Json;
using SMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSService
{
    /// <summary>
    /// 短信提交到MQ
    /// </summary>
    public class SMSSubmit
    {
        private RabbitMQHelper mq;

        private SMSSubmit()
        {
            mq = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            //设置最多10个优先级
            mq.BindQueue("SendQueue", AppConfig.MaxPriority, "SendQueue");
        }

        private static SMSSubmit _instance = null;
        private static object locker = new object();

        public static SMSSubmit Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new SMSSubmit();
                        }
                    }
                }
                return _instance;
            }
        }

        public void SendSMS(SMSDTO sms)
        {
            string message = JsonConvert.SerializeObject(sms);
            mq.PublishMessage(message, sms.Message.SMSLevel);
        }
    }
}
