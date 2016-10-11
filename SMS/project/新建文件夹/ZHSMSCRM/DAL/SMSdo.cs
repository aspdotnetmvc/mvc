using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMSModel;
using MySql.Data.MySqlClient;
using System.Data;

namespace DAL
{
    public class SMSdo
    {
        public static bool SMSAdd(SMS sms)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ReviewMT(");
            strSql.Append("SMSID,AccountID,Phones,SMSContent,SendTime,WapURL,SMSTimer)");
            strSql.Append(" values (");
            strSql.Append("@SMSID,@AccountID,@Phones,@SMSContent,@SendTime,@WapURL,@SMSTimer)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@SMSID", MySqlDbType.VarChar,64),
					new MySqlParameter("@AccountID", MySqlDbType.VarChar,64),
					new MySqlParameter("@Phones", MySqlDbType.Text),
					new MySqlParameter("@SMSContent", MySqlDbType.Text),
					new MySqlParameter("@SendTime", MySqlDbType.DateTime),
					new MySqlParameter("@WapURL", MySqlDbType.VarChar,1024),
                    new MySqlParameter("@SMSTimer",MySqlDbType.DateTime)};
            parameters[0].Value = sms.SerialNumber;
            parameters[1].Value = sms.Account;
            string str = "";
            foreach (string phone in sms.Number)
            {
                str += phone + ",";
            }
            str = str == "" ? "" : str.Substring(0, str.Length - 1);
            parameters[2].Value = str;
            parameters[3].Value = sms.Content;
            parameters[4].Value = sms.SendTime;
            parameters[5].Value = sms.WapURL;
            if ((sms.SMSTimer == null || DateTime.Compare(sms.SMSTimer, DateTime.MinValue) == 0))
            {
                parameters[6].Value = System.DBNull.Value;
            }
            else
            {
                parameters[6].Value = sms.SMSTimer;
            }
            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static DataTable GetSMSByAccount(string accountID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ReviewMT ");
            strSql.Append(" WHERE AccountID=@AccountID LIMIT 100");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountID", MySqlDbType.VarChar,64)			};
            parameters[0].Value = accountID;
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            return ds.Tables[0];
        }
        public static DataTable GetSMSByAccountAndSendTime(string accountID, DateTime start, DateTime end)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ReviewMT ");
            strSql.Append(" WHERE AccountID=@AccountID and SendTime between @start and @end ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountID", MySqlDbType.VarChar,64),
                    new MySqlParameter("@start", MySqlDbType.DateTime),
                    new MySqlParameter("@end", MySqlDbType.DateTime)
                                          };
            parameters[0].Value = accountID;
            parameters[1].Value = start;
            parameters[2].Value = end;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            return ds.Tables[0];
        }
    }
}
