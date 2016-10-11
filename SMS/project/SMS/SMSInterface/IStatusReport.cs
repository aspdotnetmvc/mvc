using SMS.Model;
using SMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSInterface
{
    public partial interface ISMSService
    {
        #region 管理平台
        /// <summary>
        /// 短信状态报告明细（缓存无，数据库获取）
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="sendTime"></param>
        /// <returns></returns>
        RPCResult<QueryResult<StatusReport>> GetStatusReportBySMSID(QueryParams qp);
        /// <summary>
        /// 获取MO短信（数据库获取）
        /// </summary>
        /// <param name="spNumber"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        RPCResult<List<MOSMS>> GetMOSMS(string spNumber, DateTime beginTime, DateTime endTime);
        /// <summary>
        /// 获取账号统计报告（数据库获取，缓存忽略，直客端使用）
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        RPCResult<List<ReportStatistics>> GetStatisticsReportByAccount(string account, DateTime beginTime, DateTime endTime);
        /// <summary>
        /// 获取账号统计报告(数据库和缓存获取，管理平台使用)
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        RPCResult<List<ReportStatistics>> GetStatisticsReportAllByAccount(string account, DateTime beginTime, DateTime endTime);

        /// <summary>
        /// 获取全部账号统计报告(数据库和缓存获取，管理平台使用)
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        RPCResult<List<ReportStatistics>> GetStatisticsReportAll(DateTime beginTime, DateTime endTime);
 
        #endregion

        #region 提供外部接口
        /// <summary>
        /// 获取用户上行短信(缓存获取，每次最多100条，获取后缓存删除)
        /// </summary>
        /// <param name="spNumber"></param>
        /// <returns></returns>
        RPCResult<List<MOSMS>> GetMOSmsByAccountID(string accountID);
        /// <summary>
        /// 客户状态报告明细
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        RPCResult<List<StatusReport>> GetCustomStatusReportBySMSID(string accountID,string SMSID);
        /// <summary>
        /// 客户状态报告明细（缓存获取，每次最多100条，获取后缓存删除）
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        RPCResult<List<StatusReport>> GetCustomStatusReportByAccount(string accountID);
        #endregion
    }
}
