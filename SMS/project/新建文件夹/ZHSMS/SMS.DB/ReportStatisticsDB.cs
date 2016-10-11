using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMS.DB
{
    public class ReportStatisticsDB
    {
        public static bool AddSMSHistory(ReportStatistics sms)
        {
            string tableName = "ReportStatistics_" + sms.SendTime.Value.ToString("yyyyMMdd");
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + tableName + "(");
            string sql = @"insert into " + tableName + @"(ID,AccountID,SPNumber,Content,Signature,NumberCount,SendTime,SplitNumber,FeeTotalCount,AuditResult,AuditTime,AuditType,AuditAccountLoginName,Channel,SMSType,Status,Source)
                       values(@ID,@AccountID,@SPNumber,@Content,@Signature,@NumberCount,@SendTime,@SplitNumber,@FeeTotalCount,@AuditResult,@AuditTime,@AuditType,@AuditAccountLoginName,@Channel,@SMSType,@Status,@Source)";

            DBHelper.Instance.Execute(sql, sms);
            return true;
        }

        #region 短信统计报告
        /// <summary>
        /// 短信统计报告
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static List<ReportStatistics> GetStatisticsByAccount(string accountID, DateTime beginTime, DateTime endTime)
        {
            DateTime date = DateTime.Parse(beginTime.ToString("yyyy-MM-dd"));
            List<ReportStatistics> list = new List<ReportStatistics>();
            while (DateTime.Compare(endTime, date) >= 0)
            {
                string tableName = "ReportStatistics_" + date.ToString("yyyyMMdd");
                List<ReportStatistics> sms = GetStatisticsByAccount(accountID, tableName, beginTime, endTime);
                if (sms.Count > 0)
                {
                    list = list.Union(sms).ToList<ReportStatistics>();
                }
                date = date.AddDays(1);
            }
            return list;
        }
        static List<ReportStatistics> GetStatisticsByAccount(string accountID, string tableName, DateTime beginTime, DateTime endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + tableName);
            strSql.Append(" where AccountID=@AccountID and SendTime>=@BeginTime and SendTime<=@EndTime and SendCount!=0");
            var r = DBHelper.Instance.Query<ReportStatistics>(strSql.ToString(), new
            {
                AccountID = accountID,
                BeginTime = beginTime,
                EndTime = endTime
            });
            return r;
        }

        /// <summary>
        /// 按日期查询短信统计报告
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static List<ReportStatistics> GetStatisticsByDate(DateTime beginTime, DateTime endTime)
        {
            DateTime date = DateTime.Parse(beginTime.ToString("yyyy-MM-dd"));
            List<ReportStatistics> list = new List<ReportStatistics>();
            while (DateTime.Compare(endTime, date) >= 0)
            {
                List<ReportStatistics> sms = GetStatisticsByDate(date);
                if (sms.Count > 0)
                {
                    list = list.Union(sms).ToList<ReportStatistics>();
                }
                date = date.AddDays(1);
            }
            return list;
        }
        static List<ReportStatistics> GetStatisticsByDate(DateTime date)
        {
            string tableName = "ReportStatistics_" + date.ToString("yyyyMMdd");
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + tableName);
            strSql.Append(" where SendCount!=0");
            var r = DBHelper.Instance.Query<ReportStatistics>(strSql.ToString());
            return r;
        }


        #endregion


        #region

        public static ReportStatistics GetReportStatistics(string smsid, DateTime sendTime)
        {
            string tableName = "ReportStatistics_" + sendTime.ToString("yyyyMMdd");
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from " + tableName);
            strSql.Append(" where ID=@SMSID ");
            return DBHelper.Instance.Query<ReportStatistics>(strSql.ToString(), new
            {
                SMSID = smsid
            }).FirstOrDefault();
        }

        public static bool Update(string accountID, ReportStatistics statistics)
        {
            throw new NotImplementedException();
            return true;
        }

        /// <summary>
        /// 复制状态表的表结构
        /// </summary>
        /// <returns></returns>
        public static bool CloneTable(string tableName)
        {
            StringBuilder strSql = new StringBuilder();//create table aa like log
            strSql.Append("create table " + tableName + " like ReportStatistics");
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
            strSql.Append("show tables like 'ReportStatistics_%'");
            return DBHelper.Instance.Query<string>(strSql.ToString());
        } 
        #endregion
    }
}
