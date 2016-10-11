using MQHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace SMSService
{
    internal class BlacklistSend
    {
        private volatile static BlacklistSend mng = null;
        private static object lockHelper = new object();
        RabbitMQHelper fqBlacklist;

        private BlacklistSend()
        {
            fqBlacklist = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            fqBlacklist.BindQueue("Blacklist", AppConfig.MaxPriority, "Blacklist");
        }

        internal static BlacklistSend Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new BlacklistSend();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        internal void Send(string message)
        {
            fqBlacklist.PublishMessage(message);
        }
    }
}