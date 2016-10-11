using SMSService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;

namespace PretreatmentHost
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
            IntPtr windowHandle = FindWindow(null, "SMSHost");
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }

        static void Main(string[] args)
        {
            Console.Title = "SMSHost";
            ShieldingCloseButton();

            Console.WriteLine(DateTime.Now.ToString() + " Starting the server ...");
            Console.WriteLine(DateTime.Now.ToString() + " Initialize the server ...");
            RemotingConfiguration.Configure("SMSHost.exe.config", false);
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
    }
}
