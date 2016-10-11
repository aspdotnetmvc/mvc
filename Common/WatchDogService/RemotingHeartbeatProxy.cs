using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Monitoring;

namespace WatchDogService
{
    public class RemotingHeartbeatProxy
    {
        static Dictionary<string, IMonitoring> monitorings = new Dictionary<string, IMonitoring>();

        public static bool GetHeartbeat(string remotingName)
        {
            IMonitoring monitoring;
            monitorings.TryGetValue(remotingName, out monitoring);

            if (monitoring == null)
            {
                monitoring = (IMonitoring)Activator.GetObject(typeof(IMonitoring), remotingName);
                monitorings.Add(remotingName,monitoring);
            }
            try
            {
                monitoring.Heartbeat();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
