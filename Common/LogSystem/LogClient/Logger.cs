using MQAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LogClient
{
    public class Logger:IDisposable
    {
        delegate void WriteDelegate(string level, string thread, string ip, string recorder, string _event, string message);
        Fanout mqSend;
        string _ips = "";
        WriteDelegate write;
        
        Queue<string> sends;
        Thread otherSendThread;
        bool run;
        object lockSend = new object();


        public Logger(string vhost,string url,string name,string password,string channel)
        {
            mqSend = new Fanout(vhost,url,name,password, channel, new string[] { channel });
            run = true;
            sends = new Queue<string>();
            otherSendThread = new Thread(new ThreadStart(OtherSendThread));
            write  = new WriteDelegate(LogWrite);
            System.Net.IPHostEntry IpEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            for (int i = 0; i != IpEntry.AddressList.Length; i++)
            {
                if (!IpEntry.AddressList[i].IsIPv6LinkLocal)
                {
                    _ips += (IpEntry.AddressList[i].ToString() + ";");
                }
            }
            otherSendThread.Start();
        }

        private void LogWrite(string level, string thread, string ip, string recorder, string _event, string message)
        {
            char space = (char)0x1d;
            string str = level + space + thread + space + ip + space + recorder + space + _event + space + message;

            bool sok = true;
            try
            {
                sok = mqSend.Send(str);
            }
            catch
            {

            }

            if (!sok)
            {
                lock (lockSend)
                {
                    sends.Enqueue(str);
                }
            }

        }

        private void OtherSendThread()
        {
            while (run)
            {
                try
                {
                    lock (lockSend)
                    {
                        while (sends.Count > 0)
                        {
                            string s = sends.Dequeue();
                            if (!mqSend.Send(s))
                            {
                                sends.Enqueue(s);
                                break;
                            }
                        }
                    }
                }
                catch
                {

                }
                Thread.Sleep(1000);
            }
        }

        public void Log(string level, string thread, string ip, string recorder, string _event, string message)
        {
            write.BeginInvoke(level, thread, ip, recorder, _event, message, null, null);
        }

        #region 2
        public void LogFatal(string recorder, string message)
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            write.BeginInvoke("Fatal", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(), _ips, recorder, stackTrace.GetFrame(1).GetMethod().ReflectedType.ToString(), message, null, null);
        }

        public void LogError(string recorder, string message)
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            write.BeginInvoke("Error", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(), _ips, recorder, stackTrace.GetFrame(1).GetMethod().ReflectedType.ToString(), message, null, null);
        }

        public void LogWarn(string recorder, string message)
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            write.BeginInvoke("Warn", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(), _ips, recorder, stackTrace.GetFrame(1).GetMethod().ReflectedType.ToString(), message, null, null);
        }

        public void LogInfo(string recorder, string message)
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            write.BeginInvoke("Info", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(), _ips, recorder, stackTrace.GetFrame(1).GetMethod().ReflectedType.ToString(), message, null, null);
        }

        public void LogDebug(string recorder, string message)
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            write.BeginInvoke("Debug", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(), _ips, recorder, stackTrace.GetFrame(1).GetMethod().ReflectedType.ToString(), message, null, null);
        }
        #endregion 2

        #region 4
        public void LogFatal(string thread, string recorder, string _event, string message)
        {
            write.BeginInvoke("Fatal", thread, _ips, recorder, _event, message, null, null);
        }
        public void LogError(string thread, string recorder, string _event, string message)
        {
            write.BeginInvoke("Error", thread, _ips, recorder, _event, message, null, null);
        }

        public void LogWarn(string thread, string recorder, string _event, string message)
        {
            write.BeginInvoke("Warn", thread, _ips, recorder, _event, message, null, null);
        }

        public void LogInfo(string thread, string recorder, string _event, string message)
        {
            write.BeginInvoke("Info", thread, _ips, recorder, _event, message, null, null);
        }

        public void LogDebug(string thread, string recorder, string _event, string message)
        {
            write.BeginInvoke("Debug", thread, _ips, recorder, _event, message, null, null);
        }
        #endregion 4

        #region 3
        public void LogFatal(string recorder, string _event, string message)
        {
            write.BeginInvoke("Fatal", "", _ips, recorder, _event, message, null, null);
        }
        public void LogError(string recorder, string _event, string message)
        {
            write.BeginInvoke("Error", "", _ips, recorder, _event, message, null, null);
        }

        public void LogWarn(string recorder, string _event, string message)
        {
            write.BeginInvoke("Warn", "", _ips, recorder, _event, message, null, null);
        }

        public void LogInfo(string recorder, string _event, string message)
        {
            write.BeginInvoke("Info", "", _ips, recorder, _event, message, null, null);
        }

        public void LogDebug(string recorder, string _event, string message)
        {
            write.BeginInvoke("Debug", "", _ips, recorder, _event, message, null, null);
        }

        #endregion 3

        #region 5
        public void LogFatal(string thread, string ip, string recorder, string _event, string message)
        {
            write.BeginInvoke("Fatal", thread, ip, recorder, _event, message, null, null);
        }
        public void LogError(string thread, string ip, string recorder, string _event, string message)
        {
            write.BeginInvoke("Error", thread, ip, recorder, _event, message, null, null);
        }

        public void LogWarn(string thread, string ip, string recorder, string _event, string message)
        {
            write.BeginInvoke("Warn", thread, ip, recorder, _event, message, null, null);
        }

        public void LogInfo(string thread, string ip, string recorder, string _event, string message)
        {
            write.BeginInvoke("Info", thread, ip, recorder, _event, message, null, null);
        }

        public void LogDebug(string thread, string ip, string recorder, string _event, string message)
        {
            write.BeginInvoke("Debug", thread, ip, recorder, _event, message, null, null);
        }
        #endregion 5

        public void Dispose()
        {
            run = false;
        }
    }
}
