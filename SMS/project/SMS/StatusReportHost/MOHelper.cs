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
    public class MOHelper
    {
        static RabbitMQHelper frMo;
        public void StartMOService()
        {
            frMo = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            frMo.OnSubsribeMessageRecieve += frMo_ReceiveMessage;
            frMo.SubsribeMessage("MOSend");
            MOCache.Instance.LoadMOCache();
        }

        private bool frMo_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            try
            {
                var mo = JsonConvert.DeserializeObject<MOSMS>(message);
                //找到给发短信的记录
                var sms = StatusReportDB.GetSMSForMO(mo.Gateway, mo.ReceiveTime, mo.UserNumber);
                if (sms != null)
                {
                    if (string.IsNullOrWhiteSpace(mo.SPNumber))
                    {
                        mo.SPNumber = sms.SPNumber;
                    }
                    mo.AccountID = sms.AccountID;
                }
                DeliverMODB.Add(mo);
                MOCache.Instance.AddMOCache(mo);
                return true;
            }
            catch (Exception ex)
            {
                LogClient.LogHelper.LogError("SendQueue", "frMo_ReceiveMessage", ex.ToString());
                return true;
            }
        }
        public void StopMOService()
        {
            frMo.Close();
        }
    }
}
