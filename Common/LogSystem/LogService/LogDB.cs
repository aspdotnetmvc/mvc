using LogInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
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
            strSql.Append("Date,Thread,IP,Logger,EventName,Level,Message,Exception)");
            strSql.Append(" values (");
            strSql.Append("@Date,@Thread,@IP,@Logger,@EventName,@Level,@Message,@Exception)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@Date", MySqlDbType.DateTime),
					new MySqlParameter("@Thread", MySqlDbType.VarChar,200),
					new MySqlParameter("@IP", MySqlDbType.VarChar,50),
					new MySqlParameter("@Logger", MySqlDbType.VarChar,200),
					new MySqlParameter("@EventName", MySqlDbType.VarChar,200),
					new MySqlParameter("@Level", MySqlDbType.VarChar,50),
					new MySqlParameter("@Message", MySqlDbType.Text),
					new MySqlParameter("@Exception", MySqlDbType.Text)};

            parameters[0].Value = date;
            parameters[1].Value = thread;
            parameters[2].Value = ip;
            parameters[3].Value = recorder;
            parameters[4].Value = _event;
            parameters[5].Value = level;
            parameters[6].Value = message;
            parameters[7].Value = System.DBNull.Value;

            int rows = MySQLAccess.mySqlHelper.Instance.ExecuteNonQuery(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CreateSubmeterTable(string SubmeterName)
        {
            string sql = "SELECT * FROM LOG_Table WHERE Submeter='" + SubmeterName+"'";
            System.Data.DataSet ds = MySQLAccess.mySqlHelper.Instance.ExecuteDataset(sql);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count==0)
            {
                sql = "DROP TABLE IF EXISTS `" + SubmeterName + "`;CREATE TABLE IF NOT EXISTS `" + SubmeterName + "` ( `ID` int(10) NOT NULL AUTO_INCREMENT, `Date` datetime DEFAULT NULL, `Thread` varchar(200) DEFAULT NULL, `IP` varchar(50) DEFAULT NULL,`Logger` varchar(200) DEFAULT NULL,`EventName` varchar(200) DEFAULT NULL,`Level` varchar(50) DEFAULT NULL, `Message` text,`Exception` text, PRIMARY KEY (`ID`)) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT";
                MySQLAccess.mySqlHelper.Instance.ExecuteNonQuery(sql);
                sql = "SELECT Submeter FROM LOG_Table";
                string str = "";
                ds = MySQLAccess.mySqlHelper.Instance.ExecuteDataset(sql);
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        str += dt.Rows[i]["Submeter"].ToString() + ",";
                    }
                }
                str += SubmeterName;
                sql = "DROP TABLE IF EXISTS `LOG`;CREATE TABLE IF NOT EXISTS `LOG` ( `ID` int(10) NOT NULL AUTO_INCREMENT,`Date` datetime DEFAULT NULL,`Thread` varchar(200) DEFAULT NULL, `IP` varchar(50) DEFAULT NULL, `Logger` varchar(200) DEFAULT NULL,`EventName` varchar(200) DEFAULT NULL, `Level` varchar(50) DEFAULT NULL, `Message` text,`Exception` text, KEY `ID` (`ID`)) ENGINE=MRG_MyISAM DEFAULT CHARSET=utf8 INSERT_METHOD=LAST UNION=(" + str + ")";
                MySQLAccess.mySqlHelper.Instance.ExecuteNonQuery(sql);
                sql = "INSERT INTO LOG_Table (Submeter) VALUES('" + SubmeterName + "')";
                MySQLAccess.mySqlHelper.Instance.ExecuteNonQuery(sql);
            }
        }

        public static List<LogMessage> GetLogs(string Level, string Recorder, string Event, string BeginTime, string EndTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from LOG ");
            if (string.IsNullOrEmpty(BeginTime) || string.IsNullOrEmpty(EndTime))
            {
                return new List<LogMessage>();
            }
            if (DateTime.Compare(DateTime.Parse(BeginTime), DateTime.Parse(EndTime)) > 0)
            {
                return new List<LogMessage>();
            }
            strSql.Append(" where Date>=@BeginTime and Date<=@EndTime and (1=1 or Level=@Level or Logger=@Logger or EventName=@EventName) ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@BeginTime", MySqlDbType.DateTime),
                    new MySqlParameter("@EndTime", MySqlDbType.DateTime),
                    new MySqlParameter("@Logger", MySqlDbType.VarChar,200),
					new MySqlParameter("@EventName", MySqlDbType.VarChar,200),
					new MySqlParameter("@Level", MySqlDbType.VarChar,50)};
            parameters[0].Value = BeginTime;
            parameters[1].Value = EndTime;
            parameters[2].Value = Recorder;
            parameters[3].Value = Event;
            parameters[4].Value = Level;
            if (!string.IsNullOrEmpty(Level))
            {
                strSql.Append(" and Level=@Level");
            }
            else
            {
                parameters[4].Value = "1";
            }
            if (!string.IsNullOrEmpty(Recorder))
            {
                strSql.Append(" and Logger=@Logger");
            }
            else
            {
                parameters[2].Value = "1";
            }
            if (!string.IsNullOrEmpty(Event))
            {
                strSql.Append(" and EventName=@EventName");
            }
            else
            {
                parameters[3].Value = "1";
            }

            DataSet ds = MySQLAccess.mySqlHelper.Instance.ExecuteDataset(strSql.ToString(), parameters);
            List<LogMessage> logs = new List<LogMessage>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    logs.Add(DataRowToModel(row));
                }
            }
            return logs;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        static LogMessage DataRowToModel(DataRow row)
        {
            LogMessage model = new LogMessage();
            if (row != null)
            {
                if (row["Date"] != null && row["Date"].ToString() != "")
                {
                    model.Date = DateTime.Parse(row["Date"].ToString());
                }
                if (row["Thread"] != null)
                {
                    model.Thread = row["Thread"].ToString();
                }
                if (row["IP"] != null)
                {
                    model.Ip = row["IP"].ToString();
                }
                if (row["Logger"] != null)
                {
                    model.Recorder = row["Logger"].ToString();
                }
                if (row["EventName"] != null)
                {
                    model.Event = row["EventName"].ToString();
                }
                if (row["Level"] != null)
                {
                    model.Level = row["Level"].ToString();
                }
                if (row["Message"] != null)
                {
                    model.Message = row["Message"].ToString();
                }
            }
            return model;
        }
    }
}
