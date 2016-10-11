using LogClient;
using SMS.DB;
using SMS.Model;
using SMSInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMSService
{
    public partial class SMSService : MarshalByRefObject, ISMSService
    {
        #region 管理平台使用
        /// <summary>
        /// 短信状态报告明细（数据库获取）
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="sendTime"></param>
        /// <returns></returns>
        public RPCResult<QueryResult<StatusReport>> GetStatusReportBySMSID(QueryParams qp)
        {
            try
            {
                var list = StatusReportDB.GetStatusReport(qp);

                if (list.Total == 0)
                {
                    return new RPCResult<QueryResult<StatusReport>>(false, list, "没有短信状态报告");
                }
                return new RPCResult<QueryResult<StatusReport>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("StatusReport", "StatusReportService.GetSMSStatusReport", ex.ToString());
                return new RPCResult<QueryResult<StatusReport>>(false, null, "获取短信报告出现异常");
            }
        }


        /// <summary>
        /// 用户上行短信（数据库获取）
        /// </summary>
        /// <param name="spNumber"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public RPCResult<List<MOSMS>> GetMOSMS(string spNumber, DateTime beginTime, DateTime endTime)
        {
            try
            {
                if (DateTime.Compare(beginTime, endTime) > 0)
                {
                    DateTime dt = beginTime;
                    beginTime = endTime;
                    endTime = dt;
                }
                if (DateTime.Compare(endTime, DateTime.Now) > 0)
                {
                    endTime = DateTime.Now;
                }
                List<MOSMS> mo = DeliverMODB.Gets(spNumber, beginTime, endTime);
                return new RPCResult<List<MOSMS>>(true, mo, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("StatusReport", "StatusReportService.GetMOSMS", ex.ToString());
                return new RPCResult<List<MOSMS>>(false, null, "获取短信报告出现异常");
            }
        }


        /// <summary>
        /// 获取全部账号统计报告ISMP 使用
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public RPCResult<List<ReportStatistics>> GetStatisticsReportAll(DateTime beginTime, DateTime endTime)
        {
            DateTime cacheBeginTime = DateTime.MaxValue;

            if (DateTime.Compare(beginTime, endTime) > 0)
            {
                DateTime dt = beginTime;
                beginTime = endTime;
                endTime = dt;
            }
            if (DateTime.Compare(endTime, DateTime.Now) > 0)
            {
                endTime = DateTime.Now;
            }

            List<string> list = ReportStatisticsDB.GetTables();
            var maxdate = list.Max(s => DateTime.Parse(s.Remove(0, 17).Insert(4, "-").Insert(7, "-")));

            if (DateTime.Compare(endTime, maxdate) > 0)
            {
                endTime = maxdate;
            }
            var mindate = list.Min(s => DateTime.Parse(s.Remove(0, 17).Insert(4, "-").Insert(7, "-")));

            if (DateTime.Compare(beginTime, mindate) < 0)
            {
                beginTime = mindate;
            }
            List<ReportStatistics> dbRS = ReportStatisticsDB.GetStatisticsByDate(beginTime, endTime);

            return new RPCResult<List<ReportStatistics>>(true, dbRS, "");
        }

        #endregion

        #region 外部使用的接口
        /// <summary>
        /// 客户状态报告明细(缓存获取，每次最多100条，获取后缓存删除)
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public RPCResult<List<StatusReport>> GetCustomStatusReport(string account)
        {
            throw new NotImplementedException();
         
        }
        
        /// <summary>
        /// 获取用户上行短信 
        /// </summary>
        /// <param name="spNumber"></param>
        /// <returns></returns>
        public RPCResult<List<MOSMS>> GetMOSmsByAccountID(string accountID)
        {
            return StatusProxy.GetStatusReportService().GetMOSmsByAccountID(accountID);
        }

        #endregion

        /// <summary>
        /// 获取状态报告
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public RPCResult<List<ReportStatistics>> GetStatisticsReportByAccount(string accountID, DateTime beginTime, DateTime endTime)
        {
            var r =  ReportStatisticsDB.GetStatisticsByAccount(accountID, beginTime, endTime);
            return new RPCResult<List<ReportStatistics>>(true, r, "");
        }

        public RPCResult<List<ReportStatistics>> GetStatisticsReportAllByAccount(string account, DateTime beginTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public RPCResult<List<StatusReport>> GetCustomStatusReportBySMSID(string accountID,string SMSID)
        {
            return StatusProxy.GetStatusReportService().GetCustomStatusReportBySMSID(accountID, SMSID);
        }

        public RPCResult<List<StatusReport>> GetCustomStatusReportByAccount(string accountID)
        {
            return StatusProxy.GetStatusReportService().GetCustomStatusReportByAccount(accountID);
        }
    }
}
