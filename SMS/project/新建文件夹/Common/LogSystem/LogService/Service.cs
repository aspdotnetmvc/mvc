using LogInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogService
{
    public class Service:MarshalByRefObject,ILogService
    {
        public List<LogMessage> Query(string beginDate, string endDate, string level, string recorder, string logEnent)
        {
            return LogDB.GetLogs(level, recorder, logEnent, beginDate, endDate);
        }

        public long Heartbeat()
        {
            return DateTime.Now.Ticks;
        }


        public void Write(string level, string thread, string ip, string recorder, string _event, string message, DateTime date)
        {
            LogDB.Write(level, thread, ip, recorder, _event, message, date);
        }
    }
}
