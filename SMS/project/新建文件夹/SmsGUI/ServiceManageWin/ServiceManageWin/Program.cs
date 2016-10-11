using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ServiceManageWin
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
             
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            BasicConfigurator.Configure();
            Application.Run(new MainForm());
        }
    }
}
