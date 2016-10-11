using GatewayInterface;
using HttpGateways;
using KeywordFilter;
using LogClient;
using MQHelper;
using Newtonsoft.Json;
using SMS.Model;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace GatewayHost
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
            IntPtr windowHandle = FindWindow(null, "GatewayHost");
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }

        //关闭控制台事件
        public delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0://Ctrl+C关闭
                case 2://按控制台关闭按钮关闭  
                    Gateway.Close();
                    break;
            }
            return false;
        }

        /////////////

        static ISMSGateway Gateway;
        static SimpleKeyword processKeyword;
        static RabbitMQHelper frSMS;
        static string GatewayName;



        static void Main(string[] args)
        {
            Console.Title = "GatewayHost";
            ShieldingCloseButton();
            SetConsoleCtrlHandler(cancelHandler, true);

            Console.WriteLine(DateTime.Now.ToString() + " Starting the server ...");
            GatewayName = System.Configuration.ConfigurationManager.AppSettings["GatewayName"];
            Console.Title = "GatewayHost - " + GatewayName;
            Console.WriteLine(DateTime.Now.ToString() + " Loading the keyword ...");

            IKeywordLoad kl = new KeywordLoad(GatewayName);
            Keyword keyword = new Keyword(kl);
            processKeyword = new SimpleKeyword(keyword);
            Console.WriteLine(DateTime.Now.ToString() + " Loading the keyword ok.");
            Console.WriteLine(DateTime.Now.ToString() + " Initialize the server ...");
            string gatewayType = System.Configuration.ConfigurationManager.AppSettings["GatewayType"];

            if (gatewayType.ToLower() == "http")  //普通http接口
            {
                Gateway = new HttpGateway(GatewayName);
            }
            else if (gatewayType.ToLower() == "httppush") //推送状态报告和上行
            {
                Gateway = new HttpPushGateway(GatewayName);
            }
            else if (gatewayType.ToLower() == "httpsdl") //四大类
            {
                Gateway = new HttpSDLGateway(GatewayName);
            }
            Gateway.SMSEvent += Gateway_SMSEvent;
            Gateway.SendEvent += Gateway_SendEvent;
            Gateway.ReportEvent += Gateway_ReportEvent;
            Gateway.DeliverEvent += Gateway_DeliverEvent;
            Gateway.Connect();
            Thread.Sleep(1000);
            frSMS = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            frSMS.OnSubsribeMessageRecieve += frSMS_ReceiveMessage;
            frSMS.SubsribeMessage(GatewayName, AppConfig.MaxPriority);

            Console.WriteLine(DateTime.Now.ToString() + " Initialize the server ok.");
            Console.WriteLine(DateTime.Now.ToString() + " Started the server ok.");
            Console.WriteLine();
            Console.WriteLine("Press input 'quit' to stop it!");

            do
            {
            } while (Console.ReadLine() != "quit");
            frSMS.Close();
            Gateway.Close();
            Console.WriteLine("The server was stopped,press any key to exit!");
            Console.ReadKey();
            Environment.Exit(0);

        }

        static void Gateway_DeliverEvent(object sender, DeliverEventArgs e)
        {
            try
            {
                //写库
                MOSend.Instance.Send(JsonConvert.SerializeObject(e.MO));
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Gateway", "GatewayHost.Gateway_DeliverEvent", ex.ToString());
            }
        }

        static bool frSMS_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            PlainSMS sms = null;
            try
            {
                sms = JsonConvert.DeserializeObject<PlainSMS>(message);
                //短信过滤
                if (sms.FilterType == (int)FilterType.Failure)
                {
                    string[] rk = processKeyword.Find(sms.Content);
                    if (rk.Length > 0)
                    {
                        int StatusCode = ((ushort)PlatformCode.SYS + (ushort)SystemCode.IllegalKeyword);
                        string Description = "关键词: " + string.Join(",", rk);
                        var reports = from num in sms.Numbers.Split(',')
                                      select new StatusReport()
                                      {
                                          SMSID = sms.ID,
                                          SendTime = sms.SendTime.Value,
                                          SerialNumber = System.Guid.NewGuid().ToString(),
                                          StatusCode = StatusCode,
                                          Succeed = false,
                                          Channel = sms.Channel,
                                          Gateway = GatewayName,
                                          Description = Description,
                                          Number = num,
                                          ResponseTime = DateTime.Now,
                                          StatusReportType = (StatusReportType)sms.StatusReportType
                                      };
                        ReportSend.Instance.BatchSend(reports.ToList());
                        return true;
                    }
                }
                else if (sms.FilterType == (int)FilterType.Replace)
                {
                    sms.Content = processKeyword.Replace(sms.Content);
                }

                Gateway.SendSMS(sms);
                return true;


            }
            catch (Exception ex)
            {
                LogHelper.LogError("Gateway", "GatewayHost.frSMS_ReceiveMessage", ex.ToString());
            }

            return true;
        }

        static void Gateway_ReportEvent(object sender, ReportEventArgs e)
        {
            ReportSend.Instance.BatchSend(e.StatusReports);
        }

        static void Gateway_SendEvent(object sender, ReportEventArgs e)
        {
            ReportSend.Instance.BatchSend(e.StatusReports);
        }

        static void Gateway_SMSEvent(object sender, SMSEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Event -----> " + e.Type);
        }
    }
}
