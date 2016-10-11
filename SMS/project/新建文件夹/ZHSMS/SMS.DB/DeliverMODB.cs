using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMS.DB
{
    public class DeliverMODB
    {
        public static bool Add(MOSMS mo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into DeliverMO(");
            strSql.Append("SPNumber,UserNumber,ReceiveTime,SerialNumber,Gateway,MessageContent,Service)");
            strSql.Append(" values (");
            strSql.Append("@SPNumber,@UserNumber,@ReceiveTime,@Serial,@Gateway,@Message,@Service)");
            DBHelper.Instance.Execute(strSql.ToString(), mo);
            return true;
        }

        public static bool Del(string spNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from DeliverMO ");
            strSql.Append(" where SPNumber=@SPNumber");
            DBHelper.Instance.Execute(strSql.ToString(), new { SPNumber = spNumber });
            return true;
        }

        public static List<MOSMS> Gets(string spNumber, DateTime beginTime, DateTime endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select SPNumber,UserNumber,ReceiveTime,SerialNumber as Serial,Gateway,MessageContent as Message,Service from DeliverMO ");
            strSql.Append(" where SPNumber=@SPNumber and ReceiveTime>=@BeginTime and ReceiveTime<=@EndTime");
            return DBHelper.Instance.Query<MOSMS>(strSql.ToString(), new { SPNumber = spNumber, BeginTime=beginTime,EndTime=endTime });
        }
    }
}
