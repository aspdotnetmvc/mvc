using LogInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace LogService
{
    public class LogDB
    {
        public static bool Write(LogMessage log)
        {
            return Write(log.Level, log.Thread, log.Ip, log.Recorder, log.Event, log.Message, log.Date.ToString());
        }

        public static bool Write(string level, string thread, string ip, string recorder, string _event, string message, string date)
        {
            DateTime d;
            try
            {
                d = DateTime.Parse(date);
            }
            catch
            {
                d = DateTime.Now;
            }
            return Write(level, thread, ip, recorder, _event, message, d);
        }

        public static bool Write(string level, string thread, string ip, string recorder, string _event, string message, DateTime date)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into LOG(");
            strSql.Append("Date,Thread,IP,Logger,EventName,Level,Message)");
            strSql.Append(" values (");
            strSql.Append("@Date,@Thread,@IP,@Logger,@EventName,@Level,@Message)");

            DBHelper.Instance.Execute(strSql.ToString(),
                new
                {
                    Date = date,
                    Thread = thread,
                    IP = ip,
                    Logger = recorder,
                    EventName = _event,
                    Level = level,
                    Message = message
                });

            return true;
        }

        public static void CreateSubmeterTable(string SubmeterName)
        {
            string sql = "SELECT count(1) FROM LOG_Table WHERE Submeter='" + SubmeterName + "'";
            var c = DBHelper.Instance.ExecuteScalar<int>(sql);
            if (c == 0)
            {
                sql = "DROP TABLE IF EXISTS `" + SubmeterName + "`;CREATE TABLE IF NOT EXISTS `" + SubmeterName + "` ( `ID` int(10) NOT NULL AUTO_INCREMENT, `Date` datetime DEFAULT NULL, `Thread` varchar(200) DEFAULT NULL, `IP` varchar(50) DEFAULT NULL,`Logger` varchar(200) DEFAULT NULL,`EventName` varchar(200) DEFAULT NULL,`Level` varchar(50) DEFAULT NULL, `Message` text,`Exception` text, PRIMARY KEY (`ID`)) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT";
                DBHelper.Instance.Execute(sql);
                sql = "SELECT Submeter FROM LOG_Table";
                var l = DBHelper.Instance.Query<string>(sql);
                string str = string.Join(",", l);
                str += "," + SubmeterName;
                sql = "DROP TABLE IF EXISTS `LOG`;CREATE TABLE IF NOT EXISTS `LOG` ( `ID` int(10) NOT NULL AUTO_INCREMENT,`Date` datetime DEFAULT NULL,`Thread` varchar(200) DEFAULT NULL, `IP` varchar(50) DEFAULT NULL, `Logger` varchar(200) DEFAULT NULL,`EventName` varchar(200) DEFAULT NULL, `Level` varchar(50) DEFAULT NULL, `Message` text,`Exception` text, KEY `ID` (`ID`)) ENGINE=MRG_MyISAM DEFAULT CHARSET=utf8 INSERT_METHOD=LAST UNION=(" + str + ")";
                DBHelper.Instance.Execute(sql);
                sql = "INSERT INTO LOG_Table (Submeter) VALUES('" + SubmeterName + "')";
                DBHelper.Instance.Execute(sql);
            }
        }

        public static List<LogMessage> GetLogs(string Level, string Recorder, string Event, string BeginTime, string EndTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Date,Thread,IP as Ip,Logger,EventName,Level,Message from LOG ");
            if (string.IsNullOrEmpty(BeginTime) || string.IsNullOrEmpty(EndTime))
            {
                return new List<LogMessage>();
            }
            if (DateTime.Compare(DateTime.Parse(BeginTime), DateTime.Parse(EndTime)) > 0)
            {
                return new List<LogMessage>();
            }
            strSql.Append(" where Date>=@BeginTime and Date<=@EndTime and (1=1 or Level=@Level or Logger=@Logger or EventName=@EventName) ");
            string level = Level;
            string logger = Recorder;
            string _event = Event;

            if (!string.IsNullOrEmpty(Level))
            {
                strSql.Append(" and Level=@Level");
            }
            else
            {
                level = "1";
            }
            if (!string.IsNullOrEmpty(Recorder))
            {
                strSql.Append(" and Logger=@Logger");
            }
            else
            {
                logger = "1";
            }
            if (!string.IsNullOrEmpty(Event))
            {
                strSql.Append(" and EventName=@EventName");
            }
            else
            {
                _event = "1";
            }
            return DBHelper.Instance.Query<LogMessage>(strSql.ToString(), new { BeginTime = BeginTime, EndTime = EndTime, Logger = logger, EventName = _event, Level = level });
        }
    }
}
