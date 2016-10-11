using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using Monitoring;

namespace ZabbixAgentExtend
{
    public class MonitoringProxy
    {
        private static Dictionary<string, IMonitoring> ms = new Dictionary<string, IMonitoring>();
        private volatile static MonitoringProxy mcp = null;
        private static object lockHelper = new object();

        private MonitoringProxy()
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;

            for (int i = 0; i < appSettings.Count; i++)
            {
                ms.Add(appSettings.GetKey(i), (IMonitoring)Activator.GetObject(typeof(IMonitoring), appSettings[i]));
            }
        }

        internal static MonitoringProxy Instance
        {
            get
            {
                if (mcp == null)
                {
                    lock (lockHelper)
                    {
                        if (mcp == null)
                        {
                            return new MonitoringProxy();
                        }
                    }
                }
                return mcp;
            }
        }

        public bool Heartbeat(string server)
        {
            if (ms.ContainsKey(server))
            {
                return TestServer.TestHeartbeat(ms[server], 3000);
            }
            return false;
        }

    }
}
