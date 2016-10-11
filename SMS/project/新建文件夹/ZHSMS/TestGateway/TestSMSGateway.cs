using BXM.Utils;
using GatewayInterface;
using SMSModel;
using SMSUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestGateway
{
    public class TestSMSGateway : ISMSGateway
    {
        public event EventHandler<SMSEventArgs> SMSEvent;
        public event EventHandler<DeliverEventArgs> DeliverEvent;
        public event EventHandler<SendEventArgs> SendEvent;
        public event EventHandler<ReportEventArgs> ReportEvent;
        int sw = 1;
        bool _simulateFailure;
        int _sendSpan;
        Thread _reportThread;
        int _reportSpan;
        int _failurePercentage;
        object _lock = new object();

        Queue<string> _qReport = new Queue<string>();

        public TestSMSGateway(int sendSpan, int reportSpan, bool simulateFailure, int failurePercentage)
        {
            _failurePercentage = failurePercentage;
            _reportSpan = reportSpan;
            _simulateFailure = simulateFailure;
            _sendSpan = sendSpan;
            _reportThread = new Thread(new ThreadStart(ReportThread));
            _reportThread.Start();
        }

        public bool Connect()
        {
            return true;
        }

        public void SendSMS(SMS sms)
        {
            //拆分长消息
            int pkSize;
            int pkNum = SMSSplit.GetSplitNumber(sms.Content, sms.Signature, out pkSize);
            if (pkNum > 1)
            {
                for (int j = 0; j < sms.Number.Count; j++)
                {
                    string content = sms.Content;
                    //发送号码
                    for (int i = 0; i < pkNum; i++)
                    {

                        string MessageContent = SMSSplit.GetSubString(content, pkSize);
                        content = content.Substring(MessageContent.Length);
                        if (i+1 == pkNum)
                        {
                            MessageContent = MessageContent + sms.Signature;
                        }

                        SMS tsms = new SMS();
                        tsms.Number = new List<string>{ sms.Number[j] };
                        tsms.Account = sms.Account;
                        tsms.Audit = sms.Audit;
                        tsms.Channel = sms.Channel;
                        tsms.Content = MessageContent;
                        tsms.Filter = sms.Filter;
                        tsms.Level = sms.Level;
                        tsms.Number = sms.Number;
                        tsms.SendTime = sms.SendTime;
                        tsms.SerialNumber = sms.SerialNumber;
                        tsms.StatusReport = sms.StatusReport;
                        tsms.Signature = sms.Signature;
                        tsms.SPNumber = sms.SPNumber;
                        tsms.WapURL = sms.WapURL;

                        sw = 0;
                        string serial = Guid.NewGuid().ToString();
                        if (SendEvent != null)
                        {
                            SendEventArgs e = new SendEventArgs( tsms, serial, true, ((ushort)PlatformCode.SYS + (ushort)SystemCode.SendReady), "send", (ushort)pkNum, (ushort)(i+1));
                            SendEvent(this, e);
                            Console.WriteLine(serial + " Send --> " + JsonSerialize.Instance.Serialize<SendEventArgs>(e));
                            sw = 1;
                        }
                        lock (_lock)
                        {
                            _qReport.Enqueue(serial);
                        }
                        Thread.Sleep(_sendSpan);
                    }
                }
            }
            else
            {
                sw = 0;
                string serial = Guid.NewGuid().ToString();
                if (SendEvent != null)
                {
                    SendEventArgs e = new SendEventArgs( sms, serial, true, ((ushort)PlatformCode.SYS + (ushort)SystemCode.SendReady), "send", (ushort)1, (ushort)1);
                    SendEvent(this, e);
                    Console.WriteLine(serial + " Send --> " + JsonSerialize.Instance.Serialize<SendEventArgs>(e));
                    sw = 1;
                }
                lock (_lock)
                {
                    _qReport.Enqueue(serial);
                }

                Thread.Sleep(_sendSpan);
            }
        }

        void ReportThread()
        {
            while (true)
            {
                try
                {
                    if (_qReport.Count > 0)
                    {
                        string serial = "";
                        lock (_lock)
                        {
                            serial = _qReport.Dequeue();
                        }
                        bool rs = true;
                        if (_simulateFailure)
                        {
                            Random rd = new Random();
                            rs = (rd.Next(0, 100) <= _failurePercentage) ? false : true;
                        }

                        if (ReportEvent != null)
                        {
                            ReportEventArgs args = new ReportEventArgs( serial, rs, ((ushort)PlatformCode.SYS + (ushort)SystemCode.ReportBack), "report", DateTime.Now);
                            ReportEvent(this, args);
                            
                            Console.WriteLine(serial + " Report --> " + JsonSerialize.Instance.Serialize<ReportEventArgs>(args));
                        }
                    }
                }
                catch
                {
                }
                Thread.Sleep(_reportSpan);
            }
        }

        public int Ready
        {
            get
            {
                return sw;
            }
        }

        public string SrcID
        {
            get
            {
                return "123456";
            }
        }

        public void SendSubmit(SMS sms)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            
        }
    }
}
