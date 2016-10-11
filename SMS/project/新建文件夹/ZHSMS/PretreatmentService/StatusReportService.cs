using LogClient;
using SMS.DB;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMSService
{
    public partial class SMSService : MarshalByRefObject, ISMS
    {
        #region 管理平台使用
        /// <summary>
        /// 短信状态报告明细（数据库获取）
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="sendTime"></param>
        /// <returns></returns>
        public RPCResult<List<StatusReport>> GetSMSStatusReport(string smsid, DateTime sendTime)
        {
            try
            {
                if (sendTime.ToString("yyyyMMdd") == "00010101")
                {
                    return new RPCResult<List<StatusReport>>(true, null, "");
                }

                var list = StatusReportDB.GetStatusReport(smsid, sendTime);

                if (list.Count == 0)
                {
                    return new RPCResult<List<StatusReport>>(false, list, "没有短信状态报告");
                }
                return new RPCResult<List<StatusReport>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("StatusReport", "StatusReportService.GetSMSStatusReport", ex.ToString());
                return new RPCResult<List<StatusReport>>(false, null, "获取短信报告出现异常");
            }
        }
        /// <summary>
        /// 查看短信统计报告
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public RPCResult<ReportStatistics> GetReportStatistics(string smsid, DateTime sendTime)
        {
            try
            {
                var rs = ReportStatisticsDB.GetReportStatistics(smsid, sendTime);
                return new RPCResult<ReportStatistics>(true, rs, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("StatusReport", "StatusReportService.GetReportStatistics", ex.ToString());
                return new RPCResult<ReportStatistics>(false, null, "获取统计报告失败");
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
            //List<Guid> serialNumbers = CacheStatisticsReport.Instance.GetSerialNumberByAccount(account);
            //try
            //{
            //    int max = 100;
            //    List<StatusReport> sr = new List<StatusReport>();
            //    foreach (Guid serialNumber in serialNumbers)
            //    {
            //        List<string> list = CacheCustomReport.Instance.Get(serialNumber);
            //        if (list.Count == 0) continue;
            //        if (max <= 0) break;

            //        if (list.Count > max)
            //        {
            //            for (int i = max - 1; i >= 0; i--)
            //            {
            //                sr.Add(CacheStatusReport.Instance.Get(list[i]));
            //                CacheCustomReport.Instance.Del(serialNumber, list[i]);
            //            }
            //            break;
            //        }
            //        else
            //        {
            //            max = max - list.Count;
            //            for (int i = 0; i < list.Count; i++)
            //            {
            //                sr.Add(CacheStatusReport.Instance.Get(list[i]));
            //                CacheCustomReport.Instance.Del(serialNumber, list[i]);
            //                i--;
            //            }
            //        }
            //    }
            //    return new RPCResult<List<StatusReport>>(true, sr, "");
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.LogError("StatusReport", "StatusReportService.GetCustomStatusReport", ex.ToString());
            //    return new RPCResult<List<StatusReport>>(false, null, "获取短信报告出现异常");
            //}
        }
        /// <summary>
        /// 客户状态报告明细(缓存获取，获取后缓存删除)
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public RPCResult<List<StatusReport>> GetCustomStatusReport(Guid serialNumber)
        {
            throw new NotImplementedException();
            //try
            //{
            //    List<StatusReport> sr = new List<StatusReport>();
            //    List<string> list = CacheCustomReport.Instance.Get(serialNumber);
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        sr.Add(CacheStatusReport.Instance.Get(list[i]));
            //        CacheCustomReport.Instance.Del(serialNumber, list[i]);
            //        i--;
            //    }
            //    return new RPCResult<List<StatusReport>>(true, sr, "");
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.LogError("StatusReport", "StatusReportService.GetCustomStatusReport", ex.ToString());
            //    return new RPCResult<List<StatusReport>>(false, null, "获取短信报告出现异常");
            //}
        }
        /// <summary>
        /// 获取用户上行短信(缓存获取，每次最多100条，获取后缓存删除)
        /// </summary>
        /// <param name="spNumber"></param>
        /// <returns></returns>
        public RPCResult<List<MOSMS>> GetMOSmsBySPNumber(string accountID, string spNumber)
        {
            throw new NotImplementedException();
            //try
            //{
            //    List<MOSMS> mo = CacheMOSMS.Instance.Get(spNumber);

            //    if (mo.Count > 100)
            //    {
            //        for (int i = 0; i < 100; i++)
            //        {
            //            CacheMOSMS.Instance.Del(mo[i].Serial);
            //        }
            //        for (int i = 100; i < mo.Count; i++)
            //        {
            //            mo.Remove(mo[i]);
            //        }
            //        return new RPCResult<List<MOSMS>>(true, mo, "");
            //    }

            //    for (int i = 0; i < mo.Count; i++)
            //    {
            //        CacheMOSMS.Instance.Del(mo[i].Serial);
            //    }

            //    return new RPCResult<List<MOSMS>>(true, mo, "");
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.LogError("StatusReport", "StatusReportService.GetMOSmsBySPNumber", ex.ToString());
            //    return new RPCResult<List<MOSMS>>(false, null, "获取短信报告出现异常");
            //}
        }

        #endregion


        public RPCResult<List<ReportStatistics>> GetStatisticsReportByAccount(string account, DateTime beginTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public RPCResult<List<ReportStatistics>> GetStatisticsReportAllByAccount(string account, DateTime beginTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public RPCResult<List<StatusReport>> GetCustomStatusReportBySerialNumber(string serialNumber)
        {
            throw new NotImplementedException();
        }

        public RPCResult<List<StatusReport>> GetCustomStatusReportByAccount(string accountID)
        {
            throw new NotImplementedException();
        }
    }
}
