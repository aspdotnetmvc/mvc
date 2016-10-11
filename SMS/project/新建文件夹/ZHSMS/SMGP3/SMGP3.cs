using GatewayInterface;
using SMSModel;
using SMSUtils;
using SocketAsyncClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SMGP
{
    public class SMGP3 : ISMSGateway, IDisposable
    {
        ISocketAsyncClient _client;
        SMGPSetting _setting;

        SlidingWindow[] _sendWindows;
        int _sendsCount = 0;
        int _connectCount = 0;
        long _lastConnectTime = 0;
        Thread _monitorConnectThread;
        Thread _monitorTimeOutThread;

        CancellationTokenSource _ctsQuit = new CancellationTokenSource();

        bool _connect = false;
        bool _connecting = true;

        private static AutoResetEvent autoConnectEvent = new AutoResetEvent(false);

        public event EventHandler<SendEventArgs> SendEvent;
        public event EventHandler<ReportEventArgs> ReportEvent;
        public event EventHandler<SMSEventArgs> SMSEvent;
        public event EventHandler<DeliverEventArgs> DeliverEvent;
        /// <summary>
        /// 最近一次网络传输时间（用于发送 CMPP_ACTIVE_TEST 网络检测包）。
        /// </summary>
        private DateTime _dtLastTransferTime;

        public SMGP3(SMGPSetting setting)
        {
            _setting = setting;

            InitClient();

            _sendWindows = new SlidingWindow[_setting.SlidingWindowSize];
            _dtLastTransferTime = DateTime.Now;
            //启动相关线程
            _monitorConnectThread = new Thread(new ThreadStart(MonitorConnectRun));
            _monitorConnectThread.Start();
            _monitorTimeOutThread = new Thread(new ThreadStart(MonitorTimeOutRun));
            _monitorTimeOutThread.Start();
        }

        #region 网络操作
        void InitClient()
        {
            _client = null;
            _client = new SocketClient(_setting.Ip, _setting.Port, 1024,_setting.LocalIP);
            _client.DataReceived += _sc_DataReceived;
            _client.Closed += _sc_Closed;
            _client.Error += _sc_Error;
        }
        void _sc_Error(Exception error)
        {
            DebugLog.Instance.Write("Error -> ", error.ToString());
        }

        void _sc_Closed()
        {
            _connect = false;
        }

        #endregion

        #region 滑动窗口
        void MonitorTimeOutRun()
        {
            while (true)
            {
                try
                {
                    if (_ctsQuit.IsCancellationRequested)
                    {
                        break;
                    }

                    if ((DateTime.Now - _dtLastTransferTime).TotalSeconds >= _setting.ActiveTestSpan)
                    {
                        Active_Test();
                    }

                    for (int i = 0; i < _sendWindows.Length; i++)
                    {
                        //发送超时
                        if (_sendWindows[i].Status == WindowStatus.Fill)
                        {
                            if ((DateTime.Now - _sendWindows[i].SendTime).TotalSeconds >= _setting.TimeOut)
                            {
                                //if (_sendWindows[i].MSG.Command == SMGP3_COMMAND.Active_Test)
                                //{
                                //    SWClr(_sendWindows[i].MSG.SequenceID);
                                //    _connect = false;
                                //    break;
                                //}
                                //小于重发次数
                                if (_sendWindows[i].SendCount < _setting.SendCount)
                                {
                                    //超时重发处理
                                    _sendWindows[i].SendTime = DateTime.Now;
                                    _sendWindows[i].SendCount++;
                                    _sendWindows[i].MSG.SequenceID = Sequence.Instance.CreateID();
                                    Send(_sendWindows[i].MSG);
                                    continue;
                                }
                                else
                                {
                                    //超时处理
                                    if (SendEvent != null)
                                    {
                                        _sendWindows[i].Report.StatusCode = ((ushort)PlatformCode.SMGP + (ushort)SystemCode.SendTimeOut);
                                        _sendWindows[i].Report.Describe = "";
                                        _sendWindows[i].Report.Succeed = false;
                                        SendEvent(this, _sendWindows[i].Report);
                                    }
                                    SWClr(_sendWindows[i].MSG.SequenceID);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
                catch
                {
                }
            }
        }
        //检测连接
        void MonitorConnectRun()
        {
            while (true)
            {
                try
                {
                    if (_ctsQuit.IsCancellationRequested)
                    {
                        break;
                    }

                    if (_connecting)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    if (!_connect)
                    {
                        Connect();
                        Thread.Sleep(1000);
                        continue;
                    }

                    Thread.Sleep(10);
                }
                catch
                {
                }
            }
        }

        void SWSet(SlidingWindow send)
        {
            lock (_sendWindows)
            {
                for (int i = 0; i < _sendWindows.Length; i++)
                {
                    if (_sendWindows[i].Status == WindowStatus.Idle)
                    {
                        _sendWindows[i] = send;
                        _sendWindows[i].Status = WindowStatus.Fill;
                        _sendsCount++;
                        return;
                    }
                }
            }
        }

        SlidingWindow SWGet(uint sequenceID)
        {
            lock (_sendWindows)
            {
                for (int i = 0; i < _sendWindows.Length; i++)
                {
                    if (_sendWindows[i].Status != WindowStatus.Idle)
                    {
                        if (_sendWindows[i].MSG.SequenceID == sequenceID)
                        {
                            return _sendWindows[i];
                        }
                    }
                }
            }
            return default(SlidingWindow);
        }

        void SWClr(uint sequenceID)
        {
            lock (_sendWindows)
            {
                for (int i = 0; i < _sendWindows.Length; i++)
                {
                    if (_sendWindows[i].Status != WindowStatus.Idle)
                    {
                        if (_sendWindows[i].MSG.SequenceID == sequenceID)
                        {
                            _sendsCount--;
                            _sendWindows[i].Status = WindowStatus.Idle;
                            _sendWindows[i].MSG = null;
                            _sendWindows[i].Report = null;
                        }
                    }
                }
            }
        }
        #endregion

        #region 数据接收
        void _sc_DataReceived(byte[] data)
        {
            MessageHeader head;
            _dtLastTransferTime = DateTime.Now;
            try
            {
                head = new MessageHeader(data); 
            }
            catch (Exception ex)
            {
                OnNetworkError(ex);
                return;
            }

            switch ((SMGP3_COMMAND)head.Command)
            {
                case SMGP3_COMMAND.Login_Resp:
                    Login_Resp login_Resp;
                    try
                    {
                        login_Resp = new Login_Resp(data);
                    }
                    catch (Exception ex)
                    {
                        OnNetworkError(ex);
                        return;
                    }
                    DebugLog.Instance.Write<Login_Resp>("Received -> Login_Resp", login_Resp);
                    switch (login_Resp.Status)
                    {
                        case 0:
                            _connect = true;
                            autoConnectEvent.Set();
                            OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT, "登录成功"));
                            break;
                        default:
                            OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, StateDictionary.stateRespDictionary(login_Resp.Status)));
                            break;
                    }
                    break;

                case SMGP3_COMMAND.Submit_Resp:
                    Submit_Resp submit_resp;

                    try
                    {
                        submit_resp = new Submit_Resp(data);
                    }
                    catch (Exception ex)
                    {
                        OnNetworkError(ex);
                        return;
                    }

                    DebugLog.Instance.Write<Submit_Resp>("Received -> Submit_Resp", submit_resp);
                    
                    SlidingWindow send = SWGet(head.SequenceID);
                    if (send.Status == WindowStatus.Idle) return;
                    SWClr(head.SequenceID);

                    if (SendEvent != null)
                    {
                        send.Report.Succeed = false;
                        switch (submit_resp.Result)
                        {
                            case 0:
                                send.Report.Succeed = true;
                                break;
                            default:
                                send.Report.Describe = StateDictionary.stateRespDictionary(submit_resp.Result);
                                break;

                        }
                        send.Report.Serial = submit_resp.MsgID;
                        SendEvent(this, send.Report);
                    }                    
                    break;
                case SMGP3_COMMAND.Deliver:
                    Deliver deliver = new Deliver(data);
                    DebugLog.Instance.Write<Deliver>("Received -> Deliver", deliver);
                    Deliver_Resp deliverResp = new Deliver_Resp(deliver.MsgID, 0, deliver.Header.SequenceID);
                    Send(deliverResp);
                    DebugLog.Instance.Write<Deliver_Resp>("Send -> Deliver_Resp", deliverResp);

                    if (deliver.IsReport == 1)
                    {
                        Report report = new Report(deliver.Report);

                        bool result = false;
                        string resultMessage = "";
                        ushort resultCode = ((ushort)PlatformCode.SMGP + (ushort)SystemCode.ReportBack);
                        
                        DateTime time;
                        try
                        {
                            time = new DateTime(DateTime.Now.Year, System.Convert.ToInt32(report.Done_date.Substring(2, 2)), System.Convert.ToInt32(report.Done_date.Substring(4, 2)), System.Convert.ToInt32(report.Done_date.Substring(6, 2)), System.Convert.ToInt32(report.Done_date.Substring(8, 2)), 0);
                        }
                        catch
                        {
                            time = DateTime.Now;
                        }

                        switch (report.Stat.ToUpper())
                        {
                            case "DELIVRD":
                            case "RETRIEV":
                                result = true;
                                break;
                            case "EXPIRED":
                            case "DELETED":
                            case "UNDELIV":
                            case "ACCEPTD":
                            case "UNKNOWN":
                            case "REJECTD":
                            default:
                                resultMessage = report.Stat;
                                break;
                        }
                        ReportEventArgs args = new ReportEventArgs( report.Id, result, resultCode, resultMessage, time);
                        ReportEvent(this, args);
                    }
                    else
                    {
                        //处理deliver
                        if (DeliverEvent != null)
                        {
                            
                            DeliverEvent(this, new DeliverEventArgs( deliver.MsgID.ToString(), DateTime.Now, deliver.MsgContent, deliver.SrcTermID, deliver.DestTermID, "",deliver.LinkID));
                        }
                    }
                    break;
                case SMGP3_COMMAND.Active_Test:
                    Active_Test_Resp active_test_resp1 = new Active_Test_Resp(data);
                    active_test_resp1.Header.Command = SMGP3_COMMAND.Active_Test_Resp;
                    Send(active_test_resp1);
                    break;
                case SMGP3_COMMAND.Active_Test_Resp:
                    //Active_Test_Resp active_test_resp2 = new Active_Test_Resp(data);
                    //SWClr(active_test_resp2.SequenceID);
                    break;
                case SMGP3_COMMAND.Exit:
                    _connect = false;
                    break;
            }
        }

        #endregion

        /// <summary>
        /// 引发 SMS 事件。
        /// </summary>
        protected virtual void OnSMSEvent(SMSEventArgs e)
        {
            if (SMSEvent != null)
            {
                SMSEvent(this, e);
            }
        }

        private void OnNetworkError(Exception ex)
        {
            DebugLog.Instance.Write("Error -> " + ex.ToString());
            _connect = false;
        }

        /// <summary>
        /// 连接到 ISMG。
        /// </summary>
        public bool Connect()
        {
            _connecting = true;
            _connect = false;

            Login connect = new Login(_setting.SPID, _setting.Password,2,DateTime.Now,0x30, Sequence.Instance.CreateID());

            byte[] bytes = connect.ToBytes();
            _client.Close();

            while (_client.IsConnected)
            {
                Thread.Sleep(100);
            }
            //最后一次连接跟本次连接是否在20秒之内
            if (DateTime.Now.Ticks - _lastConnectTime < 10000000*20)
            {
                _connectCount++;
            }
            //重连10次后重新建立客户端对象
            if (_connectCount == 10)
            {
                InitClient();
                _connectCount = 0;
            }
            _lastConnectTime = DateTime.Now.Ticks;
            if (_client.Connect())
            {
                Thread.Sleep(1000);
            }
            else
            {
                _connecting = false;
                return false;
            }

            try
            {
                _client.Send(bytes);
            }
            catch
            {
                _connecting = false;
                return false;
            }

            autoConnectEvent.WaitOne(_setting.TimeOut * 100, false);

            if (!_connect)
            {
                OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, "等待 CONNECT_RESP 超时"));
                _connecting = false;
                return false;
            }
            _connecting = false;
            return _connect;
        }

        #region 发送
        private void Send(ISMGP_MESSAGE message)
        {
            while (true)
            {
                if (!_connect)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                Thread.Sleep(_setting.SendSpan);

                try
                {
                    byte[] bytes = message.GetBytes();
                    _client.Send(bytes);
                    _dtLastTransferTime = DateTime.Now;
                    break;
                }
                catch
                {
                    continue;
                }
            }
        }

        private void Active_Test()
        {
            Active_Test active_test = new Active_Test(Sequence.Instance.CreateID());

            //SlidingWindow send = new SlidingWindow();
            //send.SendCount = 0;
            //send.SendTime = DateTime.Now;
            //send.MSG = active_test;
            //send.Status = WindowStatus.Fill;
            //SWSet(send);

            Send(active_test);
        }

        #endregion

        public int Ready
        {
            get
            {
                int count = _setting.SlidingWindowSize - _sendsCount;
                if (count < 0) count = 0;
                return count;
            }
        }

        public void SendSMS(SMS sms)
        {
            List<SlidingWindow> submits = GetSend(sms);

            foreach (SlidingWindow send in submits)
            {
                while (true)
                {
                    if (Ready == 0)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    SWSet(send);
                    Send(send.MSG);
                    DebugLog.Instance.Write<Submit>("Send -> Submit", (Submit)send.MSG);
                    break;
                }
            }
        }

        private List<SlidingWindow> GetSend(SMS sms)
        {
            List<SlidingWindow> submits = new List<SlidingWindow>();

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
                        Submit submit = new Submit(Sequence.Instance.CreateID());

                        submit.Pk_total = (uint)pkNum; ;
                        submit.Pk_number = (uint)(i + 1); ;

                        submit.WapURL = "";
                        submit.MsgType = 6;             //  uint    6＝MT 消息（SP 发给终端，包括WEB 上发送的点对点短消息）；
                        submit.NeedReport = sms.StatusReport == StatusReportType.Disable ? (byte)0 : (byte)1; ; //  uint    0＝不要求返回状态报告； 1＝要求返回状态报告；
                        submit.Priority = 1;
                        submit.ServiceID = _setting.ServiceID;
                        submit.FeeType = _setting.FeeType;
                        submit.FeeCode = _setting.FeeCode;
                        submit.FixedFee = "0";
                        submit.ValidTime = _setting.ValidTime; //Util.Get_MMDDHHMMSS_String(DateTime.Now.AddHours(5)) + "032+";  // 存活有效期,格式遵循SMPP3.3协议。 
                        submit.AtTime = "";   //Util.Get_MMDDHHMMSS_String(DateTime.Now) + "032+";     //      短消息定时发送时间，格式遵循SMPP3.3 以上版本协议。
                        submit.SrcTermID = _setting.SrcID + sms.SPNumber;
                        submit.ChargeTermID = _setting.ChargeTermID;
                        submit.DestTermID = sms.Number.ToArray();
                        submit.DestTermIDCount = (uint)submit.DestTermID.Length;
                        submit.Reserve = "";
                        submit.ProductID = "";
                        submit.LinkID = sms.LinkID;
                        submit.MsgFormat = 8;//15;
                        submit.WapURL = sms.WapURL;
                        submit.MsgSrc = _setting.MsgSrc;

                        submit.MsgContent = SMSSplit.GetSubString(content, pkSize);
                        content = content.Substring(submit.MsgContent.Length);
                        if (submit.Pk_number == pkNum)
                        {
                            submit.MsgContent = submit.MsgContent + sms.Signature;
                        }
                        submit.SetHeader();

                        SMS tsms = new SMS();
                        tsms.Account = sms.Account;
                        tsms.Audit = sms.Audit;
                        tsms.Channel = sms.Channel;
                        tsms.Content = submit.MsgContent;
                        tsms.Filter = sms.Filter;
                        tsms.Level = sms.Level;
                        tsms.Number = sms.Number;
                        tsms.SendTime = sms.SendTime;
                        tsms.SerialNumber = sms.SerialNumber;
                        tsms.StatusReport = sms.StatusReport;
                        tsms.Signature = sms.Signature;
                        tsms.SPNumber = sms.SPNumber;
                        tsms.WapURL = sms.WapURL;

                        SlidingWindow _send = new SlidingWindow();
                        _send.Report = new SendEventArgs( tsms, "", false, ((ushort)PlatformCode.SMGP + (ushort)SystemCode.SendReady), "", (ushort)submit.Pk_total, (ushort)submit.Pk_number);
                        _send.MSG = submit;
                        _send.SendCount = 0;
                        _send.SendTime = DateTime.Now;
                        _send.Status = WindowStatus.Fill;
                        submits.Add(_send);
                    }
                }
            }
            else
            {
                for (int j = 0; j < sms.Number.Count; j++)
                {
                    Submit submit = new Submit(Sequence.Instance.CreateID());

                    submit.Pk_total = 1;
                    submit.Pk_number = 1 ;

                    submit.WapURL = "";
                    submit.MsgType = 6;             //  uint    6＝MT 消息（SP 发给终端，包括WEB 上发送的点对点短消息）；
                    submit.NeedReport = sms.StatusReport == StatusReportType.Disable ? (byte)0 : (byte)1; ; //  uint    0＝不要求返回状态报告； 1＝要求返回状态报告；
                    submit.Priority = 1;
                    submit.ServiceID = _setting.ServiceID; ;
                    submit.FeeType = _setting.FeeType;
                    submit.FeeCode = _setting.FeeCode;
                    submit.FixedFee = "0";
                    submit.ValidTime = _setting.ValidTime; //Util.Get_MMDDHHMMSS_String(DateTime.Now.AddHours(5)) + "032+";  // 存活有效期,格式遵循SMPP3.3协议。 
                    submit.AtTime = "";   //Util.Get_MMDDHHMMSS_String(DateTime.Now) + "032+";     //      短消息定时发送时间，格式遵循SMPP3.3 以上版本协议。
                    submit.SrcTermID = _setting.SrcID + sms.SPNumber;
                    submit.ChargeTermID = _setting.ChargeTermID;
                    submit.DestTermID = sms.Number.ToArray();
                    submit.DestTermIDCount = (uint)submit.DestTermID.Length;
                    submit.Reserve = "";
                    submit.ProductID = "";
                    submit.LinkID = sms.LinkID;
                    submit.MsgFormat = 15;
                    submit.WapURL = sms.WapURL;
                    submit.MsgSrc = _setting.MsgSrc;
                    //信息内容

                    submit.MsgContent = sms.Content + sms.Signature;
                    submit.SetHeader();


                    SMS tsms = new SMS();
                    tsms.Account = sms.Account;
                    tsms.Audit = sms.Audit;
                    tsms.Channel = sms.Channel;
                    tsms.Content = submit.MsgContent;
                    tsms.Filter = sms.Filter;
                    tsms.Level = sms.Level;
                    tsms.Number = sms.Number;
                    tsms.SendTime = sms.SendTime;
                    tsms.SerialNumber = sms.SerialNumber;
                    tsms.StatusReport = sms.StatusReport;
                    tsms.Signature = sms.Signature;
                    tsms.SPNumber = sms.SPNumber;
                    tsms.WapURL = sms.WapURL;
                    
                    SlidingWindow _send = new SlidingWindow();
                    _send.Report = new SendEventArgs( tsms, "", false, ((ushort)PlatformCode.SMGP + (ushort)SystemCode.SendReady), "", (ushort)1, (ushort)1);
                    _send.MSG = submit;
                    _send.SendCount = 0;
                    _send.SendTime = DateTime.Now;
                    _send.Status = WindowStatus.Fill;
                    submits.Add(_send);
                }
            }

            return submits;
        }

        public string SrcID
        {
            get
            {
                return _setting.SrcID;
            }
        }
        bool _dispose = true;
        public void Dispose()
        {
            if (_dispose)
            {
                _dispose = false;
                _ctsQuit.Cancel();
                _client.Close();
            }
        }

        public void Close()
        {
            this.Dispose();
        }

        public void SendSubmit(SMS sms)
        {
            throw new NotImplementedException();
        }
    }
}
