using BXM.Utils;
using DatabaseAccess;
using LogClient;
using MQHelper;
using SMS.DTO;
using SMSModel;
using StatusReportService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;

namespace StatusReportHost
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
            IntPtr windowHandle = FindWindow(null, "StatusReportHost");
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }
        static RabbitMQHelper frReport;
        static RabbitMQHelper frMo;
        static RabbitMQHelper frSMSOriginal;

        static object locker = new object();
        static void Main(string[] args)
        {
            Console.Title = "状态报告服务";
            ShieldingCloseButton();

            Console.WriteLine(DateTime.Now.ToString() + " Starting the server!");

            Console.WriteLine(DateTime.Now.ToString() + " Initialize the server ...");
            RemotingConfiguration.Configure("StatusReportHost.exe.config", false);

            frReport = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            frReport.OnSubsribeMessageRecieve += frReport_ReceiveMessage;
            frReport.SubsribeMessage("Report");

            frMo = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            frMo.OnSubsribeMessageRecieve += frMo_ReceiveMessage;
            frMo.SubsribeMessage("MOSend");

            frMo = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            frSMSOriginal.OnSubsribeMessageRecieve += frSMSOriginal_ReceiveMessage;
            frMo.SubsribeMessage("SMSOriginal");

            Console.WriteLine(DateTime.Now.ToString() + " Initialize the server ok.");
            Console.WriteLine(DateTime.Now.ToString() + " Started the server ok.");
            Console.WriteLine();
            Console.WriteLine("Press input 'quit' to stop it!");

            do
            {
            } while (Console.ReadLine() != "quit");

            Console.WriteLine("The server was stopped,press any key to exit!");
            Console.ReadKey();
            Environment.Exit(0);
        }



        private static bool frSMSOriginal_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            try
            {
                var rs = JsonSerialize.Instance.Deserialize<ReportStatistics>(message);
                ReportStatisticsDB.AddSMSHistory(rs);
            }
            catch (Exception ex)
            {
                LogClient.LogHelper.LogError("StatusReport", "StatusReportHost.frSMSOriginal_ReceiveMessage", ex.ToString());
            }
            return true;
        }

        private static bool frMo_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            throw new NotImplementedException();
            //MOSMS mo = null;
            //try
            //{
            //    mo = JsonSerialize.Instance.Deserialize<MOSMS>(message);

            //    if (string.IsNullOrWhiteSpace(mo.SPNumber))
            //    {
            //        //如果spNumber 是空，那么从最近给该用户发短信的记录里找
            //        var rl = CacheStatisticsReport.Instance.GetStatistics(DateTime.Now.AddDays(-3), DateTime.Now.AddDays(1));
            //        var n = rl.FirstOrDefault(r => r.Telephones.Contains(mo.UserNumber));
            //        if (n != null)
            //        {
            //            mo.SPNumber = n.SPNumber;
            //        }
            //    }

            //    DeliverMODB.Add(mo);
            //    CacheMOSMS.Instance.Set(mo);
            //}
            //catch (Exception ex)
            //{
            //    LogClient.LogHelper.LogError("StatusReport", "StatusReportHost.frMo_ReceiveMessage", ex.ToString());
            //}
            return true;
        }


        static bool frReport_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            StatusReport sr = null;
            try
            {
                sr = JsonSerialize.Instance.Deserialize<StatusReport>(message);
            }
            catch (Exception ex)
            {
                LogClient.LogHelper.LogError("StatusReport", "StatusReportHost.frReport_ReceiveMessage", ex.ToString());
            }
            int status = sr.StatusCode - (sr.StatusCode / 1000) * 1000;
            if (status < 100)
            {
                GatewaySend(sr);
            }
            else if (status < 200)
            {
                GatewayResponse(sr);
            }

            return true;
        }
        /// <summary>
        /// 网关发送的数据处理
        /// </summary>
        /// <param name="sendsr"></param>
        static void GatewaySend(StatusReport sendsr)
        {
            lock (locker)
            {
                try
                {

                    if (string.IsNullOrWhiteSpace(sendsr.SerialNumber))
                    {
                        sendsr.SerialNumber = Guid.NewGuid().ToString();
                    }

                    //报告统计
                    AddReportAndStatistics(sendsr);
                    if (!sendsr.Succeed)
                    {
                        //短信返费
                        AccountHelper.ReturnFee(sendsr.SMSID, sendsr.Number);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogError("StatusReport", "StatusReportHost.GatewaySend", ex.ToString());

                }
            }
        }
        /// <summary>
        /// 网关返回的数据处理
        /// </summary>
        /// <param name="responsesr"></param>
        static void GatewayResponse(StatusReport responsesr)
        {
            lock (locker)
            {
                try
                {
                    AddReportAndStatistics(responsesr);
                    //失败返费
                    if (!responsesr.Succeed)
                    {
                        AccountHelper.ReturnFee(responsesr.SMSID, responsesr.Number);
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.LogError("StatusReport", "StatusReportHost.GatewayResponse", ex.ToString());
                }
            }
        }
        /// <summary>
        /// 统计报告更新（网关返回）
        /// </summary>
        /// <param name="report"></param>
        /// <param name="failureCount"></param>
        static void AddReportAndStatistics(StatusReport report)
        {
            throw new NotImplementedException();
            //记录状态报告
            StatusReportDB.AddStatusReport(report);
            //增加统计
        }
    }
}
