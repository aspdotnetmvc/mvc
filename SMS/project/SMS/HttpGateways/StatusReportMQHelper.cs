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
   public class StatusReportMQHelper
    {
        RabbitMQHelper StatusReportMQ;
        HttpPushGateway Gateway;
       /// <summary>
       /// 启动监听
       /// </summary>
        public void Start(HttpPushGateway gateway)
       {
           Gateway = gateway;

           StatusReportMQ = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
           StatusReportMQ.OnSubsribeMessageRecieve += OnMessageRecieve;
           StatusReportMQ.SubsribeMessage(Gateway.Gateway.Config.GatewayName+"_SR");
       }

       private bool OnMessageRecieve(RabbitMQHelper mq, string message)
       {
           var sr = JsonConvert.DeserializeObject<List<StatusResult>>(message);
           Gateway.StatusReportRecieved(sr);
           return true;
       }
       /// <summary>
       /// 停止监听
       /// </summary>
       public void Stop()
       {
           StatusReportMQ.Close();
       }
    }
}
