using GatewayInterface;
using SMSModel;
using SMSUtils;
using SocketAsyncClient;
using SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SGIP
{
    public class SGIP12 : ISMSGateway, IDisposable
    {
        public event EventHandler<SMSEventArgs> SMSEvent;
        public event EventHandler<SendEventArgs> SendEvent;
        public event EventHandler<ReportEventArgs> ReportEvent;
        public event EventHandler<DeliverEventArgs> DeliverEvent;

        private ISocketAsyncServer _server;
        private ISocketAsyncClient _client;
        static SGIPSetting _setting;

        Thread _monitorConnectThread;
        Thread _monitorTimeOutThread;
        SlidingWindow[] _sendWindows;
        int _sendsCount = 0;
        CancellationTokenSource _ctsQuit = new CancellationTokenSource();

        public static SGIPSetting Setting
        {
            get
            {
                return _setting;
            }
        }

        private static AutoResetEvent autoConnectEvent = new AutoResetEvent(false);

        bool _connect = false;
        bool _connecting = true;

        public SGIP12(SGIPSetting setting)
        {
            _setting = setting;
            _client = new SocketClient(Setting.ServerIp, Setting.ServerPort, 1024);
            _client.DataReceived += _client_DataReceived;
            _client.Closed += _client_Closed;
            _client.Error += _client_Error;
            _sendWindows = new SlidingWindow[_setting.SlidingWindowSize];
            InitServer();
            //启动相关线程
            _monitorConnectThread = new Thread(new ThreadStart(MonitorConnectRun));
            _monitorConnectThread.Start();
            _monitorTimeOutThread = new Thread(new ThreadStart(MonitorTimeOutRun));
            _monitorTimeOutThread.Start();
        }


        void _client_Error(Exception error)
        {
            DebugLog.Instance.Write("Client Error -> " + error.ToString());
        }

        void _client_Closed()
        {
            DebugLog.Instance.Write("client_Closed -> ");
            _connect = false;
        }

        private void InitServer()
        {
            _server = new SocketServer.SocketServer(100, 1024, _setting.LocalPort,_setting.LocalIP);
            _server.DataReceived += _server_DataReceived;
            _server.Error += _server_Error;
            _server.Start();
        }

        void _server_Error(Exception error)
        {
            DebugLog.Instance.Write("Server Error -> " + error.ToString());
        }

        
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
                    
                    for (int i = 0; i < _sendWindows.Length; i++)
                    {
                        //发送超时
                        if (_sendWindows[i].Status == WindowStatus.Fill)
                        {
                            if ((DateTime.Now - _sendWindows[i].SendTime).TotalSeconds >= _setting.TimeOut)
                            {
                                //小于重发次数
                                if (_sendWindows[i].SendCount < _setting.SendCount)
                                {
                                    //超时重发处理
                                    _sendWindows[i].SendTime = DateTime.Now;
                                    _sendWindows[i].SendCount++;
                                    ClientSend(_sendWindows[i].MSG);
                                    DebugLog.Instance.Write<SGIP_SUBMIT>("Client TimeOut -> " ,(SGIP_SUBMIT)_sendWindows[i].MSG);
                                    continue;
                                }
                                else
                                {
                                    //超时处理
                                    if (SendEvent != null)
                                    {
                                        _sendWindows[i].Report.StatusCode = ((ushort)PlatformCode.SGIP + (ushort)SystemCode.SendTimeOut);
                                        _sendWindows[i].Report.Describe = "";
                                        _sendWindows[i].Report.Succeed = false;
                                        SendEvent(this, _sendWindows[i].Report);
                                    }
                                    SWClr(_sendWindows[i].Report.Serial);

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

        #region 滑动窗口
        void SWSet(SlidingWindow send)
        {
            lock (_sendWindows)
            {
                for (int i = 0; i < _sendWindows.Length; i++)
                {
                    if (_sendWindows[i].Status ==  WindowStatus.Idle)
                    {
                        _sendWindows[i] = send;
                        _sendWindows[i].Status = WindowStatus.Fill;
                        _sendsCount++;
                        return;
                    }
                }
            }
        }

        SlidingWindow SWGet(string sequenceID)
        {
            lock (_sendWindows)
            {
                for (int i = 0; i < _sendWindows.Length; i++)
                {
                    if (_sendWindows[i].Status != WindowStatus.Idle)
                    {
                        if (_sendWindows[i].Report.Serial == sequenceID)
                        {
                            return _sendWindows[i];
                        }
                    }
                }
            }
            return default(SlidingWindow);
        }

        void SWClr(string sequenceID)
        {
            lock (_sendWindows)
            {
                for (int i = 0; i < _sendWindows.Length; i++)
                {
                    if (_sendWindows[i].Status !=  WindowStatus.Idle)
                    {
                        if (_sendWindows[i].Report.Serial == sequenceID)
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


        private void OnClientNetworkError(Exception ex)
        {
            DebugLog.Instance.Write("OnClientNetworkError -> ", ex.ToString());
            _connect = false;
        }

        void _client_DataReceived(byte[] data)
        {
            SGIP_MESSAGE header;
            try
            {
                header = new SGIP_MESSAGE(data);
            }
            catch (Exception ex)
            {
                OnClientNetworkError(ex);
                return;
            }
            switch (header.Command_Id)
            {
                case SGIP_COMMAND.SGIP_BIND_RESP:
                    SGIP_BIND_RESP sgip_bind_resp;
                    try
                    {
                        sgip_bind_resp = new SGIP_BIND_RESP(data);
                    }
                    catch (Exception ex)
                    {
                        OnClientNetworkError(ex);
                        return;
                    }
                    DebugLog.Instance.Write<SGIP_BIND_RESP>("Client Received -> SGIP_BIND_RESP", sgip_bind_resp);
                    
                    switch (sgip_bind_resp.Result)
                    {
                        case 0:
                            _connect = true;
                            autoConnectEvent.Set();
                            OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT, "登录成功"));
                            break;
                        default:
                            OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, StateDictionary.StateRespDictionary(sgip_bind_resp.Result)));
                            break;

                    }
                    break;
                case SGIP_COMMAND.SGIP_SUBMIT_RESP:
                    SGIP_SUBMIT_RESP submit_resp;
                    try
                    {
                        submit_resp = new SGIP_SUBMIT_RESP(data);
                    }
                    catch (Exception ex)
                    {
                        OnClientNetworkError(ex);
                        return;
                    }
                    DebugLog.Instance.Write<SGIP_SUBMIT_RESP>("Client Received -> SGIP_SUBMIT_RESP", submit_resp);
                    string serial = header.SrcNodeSequence.ToString() + header.DateSequence.ToString() + header.Sequence_Id.ToString();
                    SlidingWindow send = SWGet(serial);
                    if (send.Status == WindowStatus.Idle) return;
                    SWClr(serial);

                    if (SendEvent != null)
                    {
                        send.Report.Succeed = false;
                        switch (submit_resp.Result)
                        {
                            case 0:
                                send.Report.Succeed = true;
                                break;
                            default:
                                send.Report.Describe = StateDictionary.StateRespDictionary(submit_resp.Result);
                                break;

                        }
                        send.Report.Serial = serial;
                        SendEvent(this, send.Report);
                    }                    
                    break;
                case SGIP_COMMAND.SGIP_UNBIND:
                    SGIP_UNBIND_RESP unbindResp;
                    try
                    {
                        unbindResp = new SGIP_UNBIND_RESP(header.SrcNodeSequence, header.DateSequence, header.Sequence_Id);
                    }
                    catch (Exception ex)
                    {
                        OnClientNetworkError(ex);
                        return;
                    }
                    DebugLog.Instance.Write("Client Received -> SGIP_UNBIND");

                    ClientSend(unbindResp);
                    DebugLog.Instance.Write<SGIP_UNBIND_RESP>("Client Send -> SGIP_UNBIND_RESP", unbindResp);
                    Thread.Sleep(1000);
                    _connect = false;
                    break;
            }
        }

        void _server_DataReceived(SocketAsyncEventArgs send, byte[] data)
        {
            try
            {
                SGIP_MESSAGE header = new SGIP_MESSAGE(data);

                switch (header.Command_Id)
                {
                    case SGIP_COMMAND.SGIP_BIND:
                        SGIP_BIND bind = new SGIP_BIND(data);
                        DebugLog.Instance.Write<SGIP_BIND>("Server Received -> SGIP_BIND", bind);
                        uint BindResult = 0;
                        if (bind.LoginType == (uint)LoginTypes.SmgToSp || bind.LoginType == (uint)LoginTypes.SpToSmg)
                        {
                            if ((bind.LoginName == _setting.LocalLoginName) && (bind.LoginPassword == _setting.LocalLoginPassword))
                            {
                                BindResult = 0;
                            }
                            else
                            {
                                BindResult = 1;
                            }
                        }
                        else
                        {
                            BindResult = 4;
                        }
                        SGIP_BIND_RESP sgip_bind_resp = new SGIP_BIND_RESP(BindResult, header.SrcNodeSequence, header.DateSequence, header.Sequence_Id);
                        ServerSend(send, sgip_bind_resp);
                        DebugLog.Instance.Write<SGIP_BIND_RESP>("Server Send -> SGIP_BIND_RESP", sgip_bind_resp);
                        break;
                    case SGIP_COMMAND.SGIP_REPORT:
                       // Thread.Sleep(100);//调试代码，上线去掉
                        SGIP_REPORT report = new SGIP_REPORT(data);
                        DebugLog.Instance.Write<SGIP_REPORT>("Server Received -> SGIP_REPORT", report);
                        SGIP_REPORT_RESP reportResp = new SGIP_REPORT_RESP(0, "", header.SrcNodeSequence, header.DateSequence, header.Sequence_Id);
                        ServerSend(send, reportResp);
                        DebugLog.Instance.Write<SGIP_REPORT_RESP>("Server Send -> SGIP_REPORT_RESP", reportResp);

                        if (report.ReportType == 0)
                        {
                            bool result = false;
                            ushort resultCode;
                            string resultMsg = "";
                            string sTime = report.DateSequence.ToString().PadLeft(10, '0');
                            int mm = System.Convert.ToInt32(sTime.Substring(0, 2));
                            int dd =System.Convert.ToInt32(sTime.Substring(2, 2));
                            int hh = System.Convert.ToInt32(sTime.Substring(4, 2));
                            int m =System.Convert.ToInt32(sTime.Substring(6, 2));
                            int s =System.Convert.ToInt32(sTime.Substring(8, 2));
                            DateTime time = new DateTime(DateTime.Now.Year, mm,dd ,hh ,m , s);

                            switch (report.State)
                            {
                                case 0: //成功
                                case 1: //状态报告返回,网关等待发送.
                                    result = true;
                                    resultCode = ((ushort)PlatformCode.SGIP + (ushort)SystemCode.ReportBack);
                                    break;
                                default:
                                    resultCode = ((ushort)PlatformCode.SGIP + (ushort)SystemCode.ReportBack);
                                    resultMsg = report.ErrorCode.ToString();
                                    break;
                            }
                            string serial = report.SrcNodeSequence.ToString() + report.DateSequence.ToString() + report.Sequence_Id.ToString();
                            ReportEventArgs args = new ReportEventArgs( serial, result, resultCode,resultMsg, time);
                            ReportEvent(this, args);
                        }
                        break;
                    case SGIP_COMMAND.SGIP_UNBIND:
                        SGIP_UNBIND_RESP unbindResp = new SGIP_UNBIND_RESP(header.SrcNodeSequence, header.DateSequence, header.Sequence_Id);
                        DebugLog.Instance.Write("Server Received -> SGIP_UNBIND");
                        ServerSend(send, unbindResp);
                        DebugLog.Instance.Write<SGIP_UNBIND_RESP>("Server Send -> SGIP_UNBIND_RESP", unbindResp);
                        break;
                    case SGIP_COMMAND.SGIP_DELIVER:
                        SGIP_DELIVER deliver = new SGIP_DELIVER(data);
                        DebugLog.Instance.Write<SGIP_DELIVER>("Server Received -> SGIP_DELIVER", deliver);
                        SGIP_DELIVER_RESP deliverResp = new SGIP_DELIVER_RESP(0, "", header.SrcNodeSequence, header.DateSequence, header.Sequence_Id);
                        ServerSend(send, deliverResp);
                        DebugLog.Instance.Write<SGIP_DELIVER_RESP>("Server Send -> SGIP_DELIVER_RESP", deliverResp);
                        if (DeliverEvent != null)
                        {
                            string serial = header.SrcNodeSequence.ToString() + header.DateSequence.ToString() + header.Sequence_Id.ToString();
                            DeliverEvent(this, new DeliverEventArgs(serial , DateTime.Now,deliver.MessageContent,deliver.UserNumber,deliver.SPNumber,""));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                DebugLog.Instance.Write<Exception>("Server Received -> Exception", ex);
            }
        }

        void ServerSend(SocketAsyncEventArgs send, ISGIP_MESSAGE message)
        {
            _server.Send(send, message.GetBytes());
        }

        public bool Connect()
        {
            _connecting = true;
            _connect = false;
            
            SGIP_BIND connect = new SGIP_BIND((uint)LoginTypes.SpToSmg, Setting.LoginName, Setting.Password, Sequence.Instance.CreateID());

            byte[] bytes = connect.ToBytes();

            _client.Close();
            while (_client.IsConnected)
            {
                Thread.Sleep(100);
            }

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
                Thread.Sleep(Setting.SendSpan);
                _client.Send(bytes);
                DebugLog.Instance.Write<SGIP_BIND>("Client Send -> SGIP_BIND", connect);
            }
            catch
            {
                _connecting = false;
                return false;
            }
            autoConnectEvent.WaitOne(Setting.TimeOut * 1000, false);

            if (!_connect)
            {
                _client.Close();
                OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, "等待 CONNECT_RESP 超时"));
                _connecting = false;
                return false;
            }
            _connecting = false;
            return _connect;
        }

        private void ClientSend(ISGIP_MESSAGE message)
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
                    break;
                }
                catch
                {
                    continue;
                }
            }
        }

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

        public void SendSubmit(SMS sms)
        {
            SGIPSUBMIT sb = sms.Extend as SGIPSUBMIT;
            if (sb == null) return;
            //发送号码
            SGIP_SUBMIT submit = new SGIP_SUBMIT(Sequence.Instance.CreateID());

            submit.Pk_total = sb.Pk_total;
            submit.Pk_number = sb.Pk_number;
            //SPID
            submit.SPNumber = sb.SPNumber;
            //被计费用户号码
            submit.ChargeNumber = sb.ChargeNumber;
            submit.UserCount = sb.UserCount;
            submit.CorpId = sb.CorpId;
            submit.ServiceType = sb.ServiceType;
            submit.FeeType = sb.FeeType;
            submit.FeeValue = sb.FeeValue;
            //赠送用户的话费
            submit.GivenValue = sb.GivenValue;
            //代收费标志
            submit.AgentFlag = sb.AgentFlag;
            submit.MorelatetoMTFlag = sb.MorelatetoMTFlag;
            //优先级
            submit.Priority = sb.Priority;
            submit.ExpireTime = sb.ExpireTime;
            submit.ScheduleTime = sb.ScheduleTime;
            //状态报告
            submit.ReportFlag = sb.ReportFlag;
            submit.TP_pid = sb.TP_pid;
            submit.TP_udhi = sb.TP_udhi;
            submit.MessageCoding = sb.MessageCoding;
            submit.MessageType = sb.MessageType;
            submit.LinkID = sb.LinkID;
            submit.WapURL = sb.WapURL;
            submit.MessageContent = sb.MessageContent;
            submit.UserNumber = sb.UserNumber;

            SlidingWindow _send = new SlidingWindow();
            string serial = submit.Header.SrcNodeSequence.ToString() + submit.Header.DateSequence.ToString() + submit.Header.Sequence_Id.ToString();
            SMS tsms = new SMS();
            tsms.Account = sms.Account;
            tsms.Audit = sms.Audit;
            tsms.Channel = sms.Channel;
            tsms.Content = submit.MessageContent;
            tsms.Filter = sms.Filter;
            tsms.Level = sms.Level;
            tsms.Number = sms.Number;
            tsms.SendTime = sms.SendTime;
            tsms.SerialNumber = sms.SerialNumber;
            tsms.StatusReport = sms.StatusReport;
            tsms.Signature = sms.Signature;
            tsms.SPNumber = sms.SPNumber;
            tsms.WapURL = sms.WapURL;

            _send.Report = new SendEventArgs(tsms, serial, false, ((ushort)PlatformCode.SGIP + (ushort)SystemCode.SendReady), "", (ushort)submit.Pk_total, (ushort)submit.Pk_number);
            _send.MSG = submit;
            _send.SendCount = 0;
            _send.SendTime = DateTime.Now;
            _send.Status = WindowStatus.Fill;

            while (true)
            {
                if (Ready == 0)
                {
                    Thread.Sleep(10);
                    continue;
                }
                SWSet(_send);
                ClientSend(_send.MSG);
                DebugLog.Instance.Write<SGIP_SUBMIT>("Client SendSubmit -> SGIP_SUBMIT", (SGIP_SUBMIT)_send.MSG);
                break;
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
                    ClientSend(send.MSG);
                    DebugLog.Instance.Write<SGIP_SUBMIT>("Client Send -> SGIP_SUBMIT", (SGIP_SUBMIT)send.MSG);
                    break;
                }
            }
        }

        private List<SlidingWindow> GetSend(SMS sms)
        {
            List<SlidingWindow> submits = new List<SlidingWindow>();

            //拆分长消息
            int pkSize;
            int pkNum = SMSSplit.GetSplitNumber(sms.Content, sms.Signature,out pkSize);
            if (pkNum > 1)
            {
                for (int j = 0; j < sms.Number.Count; j++)
                {
                    string content = sms.Content;
                    //发送号码
                    for (int i = 0; i < pkNum; i++)
                    {
                        SGIP_SUBMIT submit = new SGIP_SUBMIT(Sequence.Instance.CreateID());

                        submit.Pk_total = (byte)pkNum;
                        submit.Pk_number = (byte)(i + 1);
                        //SPID
                        submit.SPNumber = _setting.SrcID + sms.SPNumber;
                        //被计费用户号码
                        submit.ChargeNumber = Setting.ChargeNumber;
                        submit.UserCount = 1;
                        submit.CorpId = Setting.CorpId;
                        submit.ServiceType = Setting.ServiceType;
                        submit.FeeType = Setting.FeeType;
                        submit.FeeValue = Setting.FeeValue;
                        //赠送用户的话费
                        submit.GivenValue = "0";
                        //代收费标志
                        submit.AgentFlag = (uint)SubmitAgentFlag.RealIncome;
                        submit.MorelatetoMTFlag = 2;
                        //优先级
                        submit.Priority = 0; //sms.Level;
                        submit.ExpireTime = "";// Setting.ExpireTime;
                        submit.ScheduleTime = "";// sms.SendTime.ToString();
                        //状态报告
                        submit.ReportFlag = (byte)1;//sms.StatusReport == StatusReportType.Disable ? (byte)2 : (byte)1;
                        submit.TP_pid = 0;
                        submit.TP_udhi = 1;
                        submit.MessageCoding = 8;
                        submit.MessageType = 0;
                        submit.LinkID = "";
                        submit.WapURL = sms.WapURL;

                        submit.MessageContent = SMSSplit.GetSubString(content, pkSize);
                        content = content.Substring(submit.MessageContent.Length);
                        if (submit.Pk_number == pkNum)
                        {
                            submit.MessageContent = submit.MessageContent + sms.Signature;
                        }
                        submit.SetHeader();

                        submit.UserNumber = sms.Number[j];
                        if (submit.UserNumber.Length == 11)
                            submit.UserNumber = "86" + submit.UserNumber;

                        SlidingWindow _send = new SlidingWindow();
                        string serial = submit.Header.SrcNodeSequence.ToString() + submit.Header.DateSequence.ToString() + submit.Header.Sequence_Id.ToString();
                        SMS tsms = new SMS();
                        tsms.Account = sms.Account;
                        tsms.Audit = sms.Audit;
                        tsms.Channel = sms.Channel;
                        tsms.Content = submit.MessageContent;
                        tsms.Filter = sms.Filter;
                        tsms.Level = sms.Level;
                        tsms.Number = sms.Number;
                        tsms.SendTime = sms.SendTime;
                        tsms.SerialNumber = sms.SerialNumber;
                        tsms.StatusReport = sms.StatusReport;
                        tsms.Signature = sms.Signature;
                        tsms.SPNumber = sms.SPNumber;
                        tsms.WapURL = sms.WapURL;

                        _send.Report = new SendEventArgs( tsms, serial, false,((ushort)PlatformCode.SGIP + (ushort)SystemCode.SendReady), "", (ushort)submit.Pk_total, (ushort)submit.Pk_number);
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
                    //发送号码
                    SGIP_SUBMIT submit = new SGIP_SUBMIT(Sequence.Instance.CreateID());

                    submit.Pk_total = 1;
                    submit.Pk_number = 1;
                    //SPID
                    submit.SPNumber = _setting.SrcID + sms.SPNumber;
                    //被计费用户号码
                    submit.ChargeNumber = Setting.ChargeNumber;
                    submit.UserCount = 1;
                    submit.CorpId = Setting.CorpId;
                    submit.ServiceType = Setting.ServiceType;
                    submit.FeeType = Setting.FeeType;
                    submit.FeeValue = Setting.FeeValue;
                    //赠送用户的话费
                    submit.GivenValue = "0";
                    //代收费标志
                    submit.AgentFlag = (uint)SubmitAgentFlag.RealIncome;
                    submit.MorelatetoMTFlag = 2;
                    //优先级
                    submit.Priority = 0; //sms.Level;
                    submit.ExpireTime = "";//Setting.ExpireTime;
                    submit.ScheduleTime = "";//sms.SendTime.ToString();
                    //状态报告
                    submit.ReportFlag = sms.StatusReport == StatusReportType.Disable ? (byte)2 : (byte)1;
                    submit.TP_pid = 0;
                    submit.TP_udhi = 0;
                    submit.MessageCoding = 15;
                    submit.MessageType = 0;
                    submit.LinkID = "";
                    submit.WapURL = sms.WapURL;

                    submit.MessageContent = sms.Content + sms.Signature;
                    submit.SetHeader();

                    submit.UserNumber = sms.Number[j];
                    if (submit.UserNumber.Length == 11)
                        submit.UserNumber = "86" + submit.UserNumber;

                    SlidingWindow _send = new SlidingWindow();
                    string serial = submit.Header.SrcNodeSequence.ToString() + submit.Header.DateSequence.ToString() + submit.Header.Sequence_Id.ToString();
                    SMS tsms = new SMS();
                    tsms.Account = sms.Account;
                    tsms.Audit = sms.Audit;
                    tsms.Channel = sms.Channel;
                    tsms.Content = submit.MessageContent;
                    tsms.Filter = sms.Filter;
                    tsms.Level = sms.Level;
                    tsms.Number = sms.Number;
                    tsms.SendTime = sms.SendTime;
                    tsms.SerialNumber = sms.SerialNumber;
                    tsms.StatusReport = sms.StatusReport;
                    tsms.Signature = sms.Signature;
                    tsms.SPNumber = sms.SPNumber;
                    tsms.WapURL = sms.WapURL;

                    _send.Report = new SendEventArgs( tsms, serial, false, ((ushort)PlatformCode.SGIP + (ushort)SystemCode.SendReady), "", (ushort)submit.Pk_total, (ushort)submit.Pk_number);
                    _send.MSG = submit;
                    _send.SendCount = 0;
                    _send.SendTime = DateTime.Now;
                    _send.Status = WindowStatus.Fill;
                    submits.Add(_send);
                }
            }

            return submits;
        }

        public int Ready
        {
            get
            {
                int count = _setting.SlidingWindowSize - _sendsCount;
                if (count < 0) count = 0;
                return count;
            }
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
                _server.Dispose();
            }
        }


        public void Close()
        {
            this.Dispose();
        }
    }
}
