using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchDogService
{
    public enum MonitoringProgramType
    {
        Service,
        Control,
        WinFrom,
    }

    public enum MonitoringMethodType
    {
        RemotingHeartbeat,
        Network,
        Process,
    }

    public class MonitoringProgram
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string StartArguments { get; set; }
        public MonitoringProgramType Type { get; set; }
        public MonitoringMethodType MonitoringMethod { get; set; }
        public int MonitoringTime { get; set; }
        public string MonitoringData { get; set; }
    }
}
