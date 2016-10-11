using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogInterface
{
    public interface ILogService
    {
        List<LogMessage> Query(string beginDate, string endDate, string level, string recorder, string logEnent);
        void Write(string level, string thread, string ip, string recorder, string _event, string message, DateTime date);
    }
}
