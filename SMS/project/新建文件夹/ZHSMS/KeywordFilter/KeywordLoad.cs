using MQHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace KeywordFilter
{
    public class KeywordLoad:IKeywordLoad
    {
        public event EventHandler UpdateKeyword;
        string _gateway;
        RabbitMQHelper fr;
        List<string> keywords = new List<string>();
        public KeywordLoad(string gateway)
        {
            _gateway = gateway;
            fr = new RabbitMQHelper(AppConfig.MQHost,AppConfig.MQVHost,AppConfig.MQUserName,AppConfig.MQPassword);
            fr.OnSubsribeMessageRecieve+=fr_ReceiveMessage;
            fr.SubsribeMessage("Keyword_" + gateway);
        }

        bool fr_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            try
            {
                string[] up = message.Split((char)5);
                if (up.Length > 0)
                {
                    if (up[0] == "ADD")
                    {
                        for(int i=1;i<up.Length;i++)
                        {
                            bool f = false;
                            foreach (string k in keywords)
                            {
                                if (k == up[i])
                                {
                                    f = true;
                                    break;
                                }
                            }
                            if (!f)
                            {
                                keywords.Add(up[i]);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; i < up.Length; i++)
                        {
                            for (int j = 0; j < keywords.Count;j++ )
                            {
                                if (keywords[j] == up[i])
                                {
                                    keywords.RemoveAt(j);
                                    break;
                                }
                            }
                        }
                    }
                }

                if (UpdateKeyword != null)
                {
                    UpdateKeyword(this, null);
                }
            }
            catch
            {

            }

            return true;
        }

        public List<string> Load()
        {
            keywords.Clear();
            string g = SMS.DB.KeywordsGatewayBindDB.GetkeyGroup(_gateway);
            keywords.AddRange(SMS.DB.WordfilteDB.Get(g));
            return keywords;
        }
    }
}
