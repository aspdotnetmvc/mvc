
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Monitoring;

namespace ZabbixAgentExtend
{
    internal class TestServer
    {
        internal static bool TestHeartbeat(IMonitoring server, int wait)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            bool err = false;

            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    server.Heartbeat();
                }
                catch
                {
                    err = true;
                }
            }, token);

            if (!t.Wait(wait, tokenSource.Token))
            {
                tokenSource.Cancel();
                return false;
            }
            if (err) return false;
            return true;
        }
    }
}
