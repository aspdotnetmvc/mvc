using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using SMS.Model;
using System.Configuration;
using MQHelper;
using System.Runtime.InteropServices;
using SMSPlatform;

namespace ZHSMSPlatHost
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
            IntPtr windowHandle = FindWindow(null, "SMSPlatformHost");
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }
        static void Main(string[] args)
        {
            Console.Title = "SMSPlatformHost";
            ShieldingCloseButton();

            Console.WriteLine(DateTime.Now.ToString() + " Starting the server!");
            Console.WriteLine(DateTime.Now.ToString() + " Loading the cache ...");
            AccountServer.Instance.LoadAccountCache();
            Console.WriteLine(DateTime.Now.ToString() + " Loading the cache ok.");
            Console.WriteLine(DateTime.Now.ToString() + " Initialize the server ...");
            RemotingConfiguration.Configure("SMSPlatformHost.exe.config", false);
            Console.WriteLine(DateTime.Now.ToString() + " Initialize the server ok.");
            Console.WriteLine(DateTime.Now.ToString() + " Started the server ok.");


            RabbitMQHelper fr = new RabbitMQHelper(AppConfig.MQHost, AppConfig.MQVHost, AppConfig.MQUserName, AppConfig.MQPassword);
            fr.OnSubsribeMessageRecieve += fr_ReceiveMessage;
            fr.SubsribeMessage(AppConfig.MQChannel);

            Console.WriteLine();
            Console.WriteLine("Press input 'quit' to stop it!");
            do
            {
            } while (Console.ReadLine() != "quit");
            Console.WriteLine("The server was stopped!");
            Console.ReadKey();
            Environment.Exit(0);

        }
        static bool fr_ReceiveMessage(RabbitMQHelper mq, string message)
        {
            //处理上行短信

            PlatformService.ProcessMOSMS(message);


            return true;
        }
        static SMSPlatformService PlatformService = new SMSPlatformService();
    }
}
