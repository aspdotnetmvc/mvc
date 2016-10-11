using Newtonsoft.Json;
using SMS.DB;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StatusReportService
{
    public class StatusReportCache
    {
        public static object locker = new object();
        private Dictionary<string, List<StatusReport>> Cache= new Dictionary<string,List<StatusReport>>();
        private static StatusReportCache _instance = null;
        public static StatusReportCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StatusReportCache();
                    Task t = new Task(() =>
                    {  
                        Thread.Sleep(1000 * 3600 * 4);
                        _instance.RemoveExpireCache();
                      
                    });
                    t.Start();
                }
                return _instance;
            }
        }
        /// <summary>
        /// 移除过期缓存
        /// </summary>
        private  void RemoveExpireCache()
        {
            foreach (var key in Cache.Keys)
            {
                var list = new List<StatusReport>();
                foreach (var sr in Cache[key])
                {
                    if (sr.SendTime < DateTime.Now.AddDays(-2))
                    {
                        list.Add(sr);
                    }
                }
                foreach (var sr in list)
                {
                    Cache[key].Remove(sr);
                }
            }
        }

        public void LoadStatusReportCache()
        {
            var list = StatusReportDB.GetStatusReportCache();
            lock (locker)
            {
                list.ForEach(sr =>
                {
                    if (!Cache.ContainsKey(sr.AccountID))
                    {
                        Cache.Add(sr.AccountID, new List<StatusReport>() { sr });
                    }
                    else
                    {
                        Cache[sr.AccountID].Add(sr);
                    }
                });
            }
        }
         
        public void AddStatusReportCache(StatusReport sr)
        {
            lock (locker)
            {
                if (!Cache.ContainsKey(sr.AccountID))
                {
                    Cache.Add(sr.AccountID, new List<StatusReport>() { sr });
                }
                else
                {
                    Cache[sr.AccountID].Add(sr);
                }
            }
        }


        public List<StatusReport> GetStatusReportFromCache(string AccountId)
        {
            if (!Cache.ContainsKey(AccountId))
            {
                return null;
            }
            else
            {
                return Cache[AccountId];
            }
        }
        /// <summary>
        /// 更新状态报告(客户端已获取）
        /// </summary>
        /// <param name="sr"></param>
        public void UpdateStatusReportCache(StatusReport sr)
        {
            try
            {
                StatusReportDB.UpdateStatusReportType(sr);
                Cache[sr.AccountID].Remove(sr);
            }
            catch (Exception ex)
            {
                LogClient.LogHelper.LogError("SatatusReportCache", "UpdateStatusReportCache", ex.ToString());
            }
        }
    }
}
