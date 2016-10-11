using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WatchDogService
{
    public partial class WatchDogService : ServiceBase
    {
        List<Monitoring> monitorings = new List<Monitoring>();

        public WatchDogService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            List<MonitoringProgram> mps = new List<MonitoringProgram>();

            XmlFileSerialize xfs = new XmlFileSerialize(InstallPath.GetWindowsServiceInstallPath("WatchDogService")+@"\WatchDogService.Config");

            if (!xfs.FileExist())
            {
                mps.Clear();
                MonitoringProgram mp = new MonitoringProgram();
                mp.Name = "";
                mp.Path = "";
                mp.StartArguments = "";
                mp.MonitoringData = "";
                mp.MonitoringMethod = MonitoringMethodType.Process;
                mp.MonitoringTime = 10000;
                mp.Type = MonitoringProgramType.Control;
                mps.Add(mp);
                xfs.Serialize<List<MonitoringProgram>>(mps);
                throw new Exception("WatchDogService.Config is not edit.");
            }

            mps = xfs.DeSerialize<List<MonitoringProgram>>();

            foreach (MonitoringProgram mp in mps)
            {
                Monitoring monitoring = new Monitoring(mp);
                try
                {
                    monitoring.Start();
                    monitorings.Add(monitoring);
                }
                catch
                {
                    continue;
                }
            }
            
        }

        protected override void OnStop()
        {
            foreach(Monitoring monitoring in monitorings)
            {
                monitoring.Stop();
            }
        }
    }

    
}
