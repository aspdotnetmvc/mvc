using LogInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;


namespace LogQuery
{
    public class LogProxy
    {
        static ILogService logService;

        public static ILogService Instance
        {
            get
            {
                if (logService == null)
                {
                    logService = (ILogService)Activator.GetObject(typeof(ILogService), ConfigurationManager.AppSettings["LogService"]);
                }
                return logService;
            }
        }
    }
}
