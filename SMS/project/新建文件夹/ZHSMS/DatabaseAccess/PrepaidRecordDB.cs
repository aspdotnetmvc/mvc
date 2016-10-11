using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DatabaseAccess
{
    public class PrepaidRecordDB
    {
        public static bool Add(string operatorAccount, string accountID, uint quantity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into PrepaidRecord(");
            strSql.Append("OperatorAccount,PrepaidAccount,Quantity,PrepaidTime)");
            strSql.Append(" values (");
            strSql.Append("@OperatorAccount,@PrepaidAccount,@Quantity,Now())");
            DBHelper.Instance.Execute(strSql.ToString(), new { OperatorAccount = operatorAccount, PrepaidAccount = accountID, Quantity = quantity });
            return true;
        }

        public static List<PrepaidRecord> Get(DateTime beginTime, DateTime endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select OperatorAccount as AccountID,PrepaidAccount,Quantity,PrepaidTime from PrepaidRecord ");
            strSql.Append(" where PrepaidTime>=@BeginTime and PrepaidTime<=@EndTime");
            return DBHelper.Instance.Query<PrepaidRecord>(strSql.ToString(), new { BeginTime = beginTime, EndTime = endTime });
        
        }
    }
}
