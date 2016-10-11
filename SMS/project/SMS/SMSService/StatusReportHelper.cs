using SMS.DB;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSService
{
    public class StatusReportHelper
    {
        private Dictionary<string, List<StatusReport>> StatusReportCache;
        private StatusReportHelper() { StatusReportCache = new Dictionary<string, List<StatusReport>>(); }
        private StatusReportHelper _instance;
        public StatusReportHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StatusReportHelper();
                }
                return _instance;
            }
        }

        public void LoadStatusReportCache()
        {
            var list = StatusReportDB.GetStatusReportCache();
            list.ForEach(sr => {
                if (!StatusReportCache.ContainsKey(sr.AccountID))
                {
                    StatusReportCache.Add(sr.AccountID, new List<StatusReport>() { sr});
                }
                else
                {
                    StatusReportCache[sr.AccountID].Add(sr);
                }
            });
        }
    }
}
