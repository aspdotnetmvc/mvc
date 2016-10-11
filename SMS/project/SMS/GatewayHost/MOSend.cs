using MQHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace GatewayHost
{
    public class MOSend
    {
        private volatile static MOSend mng = null;
        private static object lockHelper = new object();
        RabbitMQHelper fqBilling;

        private MOSend()
        {
            fqBilling = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            fqBilling.BindQueue("MOSend", AppConfig.MaxPriority, "MOSend", "MOProcess");
        }

        public static MOSend Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new MOSend();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        public void Send(string message)
        {
            fqBilling.PublishMessage(message);
        }
    }
}

