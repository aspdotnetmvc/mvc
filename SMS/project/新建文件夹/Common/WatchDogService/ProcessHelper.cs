using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WatchDogService
{
    public class ProcessHelper
    {
        public static bool KillProcess(string processName)
        {
            System.Diagnostics.Process myproc = new System.Diagnostics.Process();
            foreach (Process thisproc in Process.GetProcessesByName(processName))
            {
                try
                {
                    if (!thisproc.CloseMainWindow())
                    {
                        thisproc.Kill();
                        thisproc.Close();
                        GC.Collect();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static bool StartProcess(string fileName,string filePath,string arguments)
        {
            try
            {
                if (!CheckProcessExists(filePath + fileName))
                {
                    Process p = new Process();
                    p.StartInfo.FileName = filePath+fileName;
                    p.StartInfo.WorkingDirectory = filePath;
                    p.StartInfo.Arguments = arguments;

                    //p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    //p.StartInfo.UseShellExecute = false;
                    //p.StartInfo.RedirectStandardInput = true;
                    //p.StartInfo.RedirectStandardOutput = true;
                    //p.StartInfo.RedirectStandardError = true;
                    //p.StartInfo.CreateNoWindow = true;

                    p.Start();
                    p.WaitForInputIdle(10000);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckProcessExists(string processPath)
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process p in processes)
            {
                try
                {
                    if (processPath == p.MainModule.FileName)
                        return true;
                }
                catch
                {
                    continue;
                }
            }

            return false;
        }

        public static bool StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
