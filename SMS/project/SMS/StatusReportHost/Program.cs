using LogClient;
using MQHelper;
using Newtonsoft.Json;
using SMS.DB;
using SMS.DTO;
using SMS.Model;
using SMSService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;

namespace StatusReportService
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

        static void Main(string[] args)
        {
            Console.Title = "StatusReportHost";
            ShieldingCloseButton();

            Console.WriteLine(DateTime.Now.ToString() + " Starting the server ...");

            MessageTools.MessageHelper.Instance.WirteInfo(DateTime.Now.ToString() + " 启动分表服务...");
            TableService.Instance.GetStatisticsTables();
            MessageTools.MessageHelper.Instance.WirteInfo(DateTime.Now.ToString() + " 启动分表服务完成.");

            MessageTools.MessageHelper.Instance.WirteInfo(DateTime.Now.ToString() + " 启动状态报告服务...");
            StatusReportHelper.Instance.StartStatusService();
            MessageTools.MessageHelper.Instance.WirteInfo(DateTime.Now.ToString() + " 启动状态报告服务完成.");
            MessageTools.MessageHelper.Instance.WirteInfo(DateTime.Now.ToString() + " 启动短信上行服务...");
            MOHelper MOHelper = new MOHelper();
            MOHelper.StartMOService();
            MessageTools.MessageHelper.Instance.WirteInfo(DateTime.Now.ToString() + " 启动短信上行服务完成.");
            Console.WriteLine(DateTime.Now.ToString() + " 启动远程服务 ...");
            RemotingConfiguration.Configure("StatusReportHost.exe.config", false);

            Console.WriteLine(DateTime.Now.ToString() + " 启动远程服务完成.");
            Console.WriteLine(DateTime.Now.ToString() + " 服务启动完成");
            Console.WriteLine();
            Console.WriteLine("Press input 'quit' to stop it!");

            do
            {
            } while (Console.ReadLine() != "quit");
            StatusReportHelper.Instance.StopStatusService();
            MOHelper.StopMOService();
            Console.WriteLine("服务已停止!");

            Console.ReadKey();
            Environment.Exit(0);
        }
        /// <summary>
        /// 
        /// </summary>
        static void StartSMSTimerMonitor()
        {
            Task t = new Task(() =>
            {
                while (true)
                {
                    var list = SMSDAL.GetTimerSMS();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var smsid in list)
                        {
                            var sms = SMSDAL.GetSMSById(smsid);
                            SMSSubmit.Instance.SendSMS(sms);
                        }
                    }
                    Thread.Sleep(1000 * 30);
                }
            });
            t.Start();
        }
    }
}
