using MQHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendQueueHost
{
    public class MOHelper
    {
        static RabbitMQHelper frMo;
        public void StartMOService()
        {
            frMo = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            frMo.OnSubsribeMessageRecieve += frMo_ReceiveMessage;
            frMo.SubsribeMessage("MOSend");
        }

        private bool frMo_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            throw new NotImplementedException();
        }
        public void StopMOService()
        {
            frMo.Close();
        }
    }
}
