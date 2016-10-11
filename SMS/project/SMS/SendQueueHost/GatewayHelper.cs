using Newtonsoft.Json;
using SMS.DTO;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SendQueueHost
{
    public class GatewayHelper
    {
        #region

        #endregion


        public GatewayHelper(GatewayConfiguration config)
        {
            this.GatewayConfig = config;
            Operators = (from o in config.Operators select (OperatorType)Enum.Parse(typeof(OperatorType), o)).ToList();

            SendMQHelper = new MQHelper.RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            SendMQHelper.BindQueue(config.Gateway, AppConfig.MaxPriority, config.Gateway);
        }
        public GatewayConfiguration GatewayConfig { get; set; }
        /// <summary>
        /// 该网关支持的运营商
        /// </summary>
        public List<OperatorType> Operators { get; set; }
        /// <summary>
        /// 判断网关是否支持该运营商
        /// </summary>
        /// <param name="ot"></param>
        /// <returns></returns>
        public bool HasOperators(OperatorType ot)
        {
            return Operators.Any(o => o == ot);
        }
        public MQHelper.RabbitMQHelper SendMQHelper { get; set; }
        /// <summary>
        /// 把短信发送到MQ
        /// 负责拆包,递交出去的PlainSMS 
        /// </summary>
        /// <param name="sms"></param>
        public void SendSMS(SMSDTO sms)
        {
            foreach (var smsnumber in sms.SMSNumbers)
            {
                if (smsnumber.NumberCount > this.GatewayConfig.MaxPackageSize)
                {

                    List<string> Numbers = smsnumber.Numbers.Split(',').ToList();
                    for (int i = 0; i < Numbers.Count; i = i + this.GatewayConfig.MaxPackageSize)
                    {
                        var numbers = Numbers.Skip(i).Take(this.GatewayConfig.MaxPackageSize).ToList();

                        PlainSMS tmp = PlainSMS.CreatePlainSMS(sms.Message, numbers, smsnumber.Operator);


                        this.SendMQHelper.PublishMessage(JsonConvert.SerializeObject(tmp), sms.Message.SMSLevel);
                    }
                }
                else
                {
                    PlainSMS tmp = PlainSMS.CreatePlainSMS(sms.Message, smsnumber);

                    this.SendMQHelper.PublishMessage(JsonConvert.SerializeObject(tmp), sms.Message.SMSLevel);
                }
            }
        }
    }
}
