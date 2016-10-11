using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LogInterface;
using System.Threading;

namespace LogService
{
    public class Log
    {
        public static void Write(string level,string thread, string ip, string recorder, string _event,  string message,string date)
        {
            try
            {
                LogDB.Write(level, thread, ip, recorder, _event, message,date);
            }
            catch(Exception ex)
            {
                //LogManager.ResetConfiguration();
            }
        }

        public static bool CreateSubmeterTable(string submeterName)
        {
            try
            {
                LogDB.CreateSubmeterTable(submeterName);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}