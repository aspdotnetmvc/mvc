using DBTools;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMS.DB
{
    public class StatusReportDB
    {
        #region 状态报告
        public static bool AddStatusReport(StatusReport report)
        {
            IDbTransaction tran = DBHelper.Instance.GetDBAdapter().BeginTransaction();
            try
            {
                string tableName = "SMSStatusReport_" + report.SendTime.ToString("yyyyMMdd");
                string sql = @"insert into " + tableName + @"(SMSID,AccountID,StatusReportType,SerialNumber,Number,SendTime,Gateway,Channel,StatusCode,Succeed,Description,ResponseTime)
                       values(@SMSID,@AccountID,@StatusReportType,@SerialNumber,@Number,@SendTime,@Gateway,@Channel,@StatusCode,@Succeed,@Description,@ResponseTime)";

                DBHelper.Instance.Execute(sql, report, tran);

                if (report.Succeed)
                {
                    sql = @"update ReportStatistics_" + report.SendTime.ToString("yyyyMMdd") + @" set SendCount=ifnull(SendCount,0)+1 where ID=@SMSID";
                    DBHelper.Instance.Execute(sql, report, tran);
                }
                else
                {
                    sql = @"update ReportStatistics_" + report.SendTime.ToString("yyyyMMdd") + @" set SendCount=ifnull(SendCount,0)+1,
                            FailureCount=ifnull(FailureCount,0)+1,FeeBack=ifnull(FeeBack,0)+SplitNumber,FeeBackReason = '发送失败'
                            where ID=@SMSID";
                    DBHelper.Instance.Execute(sql, report, tran);
                    sql = @"update SMS set FailureCount=ifnull(FailureCount,0)+1,FeeBack=ifnull(FeeBack,0)+SplitNumber,FeeBackReason = '发送失败'
                            where ID=@SMSID";
                    DBHelper.Instance.Execute(sql, report, tran);
                    //返费
                    int Fee = DBHelper.Instance.ExecuteScalar<int>("select SplitNumber from SMS where ID=@SMSID", report, tran);
                    if (Fee > 0 && Fee < 10)
                    {
                        DBHelper.Instance.Execute("update Account set SMSNumber=SMSNumber+@Fee where AccountID=@AccountID", new { AccountID = report.AccountID, SMSID = report.SMSID, Fee = Fee }, tran);
                    }
                    else
                    {
                        throw new Exception("返费数值异常:" + Fee);
                    }
                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            finally
            {
                tran.Connection.Close();
            }
            return true;
        }
        /// <summary>
        /// 更新状态报告
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public static bool UpdateStatusReport(StatusReport report)
        {
            string tableName = "SMSStatusReport_" + report.SendTime.ToString("yyyyMMdd");

            IDbTransaction tran = DBHelper.Instance.GetDBAdapter().BeginTransaction();
            try
            {
                string sql = @"update " + tableName + @" set Succeed=@Succeed,StatusCode=@StatusCode,Description=@Description,ResponseTime=@ResponseTime where SerialNumber=@SerialNumber";

                DBHelper.Instance.Execute(sql, report, tran);

                if (!report.Succeed)
                {
                    sql = @"update ReportStatistics_" + report.SendTime.ToString("yyyyMMdd") + @" set SendCount=ifnull(SendCount,0)+1,
                            FailureCount=ifnull(FailureCount,0)+1,FeeBack=ifnull(FeeBack,0)+SplitNumber,FeeBackReason = '发送失败'
                            where ID=@SMSID";
                    DBHelper.Instance.Execute(sql, report, tran);
                    sql = @"update SMS set FailureCount=ifnull(FailureCount,0)+1,FeeBack=ifnull(FeeBack,0)+SplitNumber,FeeBackReason = '发送失败'
                            where ID=@SMSID";
                    DBHelper.Instance.Execute(sql, report, tran);

                    //返费
                    int Fee = DBHelper.Instance.ExecuteScalar<int>("select SplitNumber from SMS where ID=@SMSID", report, tran);
                    if (Fee > 0 && Fee < 10)
                    {
                        DBHelper.Instance.Execute("update Account set SMSNumber=SMSNumber+@Fee where AccountID=@AccountID", new { AccountID = report.AccountID, SMSID = report.SMSID, Fee = Fee }, tran);
                    }
                    else
                    {
                        throw new Exception("返费数值异常:" + Fee);
                    }
                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            finally
            {
                tran.Connection.Close();
            }
            return true;
        }
        #endregion

        #region  分表
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
        #endregion

        #region 状态报告
        //public static List<StatusReport> GetStatusReport(string smsid, DateTime sendTime)
        //{
        //    string tableName = "SMSStatusReport_" + sendTime.ToString("yyyyMMdd");
        //    string sql = "select * from " + tableName + " where SMSID=@SMSID";
        //    var r = DBHelper.Instance.Query<StatusReport>(sql, new { SMSID = smsid });
        //    return r;
        //}
        /// <summary>
        /// 必须包含SMSID SendTime, page,rows 四个参数
        /// 可选参数 Succeed,Number
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public static QueryResult<StatusReport> GetStatusReport(QueryParams qp)
        {
            string tableName = "SMSStatusReport_" + qp.get("SendTime").ToString();
            string sql = "select * from " + tableName + " where SMSID=@SMSID";    //添加各种查询条件
            ParamList pl = DBHelper.Instance.GetParamList(qp);
            if (pl.isnotnull("Succeed"))
            {
                sql += " and Succeed=@Succeed";
            }

            if (pl.isnotnull("Number"))
            {
                sql += " and Number=@Number";
            }
            var rs = DBHelper.Instance.GetResultSet<StatusReport>(sql, "SerialNumber", pl);
            return DBHelper.Instance.ToQueryResult(rs);
        }
        public static SMSMessage GetSMSForMO(string Gateway, DateTime sendTime, string number)
        {
            var smsid = GetSMSIDForMO(Gateway, sendTime, number);
            if (string.IsNullOrWhiteSpace(smsid))
            {
                smsid = GetSMSIDForMO(Gateway, sendTime.AddDays(-1), number);
            }
            if (string.IsNullOrWhiteSpace(smsid))
            {
                return null;
            }

            return SMSDAL.GetSMSMessageById(smsid);

        }
        private static string GetSMSIDForMO(string gateway, DateTime sendTime, string number)
        {
            string tableName = "SMSStatusReport_" + sendTime.ToString("yyyyMMdd");
            string sql = "select SMSID from " + tableName + " where Gateway = @Gateway and Number=@Number";
            return DBHelper.Instance.Query<string>(sql, new { Gateway = gateway, Number = number }).FirstOrDefault();
        }
        #endregion

        #region 状态报告缓存，用于第三方接口获取
        /// <summary>
        /// 状态报告2天内，第三方接口获取
        /// </summary>
        /// <returns></returns>
        public static List<StatusReport> GetStatusReportCache()
        {
            List<StatusReport> list = new List<StatusReport>();
            list.AddRange(GetStatusReportCache(DateTime.Now));
            list.AddRange(GetStatusReportCache(DateTime.Now.AddDays(-1)));
            list.AddRange(GetStatusReportCache(DateTime.Now.AddDays(-2)));
            return list;
        }
        private static List<StatusReport> GetStatusReportCache(DateTime time)
        {
            try
            {
                string tableName = "SMSStatusReport_" + time.ToString("yyyyMMdd");
                string sql = "select * from " + tableName + " where StatusReportType>0 and StatusReportType<10";
                var r = DBHelper.Instance.Query<StatusReport>(sql);
                return r;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        /// <summary>
        /// 更新客户获取状态
        /// </summary>
        /// <param name="sr"></param>
        public static void UpdateStatusReportType(StatusReport sr)
        {
            string tableName = "SMSStatusReport_" + sr.SendTime.ToString("yyyyMMdd");

            string sql = @"update " + tableName + @" set StatusReportType=@StatusReportType where SerialNumber=@SerialNumber";

            DBHelper.Instance.Execute(sql, new { SerialNumber = sr.SerialNumber, StatusReportType = StatusReportType.Finished });
        }
        #endregion
    }
}
