using SMS.DB;
using MQHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace SMSService
{
    public class KeywordsSend
    {
        private volatile static KeywordsSend mng = null;
        private static object lockHelper = new object();

        public static string KeyGroup { get; set; }

        public KeywordsSend()
        {

        }

        internal static KeywordsSend Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new KeywordsSend();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        public void Send(string keyGroup, string message)
        {
            lock (lockHelper)
            {
                RabbitMQHelper fqKeywords;
                List<string> update = KeywordsGatewayBindDB.GetGateways(keyGroup);
                for (int i = 0; i < update.Count; i++)
                {
                    update[i] = "Keyword_" + update[i];
                }
                fqKeywords = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
                fqKeywords.BindQueue("Keyword_" + KeyGroup, AppConfig.MaxPriority, update.ToArray());
                fqKeywords.PublishMessage(message);
            }
        }
    }
}