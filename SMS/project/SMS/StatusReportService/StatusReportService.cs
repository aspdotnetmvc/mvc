using LogClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatusReportService
{
    public class StatusReportService : MarshalByRefObject, IStatusReportService
    {
        public SMS.Model.RPCResult<List<SMS.Model.MOSMS>> GetMOSmsByAccountID(string accountID)
        {
            var list = MOCache.Instance.GetMOFromCache(accountID);
            try
            {

                if (list != null && list.Count > 0)
                {
                    var list2 = new List<MOSMS>();
                    int max = 100;
                    int i = 0;
                    lock (MOCache.locker)
                    {
                        foreach (var mo in list)
                        {
                            list2.Add(mo);
                            if (i >= max)
                            {
                                break;
                            }
                            i++;
                        }
                        if (list2.Count == 0)
                        {
                            return new RPCResult<List<MOSMS>>(false, list2, "没有上行短信");
                        }
                        else
                        {
                            foreach (var mo in list2)
                            {
                                MOCache.Instance.UpdateMOStatus(mo);
                            }
                        }
                    }

                    return new RPCResult<List<MOSMS>>(true, list2, "");
                }
                else
                {
                    return new RPCResult<List<MOSMS>>(false, null, "没有上行短信");
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError("StatusReport", "StatusReportService.GetCustomStatusReport", ex.ToString());
                return new RPCResult<List<MOSMS>>(false, null, "获取上行短信出现异常");
            }
        }

        public SMS.Model.RPCResult<List<SMS.Model.StatusReport>> GetCustomStatusReportBySMSID(string accountID, string SMSID)
        {
            var list = StatusReportCache.Instance.GetStatusReportFromCache(accountID);
            try
            {
                if (list != null && list.Count > 0)
                {
                    var list2 = new List<StatusReport>();
                    lock (StatusReportCache.locker)
                    {
                        list = list.Where(sr => sr.SMSID == SMSID).ToList();

                        int max = 1000;
                        int i = 0;

                        foreach (var sr in list)
                        {
                            list2.Add(sr);
                            if (i >= max)
                            {
                                break;
                            }
                            i++;
                        }
                        if (list2.Count == 0)
                        {
                            return new RPCResult<List<StatusReport>>(false, list2, "没有状态报告");
                        }
                        else
                        {
                            foreach (var sr in list2)
                            {
                                StatusReportCache.Instance.UpdateStatusReportCache(sr);
                            }
                        }
                    }
                    return new RPCResult<List<StatusReport>>(true, list2, "");

                }
                else
                {
                    return new RPCResult<List<StatusReport>>(true, null, "");
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError("StatusReport", "GetCustomStatusReportBySerialNumber", ex.ToString());
                return new RPCResult<List<StatusReport>>(false, null, "获取短信报告出现异常");
            }
        }

        public SMS.Model.RPCResult<List<SMS.Model.StatusReport>> GetCustomStatusReportByAccount(string accountID)
        {
            var list = StatusReportCache.Instance.GetStatusReportFromCache(accountID);
            try
            {
                if (list != null && list.Count > 0)
                {
                    var list2 = new List<StatusReport>();
                    int max = 1000;
                    int i = 0;
                    lock (StatusReportCache.locker)
                    {
                        foreach (var sr in list)
                        {
                            list2.Add(sr);
                            if (i >= max)
                            {
                                break;
                            }
                            i++;
                        }
                        if (list2.Count == 0)
                        {
                            return new RPCResult<List<StatusReport>>(false, list2, "没有状态报告");
                        }
                        else
                        {
                            foreach (var sr in list2)
                            {
                                StatusReportCache.Instance.UpdateStatusReportCache(sr);
                            }
                        }
                    }
                    return new RPCResult<List<StatusReport>>(true, list2, "");
                }
                else
                {
                    return new RPCResult<List<StatusReport>>(false, null, "没有状态报告");
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError("StatusReport", "StatusReportService.GetCustomStatusReport", ex.ToString());
                return new RPCResult<List<StatusReport>>(false, null, "获取短信报告出现异常");
            }
        }
    }
}
