using Newtonsoft.Json;
using SMS.DB;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatusReportService
{
    public class MOCache
    {
        public static object locker = new object();
        private Dictionary<string, List<MOSMS>> Cache = new Dictionary<string, List<MOSMS>>();
        private static MOCache _instance = null;
        public static MOCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MOCache();
                }
                return _instance;
            }
        }

        public void LoadMOCache()
        {
            var list = DeliverMODB.GetMOCache();
            list.ForEach(mo =>
            {
                if (!string.IsNullOrWhiteSpace(mo.AccountID))
                {
                    if (!Cache.ContainsKey(mo.AccountID))
                    {
                        Cache.Add(mo.AccountID, new List<MOSMS>() { mo });
                    }
                    else
                    {
                        Cache[mo.AccountID].Add(mo);
                    }
                }
            });
        }

        public void AddMOCache(MOSMS mo)
        {
            if (string.IsNullOrWhiteSpace(mo.AccountID))
            {
                return;
            }
            lock (locker)
            {
                if (!Cache.ContainsKey(mo.AccountID))
                {
                    Cache.Add(mo.AccountID, new List<MOSMS>() { mo });
                }
                else
                {
                    Cache[mo.AccountID].Add(mo);
                }
            }
        }


        public List<MOSMS> GetMOFromCache(string AccountId)
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
        /// <param name="mo"></param>
        public void UpdateMOStatus(MOSMS mo)
        {
            try
            {
                DeliverMODB.UpdateMOStatus(mo);
                Cache[mo.AccountID].Remove(mo);
            }
            catch (Exception ex)
            {
                LogClient.LogHelper.LogError("MOCache", "UpdateMOStauts", ex.ToString());
            }
        }
    }
}
