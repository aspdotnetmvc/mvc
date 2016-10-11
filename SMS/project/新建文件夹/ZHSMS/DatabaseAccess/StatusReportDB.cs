using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DatabaseAccess
{
    public class StatusReportDB
    {
        public static bool AddStatusReport(StatusReport report)
        {

            string tableName = "SMSStatusReport_" + report.SendTime.ToString("yyyyMMdd");
            string sql = @"insert into " + tableName + @"(SMSID,SerialNumber,Number,SendTime,Gateway,Channel,StatusCode,Succeed,Description,ResponseTime)
                       values(@SMSID,@SerialNumber,@Number,@SendTime,@Gateway,@Channel,@StatusCode,@Succeed,@Description,@ResponseTime)";

            DBHelper.Instance.Execute(sql, report);
            return true;
        }
        /// <summary>
        /// 更新状态报告
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public static bool Update(StatusReport report)
        {
            string tableName = "SMSStatusReport_" + report.SendTime.ToString("yyyyMMdd");

            string sql = @"update " + tableName + @" set Succeed=@Succeed,StatusCode=@StatusCode,Description=@Description,ResponseTime=@ResponseTime where SerialNumber=@SerialNumber";

            DBHelper.Instance.Execute(sql, report);
            return true;
        }

        /// <summary>
        /// 复制状态表的表结构
        /// </summary>
        /// <returns></returns>
        public static bool CloneTable(string tableName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("create table " + tableName + " like SMSStatusReport");
            DBHelper.Instance.Execute(strSql.ToString());
            return true;
        }
        /// <summary>
        /// 获取数据库所有状态报告的表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTables()
        {
            StringBuilder strSql = new StringBuilder();//create table aa like log
            strSql.Append("show tables like 'SMSStatusReport_%'");
            return DBHelper.Instance.Query<string>(strSql.ToString());
        }

        public static List<StatusReport> GetStatusReport(string smsid, DateTime sendTime)
        {
            string tableName = "SMSStatusReport_" + sendTime.ToString("yyyyMMdd");
            string sql ="select * from " + tableName+" where SMSID=@SMSID";
            var r = DBHelper.Instance.Query<StatusReport>(sql, new { SMSID = smsid });
            return r;
        }
    }
}
