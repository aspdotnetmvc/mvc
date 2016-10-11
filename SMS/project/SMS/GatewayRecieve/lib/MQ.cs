using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MQHelper;
namespace GatewayRecieve.lib
{
    /// <summary>
    /// MQ 引用
    /// </summary>
    public class MQ
    {
        #region
        public static string MQHost = System.Configuration.ConfigurationManager.AppSettings["MQHost"];
        public static string MQVHost = System.Configuration.ConfigurationManager.AppSettings["MQVHost"];
        public static string MQUserName = System.Configuration.ConfigurationManager.AppSettings["MQUserName"];
        public static string MQPassword = System.Configuration.ConfigurationManager.AppSettings["MQPassword"];
        public static int MaxPriority = 10;
        #endregion

        private string GatewayName = "";
        private RabbitMQHelper statusreport;
        private RabbitMQHelper mo;
        public MQ(string gateway)
        {
            GatewayName = gateway;  
            statusreport = new RabbitMQHelper(MQHost,MQVHost,MQUserName,MQPassword);
            statusreport.BindQueue(GatewayName + "_SR", MaxPriority, GatewayName + "_SR");

            mo = new RabbitMQHelper(MQHost, MQVHost, MQUserName, MQPassword);
            mo.BindQueue(GatewayName + "_MO", MaxPriority, GatewayName + "_MO");
        }
        public void SendStatusReportMQ(string msg)
        {
            statusreport.PublishMessage(msg);
        }
        public void SendMO(string msg)
        {
            mo.PublishMessage(msg);
        }
    }
}