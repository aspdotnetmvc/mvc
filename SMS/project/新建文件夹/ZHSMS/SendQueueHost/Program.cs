using LogClient;
using MQHelper;
using Newtonsoft.Json;
using SMS.DB;
using SMS.DTO;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SendQueueHost
{
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);
        /// <summary>
        /// 禁用关闭按钮
        /// </summary>
        static void ShieldingCloseButton()
        {
            IntPtr windowHandle = FindWindow(null, "SendQueueHost");
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }
        static object _lockSendScan = new object();
        static RabbitMQHelper accountBlacklist;
        static RabbitMQHelper SMSMQHelper;
        static Dictionary<string, List<GatewayHelper>> Channels = new Dictionary<string, List<GatewayHelper>>();

        static void Main(string[] args)
        {
            Console.Title = "SendQueueHost";
            ShieldingCloseButton();

            Console.WriteLine(DateTime.Now.ToString() + " Starting the server ...");
            Console.WriteLine(DateTime.Now.ToString() + " Loading the config ...");

            Console.WriteLine(DateTime.Now.ToString() + " Loading the config ok.");
            Console.WriteLine(DateTime.Now.ToString() + " Loading the cache ...");
            Console.WriteLine(DateTime.Now.ToString() + " Loading the cache ok.");
            Console.WriteLine(DateTime.Now.ToString() + " Initialize the server ...");
            accountBlacklist = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            accountBlacklist.OnSubsribeMessageRecieve += accountBlacklist_ReceiveMessage;
            accountBlacklist.SubsribeMessage("Blacklist", AppConfig.MaxPriority);

            SMSMQHelper = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            accountBlacklist.OnSubsribeMessageRecieve += SMSMQHelper_RecieveMessage;
            accountBlacklist.SubsribeMessage("SendQueue", AppConfig.MaxPriority);

            Console.WriteLine(DateTime.Now.ToString() + " Initialize the server ok.");
            Console.WriteLine(DateTime.Now.ToString() + " Started the server ok.");
            Console.WriteLine();
            Console.WriteLine("Press input 'quit' to stop it!");

            do
            {
            } while (Console.ReadLine() != "quit");
            SMSMQHelper.Close();
            accountBlacklist.Close();
            Console.WriteLine("The server was stopped!");

            Console.ReadKey();
            Environment.Exit(0);
        }


        static void LoadChannel()
        {
            var channels = ChannelDB.GetChannels();
            var gateways = GatewayConfigDB.GetConfigs();
            foreach (var ch in channels)
            {
                List<GatewayHelper> gatewayHelpers = new List<GatewayHelper>();
                List<string> gatewayIds = ChannelDB.GetGatewaysByChannel(ch.ChannelID);
                foreach (var gid in gatewayIds)
                {
                    var gatewayconfig = gateways.FirstOrDefault(g => g.Gateway == gid);
                    if (gatewayconfig == null)
                    {
                        throw new Exception(ch.ChannelName + " 找不到网关" + gid + "的配置");
                    }
                    GatewayHelper gh = new GatewayHelper(gatewayconfig);
                    gatewayHelpers.Add(gh);
                }
                Channels.Add(ch.ChannelID, gatewayHelpers);
            }

        }
        private static bool SMSMQHelper_RecieveMessage(RabbitMQHelper mq, string message)
        {
            try
            {
                var sms = JsonConvert.DeserializeObject<SMSDTO>(message);
                SMSOriginalSend.Instance.Send(sms);
                foreach (var smsnumber in sms.SMSNumbers)
                {
                    try
                    {
                        GatewayHelper gateway = GetGatewayByOperator(sms.Message.Channel, smsnumber.Operator);
                        if (gateway == null)
                        {
                            SendFail(PlainSMS.CreatePlainSMS(sms.Message,smsnumber), "没有找到可用网关！",22);
                        }
                        SMSDTO dto = new SMSDTO() { Message = sms.Message };
                        dto.SMSNumbers = new List<SMSNumber>() { smsnumber };
                        gateway.SendSMS(dto);
                    }
                    catch (Exception ex)
                    {
                        //记录异常
                        LogHelper.LogError("SendQueue", "SMSMQHelper_RecieveMessage", "发送短信时发生异常:" + ex.ToString());
                        SendFail(PlainSMS.CreatePlainSMS(sms.Message, smsnumber), "发送时失败！", 99);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SendQueue", "SMSMQHelper_RecieveMessage", "解析短信时发生异常:" + ex.ToString());
                LogHelper.LogError("SendQueue", "SMSMQHelper_RecieveMessage", message);
                // throw ex;
                return true;
            }
        }

        private static void SendFail(PlainSMS sms, string message,ushort statusCode)
        {
            try
            {
                var reports = from ss in sms.Numbers.Split(',')
                              select new StatusReport()
                              {

                              };

                ReportSend.Instance.BatchSend(reports.ToList());
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SendQueue", "SendQueueHost._gatewayHandler_SendSMSError", ex.ToString());
            }
        }
        private static GatewayHelper GetGatewayByOperator(string channel, OperatorType operatorType)
        {
            return Channels[channel].FirstOrDefault(gh => gh.HasOperators(operatorType));
        }
        private static bool accountBlacklist_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            try
            {
                string[] blData = message.Split((char)2);
                if (blData[0] == "Add")
                {
                    //添加黑名单
                    List<string> list = new List<string>();
                    for (int i = 1; i < blData.Length; i++)
                    {
                        list.Add(blData[i]);
                    }
                    BlacklistManager.Instance.Add(list);

                }
                if (blData[0] == "Dec")
                {
                    //删除黑名单
                    List<string> list = new List<string>();
                    for (int i = 1; i < blData.Length; i++)
                    {
                        list.Add(blData[i]);
                    }
                    BlacklistManager.Instance.Del(list);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SendQueue", "SendQueueHost.accountBlacklist_ReceiveMessage", ex.ToString());
            }
            return true;
        }
    }
}
