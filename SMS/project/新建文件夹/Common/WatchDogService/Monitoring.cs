using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatchDogService
{
    public class Monitoring
    {
        MonitoringProgram mp;
        bool run = false;

        public Monitoring(MonitoringProgram mp)
        {
            this.mp = mp;
        }

        public void Start()
        {
            run = true;
            Thread th = new Thread(this.MonitoringProgram);
            th.Start();
        }

        public void Stop()
        {
            run = false;
        }

        private  void MonitoringProgram()
        {
            while (run)
            {
                switch (mp.MonitoringMethod)
                {
                    case MonitoringMethodType.RemotingHeartbeat:
                        if (!TestRemotingServer(1000))
                        {
                            ProcessProgram();
                        }
                        break;
                    case MonitoringMethodType.Process:
                        if (!TestProcess())
                        {
                            ProcessProgram();
                        }
                        break;
                    case MonitoringMethodType.Network:
                        break;
                }

                Thread.Sleep(mp.MonitoringTime);
            }
        }

        private void ProcessProgram()
        {
            switch (mp.Type)
            {
                case MonitoringProgramType.Control:
                    ProcessHelper.KillProcess(mp.Name);
                    ProcessHelper.StartProcess(mp.Name,mp.Path, mp.StartArguments);
                    break;
            }
        }

        private bool TestProcess()
        {
            return ProcessHelper.CheckProcessExists(mp.Path + mp.Name);
        }

        private bool TestNetwork()
        {
            return true;
        }

        private bool TestRemotingServer(int wait)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            bool err = false;
            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    err = !RemotingHeartbeatProxy.GetHeartbeat(mp.MonitoringData);
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
