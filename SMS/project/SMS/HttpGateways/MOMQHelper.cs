using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MQHelper;
using Newtonsoft.Json;
using SMS.Model;
using GatewayInterface;

namespace HttpGateways
{
   public class MOMQHelper
    {
        RabbitMQHelper MOMQ;
        HttpPushGateway Gateway;
       /// <summary>
       /// 启动监听
       /// </summary>
        public void Start(HttpPushGateway gateway)
       {
           Gateway = gateway;

           MOMQ = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
           MOMQ.OnSubsribeMessageRecieve += OnMessageRecieve;
           MOMQ.SubsribeMessage(Gateway.Gateway.Config.GatewayName + "_MO");
       }

       private bool OnMessageRecieve(RabbitMQHelper mq, string message)
       {
           var mo = JsonConvert.DeserializeObject<List<MOSMS>>(message);
           Gateway.MORecieved(mo);
           return true;
       }
       /// <summary>
       /// 停止监听
       /// </summary>
       public void Stop()
       {
           MOMQ.Close();
       }
    }
}
