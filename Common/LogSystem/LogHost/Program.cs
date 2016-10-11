using System;
using System.Configuration;
using MQAccess;
using System.Runtime.Remoting;
using System.Threading;
using System.Runtime.InteropServices;
using LogService;

namespace LogHost
{
    public class Program
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
            IntPtr windowHandle = FindWindow(null, "LogHost");
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }

        static string SubmeterDate;

        static void Main(string[] args)
        {
            Console.Title = "LogHost";
            ShieldingCloseButton();

            Console.WriteLine(DateTime.Now.ToString() + " Start the server!");
            RemotingConfiguration.Configure("LogHost.exe.config", false);
            Console.WriteLine();
            Console.WriteLine("Press input 'quit' to stop it!");
            FanoutReceive fr = new FanoutReceive(ConfigurationManager.AppSettings["MQVHost"], 
                ConfigurationManager.AppSettings["MQUrl"],
                ConfigurationManager.AppSettings["MQName"],
                ConfigurationManager.AppSettings["MQPassword"],
                ConfigurationManager.AppSettings["MQChannel"], true, 3);
            fr.ReceiveMessage += fr_ReceiveMessage;
            Thread thread = new Thread(new ThreadStart(CreateSubmeter));
            thread.Start();

            do
            {
            } while (Console.ReadLine() != "quit");
            Console.WriteLine("The server was stopped,press any key to exit!");
            Console.ReadKey();
            Environment.Exit(0);
        }

        static bool fr_ReceiveMessage(string queue, string message)
        {
            string[] msgs = message.Split((char)0x1d);
            Log.Write(msgs[0].ToString(), msgs[1].ToString(), msgs[2].ToString(), msgs[3].ToString(), msgs[4].ToString(), msgs[5].ToString(), DateTime.Now.ToString());
            Console.WriteLine(DateTime.Now.ToString() + " : Message --> " + msgs[0] + " , " + msgs[1] + " , " + msgs[2] + " , " + msgs[3] + " , " + msgs[4] + " , " + msgs[5]);
            Console.WriteLine();
            return true;
        }

        static void CreateSubmeter()
        {
            while (true)
            {

                long sleepMilliseconds = CompareDate();
                long St = sleepMilliseconds / 100000;
                int Stt = Convert.ToInt32(St);
                SubmeterDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString();
                //                SubmeterDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

                Log.CreateSubmeterTable("LOG_" + SubmeterDate);
                Thread.Sleep(Stt);
            }
        }

        private static long CompareDate()
        {
            DateTime centuryBegin = DateTime.Now;
            DateTime currentDate = DateTime.Now.AddMonths(1);
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            return elapsedTicks;
        }
    }
}
