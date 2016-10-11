using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SMS.Model;

namespace DatabaseAccess
{
    public class AuditRecordDB
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(AuditRecord record)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into AuditRecord(");
            strSql.Append("AccountID,SerialNumber,AuditTime,Result,SendTime,SMSContent)");
            strSql.Append(" values (");
            strSql.Append("@AccountID,@SerialNumber,@AuditTime,@Result,@SendTime,@Content)");
            DBHelper.Instance.Execute(strSql.ToString(), record);

            return true;

        }

        public static List<AuditRecord> GetAudit(DateTime beginTime, DateTime endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select AccountID,SerialNumber,AuditTime,Result,SendTime,SMSContent as Content from AuditRecord ");
            strSql.Append(" where AuditTime>=@BeginTime and AuditTime<=@EndTime order by AuditTime desc ");
            var l = DBHelper.Instance.Query(strSql.ToString(), new { BeginTime = beginTime, EndTime = endTime });

            return (from a in l select ToAuditRecord((object)a)).ToList();

        }
        public static AuditRecord GetAudit(Guid serialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select AccountID,SerialNumber,AuditTime,Result,SendTime,SMSContent as Content from AuditRecord ");
            strSql.Append(" where SerialNumber=@SerialNumber ");
            var r = DBHelper.Instance.Query(strSql.ToString(), new { SerialNumber = serialNumber }).FirstOrDefault();

            return ToAuditRecord((object)r);

        }
        public static AuditRecord ToAuditRecord(object r)
        {
            if (r == null) return null;
            var row = (dynamic)r;
            AuditRecord ar = new AuditRecord();
            ar.AccountID = row.AccountID;
            ar.SerialNumber = Guid.Parse(row.SerialNumber);

            ar.Result = row.Result > 0;
            ar.Content = row.Content;
            if (row.SendTime != null) ar.SendTime = row.SendTime;
            if (row.AuditTime != null) ar.AuditTime = row.AuditTime;
            return ar;
        }

    }

}