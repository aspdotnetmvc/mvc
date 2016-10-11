using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMSModel;
using DBUtility;
using System.Data;

namespace DAL
{
    public class MO
    {
        public static bool Add(MOSMS mo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into DeliverMO(");
            strSql.Append("SPNumber,UserNumber,ReceiveTime,Serial,Gateway,MessageContent,Service)");
            strSql.Append(" values (");
            strSql.Append("@SPNumber,@UserNumber,@ReceiveTime,@Serial,@Gateway,@MessageContent,@Service)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@SPNumber", MySqlDbType.VarChar,32),
					new MySqlParameter("@UserNumber", MySqlDbType.VarChar,32),
					new MySqlParameter("@ReceiveTime", MySqlDbType.DateTime),
					new MySqlParameter("@Serial", MySqlDbType.VarChar,64),
					new MySqlParameter("@Gateway", MySqlDbType.VarChar,64),
					new MySqlParameter("@MessageContent", MySqlDbType.Text),
					new MySqlParameter("@Service", MySqlDbType.VarChar,256)};
            parameters[0].Value = mo.SPNumber;
            parameters[1].Value = mo.UserNumber;
            parameters[2].Value = mo.ReceiveTime;
            parameters[3].Value = mo.Serial;
            parameters[4].Value = mo.Gateway;
            parameters[5].Value = mo.Message;
            parameters[6].Value = mo.Service;
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

        public static List<MOSMS> Gets(string spNumber, DateTime beginTime, DateTime endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from DeliverMO ");
            strSql.Append(" where SPNumber=@SPNumber and ReceiveTime>=@BeginTime and ReceiveTime<=@EndTime");
            MySqlParameter[] parameters = {
					new MySqlParameter("@SPNumber", MySqlDbType.VarChar,32),
                    new MySqlParameter("@BeginTime", MySqlDbType.DateTime),
                    new MySqlParameter("@EndTime", MySqlDbType.DateTime)
			};
            parameters[0].Value = spNumber;
            parameters[1].Value = beginTime;
            parameters[2].Value = endTime;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            List<MOSMS> list = new List<MOSMS>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(DataRowToMOSMS(row));
                }
            }
            return list;
        }
        static MOSMS DataRowToMOSMS(DataRow row)
        {
            string gateway = "";
            string serial = "";
            DateTime receiveTime = DateTime.MinValue;
            string message = "";
            string userNumber = "";
            string spNumber = "";
            string service = "";

            if (row != null)
            {
                if (row["SPNumber"] != null)
                {
                    spNumber = row["SPNumber"].ToString();
                }
                if (row["UserNumber"] != null)
                {
                    userNumber = row["UserNumber"].ToString();
                }
                if (row["ReceiveTime"] != null && row["ReceiveTime"].ToString() != "")
                {
                    receiveTime = DateTime.Parse(row["ReceiveTime"].ToString());
                }
                if (row["Serial"] != null)
                {
                    serial = row["Serial"].ToString();
                }
                if (row["Gateway"] != null)
                {
                    gateway = row["Gateway"].ToString();
                }
                if (row["MessageContent"] != null)
                {
                    message = row["MessageContent"].ToString();
                }
                if (row["Service"] != null)
                {
                    service = row["Service"].ToString();
                }
            }
            MOSMS model = new MOSMS(gateway, serial, receiveTime, message, userNumber, spNumber, service);
            return model;
        }
    }
}
