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

namespace CMPP
{
    public class CMPP30 : ISMSGateway, IDisposable
    {
        ISocketAsyncClient _client;
        CMPPSetting _setting;

        SlidingWindow[] _sendWindows;
        int _sendsCount = 0;

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

        #region 常量
        /// <summary>
        /// CMPP 版本。
        /// </summary>
        public const byte CMPP_VERSION_30 = 0x30;

        #region SMS数据格式定义
        /// <summary>
        /// ASCII 串。
        /// </summary>
        public const byte CODING_ASCII = 0;
        /// <summary>
        /// 二进制信息。
        /// </summary>
        public const byte CODING_BINARY = 4;
        /// <summary>
        /// UCS2编码。
        /// </summary>
        public const byte CODING_UCS2 = 8;
        /// <summary>
        /// 含GB汉字。
        /// </summary>
        public const byte CODING_GBK = 15;
        #endregion

        #endregion


        public CMPP30(CMPPSetting setting)
        {
            _setting = setting;
            _client = new SocketClient(_setting.Ip, _setting.Port, 1024);
            _client.DataReceived += _sc_DataReceived;
            _client.Closed += _sc_Closed;
            _client.Error += _sc_Error;

            _sendWindows = new SlidingWindow[_setting.SlidingWindowSize];

            //启动相关线程
            _monitorConnectThread = new Thread(new ThreadStart(MonitorConnectRun));
            _monitorConnectThread.Start();
            _monitorTimeOutThread = new Thread(new ThreadStart(MonitorTimeOutRun));
            _monitorTimeOutThread.Start();
        }


        #region 网络操作
        void _sc_Error(Exception error)
        {
            //DebugLog.Instance.Write("sc_Error -> " + error.ToString());
            MessageTools.MessageHelper.Instance.WirteError("sc_error", error);
        }

        void _sc_Closed()
        {
            //DebugLog.Instance.Write("sc_Closed -> ");
            MessageTools.MessageHelper.Instance.WirteInfo("sc_Closed");

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
                        MessageTools.MessageHelper.Instance.WirteTest("调用Active_Test()");
                        Active_Test();
                    }

                    for (int i = 0; i < _sendWindows.Length; i++)
                    {
                        //发送超时
                        if (_sendWindows[i].Status == WindowStatus.Fill)
                        {
                            if ((DateTime.Now - _sendWindows[i].SendTime).TotalSeconds >= _setting.TimeOut)
                            {
                                //if (_sendWindows[i].MSG.Command == CMPP_COMMAND.CMD_ACTIVE_TEST)
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
                                    // DebugLog.Instance.Write<CMPP_SUBMIT>("Client TimeOut -> ", (CMPP_SUBMIT)_sendWindows[i].MSG);
                                    continue;
                                }
                                else
                                {
                                    //超时处理
                                    if (SendEvent != null)
                                    {
                                        _sendWindows[i].Report.StatusCode = ((ushort)PlatformCode.CMPP + (ushort)SystemCode.SendTimeOut);
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
            int headSize = 0;
            Byte[] buffer = null;
            CMPP_HEAD head;

            try
            {
                if (data.Length > 0)
                {
                    buffer = new byte[12];
                    Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
                    head.TotalLength = Convert.ToUInt32(buffer, 0);
                    head.CommandID = Convert.ToUInt32(buffer, 4);
                    head.SequenceID = Convert.ToUInt32(buffer, 8);
                }
                else
                {
                    OnNetworkError(new Exception("Head data error."));
                    return;
                }
            }
            catch (Exception ex)
            {
                OnNetworkError(ex);
                return;
            }

            switch ((CMPP_COMMAND)head.CommandID)
            {
                case CMPP_COMMAND.CMD_SUBMIT_RESP:
                    CMPP_SUBMIT_RESP submit_resp = new CMPP_SUBMIT_RESP();
                    submit_resp.Head = head;

                    try
                    {
                        headSize = Marshal.SizeOf(head);
                        buffer = new byte[head.TotalLength - headSize];
                        Buffer.BlockCopy(data, headSize, buffer, 0, buffer.Length);
                        submit_resp.MsgID = Convert.ToUInt64(buffer, 0);
                        submit_resp.Result = Convert.ToUInt32(buffer, 8);
                    }
                    catch (Exception ex)
                    {
                        OnNetworkError(ex);
                        return;
                    }
                    // DebugLog.Instance.Write<CMPP_SUBMIT_RESP>("Client Received -> CMPP_SUBMIT_RESP", submit_resp);
                    MessageTools.MessageHelper.Instance.WirteInfo("Client Received -> CMPP_SUBMIT_RESP" + submit_resp.ToString());
                    SlidingWindow send = SWGet(head.SequenceID);
                    if (send.Status == WindowStatus.Idle) return;

                    //流控
                    if (submit_resp.Result == 8)
                    {
                        SWClr(head.SequenceID);
                        send.SendTime = DateTime.Now;
                        send.MSG.SequenceID = Sequence.Instance.CreateID();
                        Send(send.MSG);
                        return;
                    }

                    if (SendEvent != null)
                    {
                        send.Report.Succeed = false;
                        switch (submit_resp.Result)
                        {
                            case 0:
                                send.Report.Succeed = true;
                                send.Report.Describe = "等待报告";
                                break;
                            case 1:
                                send.Report.Describe = "消息结构错";
                                break;
                            case 2:
                                send.Report.Describe = "命令字错";
                                break;
                            case 3:
                                send.Report.Describe = "消息序号重复";
                                break;
                            case 4:
                                send.Report.Describe = "消息长度错";
                                break;
                            case 5:
                                send.Report.Describe = "资费代码错";
                                break;
                            case 6:
                                send.Report.Describe = "超过最大信息长";
                                break;
                            case 7:
                                send.Report.Describe = "业务代码错";
                                break;
                            case 8:
                                send.Report.Describe = "流量控制错";
                                break;
                            case 9:
                                send.Report.Describe = "本网关不负责服务此计费号码";
                                break;
                            case 10:
                                send.Report.Describe = "源号码错误";
                                break;
                            case 11:
                                send.Report.Describe = "短信内容错误";
                                break;
                            case 12:
                                send.Report.Describe = "被计费号码错误";
                                break;
                            case 13:
                                send.Report.Describe = "终端号码错误";
                                break;
                        }
                        send.Report.Serial = submit_resp.MsgID.ToString();
                        SendEvent(this, send.Report);
                    }
                    //清除窗口
                    SWClr(head.SequenceID);
                    break;
                case CMPP_COMMAND.CMD_CANCEL_RESP:
                    CMPP_CANCEL_RESP cancel_resp = new CMPP_CANCEL_RESP();
                    cancel_resp.Head = head;
                    try
                    {
                        headSize = Marshal.SizeOf(head);
                        buffer = new byte[head.TotalLength - headSize];
                        Buffer.BlockCopy(data, headSize, buffer, 0, buffer.Length);
                        cancel_resp.SuccessID = Convert.ToUInt32(buffer, 0);
                    }
                    catch (Exception ex)
                    {
                        OnNetworkError(ex);
                        return;
                    }
                    // DebugLog.Instance.Write<CMPP_CANCEL_RESP>("Client Received -> CMPP_CANCEL_RESP", cancel_resp);
                    break;
                case CMPP_COMMAND.CMD_CONNECT_RESP:
                    CMPP_CONNECT_RESP connect_resp = new CMPP_CONNECT_RESP();
                    connect_resp.Head = head;
                    try
                    {
                        headSize = Marshal.SizeOf(head);
                        buffer = new byte[head.TotalLength - headSize];
                        Buffer.BlockCopy(data, headSize, buffer, 0, buffer.Length);
                        connect_resp.Status = Convert.ToUInt32(buffer, 0);
                        connect_resp.AuthenticatorISMG = new Byte[16];
                        Array.Copy(buffer, 4, connect_resp.AuthenticatorISMG, 0, 16);
                        connect_resp.Version = buffer[buffer.Length - 1];
                    }
                    catch (Exception ex)
                    {
                        OnNetworkError(ex);
                        return;
                    }
                    // DebugLog.Instance.Write<CMPP_CONNECT_RESP>("Client Received -> CMPP_CONNECT_RESP", connect_resp);
                    switch (connect_resp.Status)
                    {
                        case 0:
                            _connect = true;
                            autoConnectEvent.Set();
                            OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT, "登录成功"));
                            break;
                        default:
                            switch (connect_resp.Status)
                            {
                                case 1:
                                    OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, "消息结构错"));
                                    break;
                                case 2:
                                    OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, "非法源地址"));
                                    break;
                                case 3:
                                    OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, "认证错"));
                                    break;
                                case 4:
                                    OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, "版本太高"));
                                    break;
                                default:
                                    OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, string.Format("其他错误（错误码：{0}）", connect_resp.Status)));
                                    break;
                            }
                            OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, "错误代码 " + connect_resp.Status));
                            break;
                    }
                    break;
                case CMPP_COMMAND.CMD_ACTIVE_TEST_RESP:
                    CMPP_ACTIVE_TEST_RESP resp = new CMPP_ACTIVE_TEST_RESP();
                    resp.Head = head;

                    try
                    {
                        headSize = Marshal.SizeOf(head);
                        buffer = new byte[head.TotalLength - headSize];
                        Buffer.BlockCopy(data, headSize, buffer, 0, buffer.Length);
                        resp.Reserved = buffer[0];
                    }
                    catch (Exception ex)
                    {
                        OnNetworkError(ex);
                        return;
                    }
                    //     DebugLog.Instance.Write<CMPP_ACTIVE_TEST_RESP>("Client Received -> CMPP_ACTIVE_TEST_RESP", resp);
                    //SWClr(head.SequenceID);
                    break;
                case CMPP_COMMAND.CMD_ACTIVE_TEST:
                   // DebugLog.Instance.Write("Client Received -> CMD_ACTIVE_TEST");
                    MessageTools.MessageHelper.Instance.WirteTest("Client Received -> CMD_ACTIVE_TEST");
                    CMPP_ACTIVE_TEST_RESP active_test_resp = new CMPP_ACTIVE_TEST_RESP();
                    active_test_resp.Head = new CMPP_HEAD();
                    active_test_resp.Head.CommandID = (uint)CMPP_COMMAND.CMD_ACTIVE_TEST_RESP;
                    active_test_resp.Head.SequenceID = head.SequenceID;
                    active_test_resp.Reserved = 0;
                    Send(active_test_resp);
                    MessageTools.MessageHelper.Instance.WirteTest("Client Send -> CMPP_ACTIVE_TEST_RESP");
                    //     DebugLog.Instance.Write<CMPP_ACTIVE_TEST_RESP>("Client Send -> CMPP_ACTIVE_TEST_RESP", active_test_resp);
                    break;
                case CMPP_COMMAND.CMD_DELIVER:
                    CMPP_DELIVER deliver = new CMPP_DELIVER();
                    deliver.Head = head;
                    try
                    {
                        headSize = Marshal.SizeOf(head);
                        buffer = new byte[head.TotalLength - headSize];
                        Buffer.BlockCopy(data, headSize, buffer, 0, buffer.Length);
                        deliver.Init(buffer);
                    }
                    catch (Exception ex)
                    {
                        OnNetworkError(ex);
                        return;
                    }
                    //     DebugLog.Instance.Write<CMPP_DELIVER>("Client Received -> CMPP_DELIVER", deliver);
                    CMPP_DELIVER_RESP deliver_resp = new CMPP_DELIVER_RESP();
                    deliver_resp.Head = new CMPP_HEAD();
                    deliver_resp.Head.CommandID = (uint)CMPP_COMMAND.CMD_DELIVER_RESP;
                    deliver_resp.Head.SequenceID = deliver.Head.SequenceID;
                    deliver_resp.MsgID = deliver.MsgID;
                    deliver_resp.Result = 0;
                    Send(deliver_resp);
                    //   DebugLog.Instance.Write<CMPP_DELIVER_RESP>("Client Send -> CMPP_DELIVER_RESP", deliver_resp);

                    if (deliver.RegisteredDelivery == 1)
                    {
                        ////状态报告
                        if (ReportEvent != null)
                        {
                            CMPP_REPORT report = deliver.GetReport();
                            bool result = false;
                            string resultMessage = "";
                            ushort resultCode = ((ushort)PlatformCode.CMPP + (ushort)SystemCode.ReportBack);

                            DateTime time;
                            try
                            {
                                time = new DateTime(DateTime.Now.Year, System.Convert.ToInt32(report.DoneTime.Substring(2, 2)), System.Convert.ToInt32(report.DoneTime.Substring(4, 2)), System.Convert.ToInt32(report.DoneTime.Substring(6, 2)), System.Convert.ToInt32(report.DoneTime.Substring(8, 2)), 0);
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
                            ReportEventArgs args = new ReportEventArgs(report.MsgID.ToString(), result, resultCode, resultMessage, time);
                            ReportEvent(this, args);
                        }
                    }
                    else
                    {
                        if (DeliverEvent != null)
                        {
                            DeliverEvent(this, new DeliverEventArgs(deliver.MsgID.ToString(), DateTime.Now, deliver.MsgContent, deliver.SrcTerminalID, deliver.DestID, deliver.ServiceID.ToString()));
                        }
                    }
                    break;
                case CMPP_COMMAND.CMPP_QUERY_RESP:
                    CMPP_QUERY_RESP query_resp = new CMPP_QUERY_RESP();
                    query_resp.Head = head;
                    try
                    {
                        headSize = Marshal.SizeOf(head);
                        buffer = new byte[head.TotalLength - headSize];
                        Buffer.BlockCopy(data, headSize, buffer, 0, buffer.Length);
                        query_resp.Init(buffer);
                    }
                    catch (Exception ex)
                    {
                        OnNetworkError(ex);
                        return;
                    }
                    //   DebugLog.Instance.Write<CMPP_QUERY_RESP>("Client Received -> CMPP_QUERY_RESP", query_resp);
                    break;
                case CMPP_COMMAND.CMD_TERMINATE:
                    //      DebugLog.Instance.Write("Client Received -> CMD_TERMINATE");
                    _connect = false;
                    Thread.Sleep(100);
                    break;
                case CMPP_COMMAND.CMD_ERROR:
                    //      DebugLog.Instance.Write("Client Received -> CMD_ERROR");
                    _connect = false;
                    Thread.Sleep(100);
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
            DebugLog.Instance.Write("OnNetworkError -> ", ex.ToString());
            _connect = false;
        }

        /// <summary>
        /// 连接到 ISMG。
        /// </summary>
        public bool Connect()
        {
            _connecting = true;
            _connect = false;

            DateTime dt = DateTime.Now;
            CMPP_CONNECT conn = new CMPP_CONNECT();
            conn.Head = new CMPP_HEAD();
            conn.Head.CommandID = (uint)CMPP_COMMAND.CMD_CONNECT;
            conn.Head.SequenceID = Sequence.Instance.CreateID();
            conn.SourceAddress = _setting.SPID;
            conn.TimeStamp = System.Convert.ToUInt32(string.Format("{0:MMddhhmmss}", dt));
            conn.AuthenticatorSource = CreateDigest(dt);
            conn.Version = CMPP_VERSION_30;

            byte[] bytes = conn.GetBytes();

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
                Thread.Sleep(_setting.SendSpan);
                _client.Send(bytes);
                //  DebugLog.Instance.Write<CMPP_CONNECT>("Client Send -> CMPP_CONNECT", conn);
            }
            catch (Exception ex)
            {
                //  DebugLog.Instance.Write("Client Send Error -> CMPP_CONNECT", ex.ToString());
                MessageTools.MessageHelper.Instance.WirteError("Client Send Error -> CMPP_CONNECT", ex);

                _connecting = false;
                return false;
            }

            autoConnectEvent.WaitOne(_setting.TimeOut * 1000, false);

            if (!_connect)
            {
                _client.Close();
                OnSMSEvent(new SMSEventArgs(SMS_Event.SP_CONNECT_ERROR, "连接服务器超时"));
                _connecting = false;
                return false;
            }
            _connecting = false;
            return _connect;
        }

        /// <summary>
        /// 计算 CMPP_CONNECT 包的 AuthenticatorSource 字段。
        /// </summary>
        /// <remarks>
        /// MD5(Source_Addr + 9字节的0 + shared secret + timestamp);
        /// </remarks>
        private byte[] CreateDigest(DateTime dt)
        {
            byte[] btContent = new byte[25 + _setting.Password.Length];
            Array.Clear(btContent, 0, btContent.Length);

            // Source_Addr，SP的企业代码（6位）。
            int iPos = 0;
            foreach (char ch in _setting.SPID)
            {
                btContent[iPos] = (byte)ch;
                iPos++;
            }

            // 9字节的0。
            iPos += 9;

            // password，由 China Mobile 提供（长度不固定）。
            foreach (char ch in _setting.Password)
            {
                btContent[iPos] = (byte)ch;
                iPos++;
            }

            // 时间戳（10位）。
            foreach (char ch in string.Format("{0:MMddhhmmss}", dt))
            {
                btContent[iPos] = (byte)ch;
                iPos++;
            }
            return new MD5CryptoServiceProvider().ComputeHash(btContent);
        }

        #region 发送
        private void Send(ICMPP_MESSAGE message)
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
            CMPP_HEAD Head = new CMPP_HEAD();
            Head.TotalLength = 12;
            Head.CommandID = (uint)CMPP_COMMAND.CMD_ACTIVE_TEST;
            Head.SequenceID = Sequence.Instance.CreateID();
            CMPP_ACTIVE_TEST active_test = new CMPP_ACTIVE_TEST(Head);

            //SlidingWindow send = new SlidingWindow();
            //send.SendCount = 0;
            //send.SendTime = DateTime.Now;
            //send.MSG = active_test;
            //send.Status = WindowStatus.Fill;
            //SWSet(send);

            Send(active_test);
            MessageTools.MessageHelper.Instance.WirteTest("Client Send -> CMPP_ACTIVE_TEST");

           // DebugLog.Instance.Write<CMPP_ACTIVE_TEST>("Client Send -> CMPP_ACTIVE_TEST", active_test);
        }

        public void Cancel(ulong msgID)
        {
            CMPP_CANCEL cancel = new CMPP_CANCEL();
            cancel.Head = new CMPP_HEAD();
            cancel.Head.CommandID = (uint)CMPP_COMMAND.CMD_CANCEL;
            cancel.Head.SequenceID = Sequence.Instance.CreateID();
            cancel.MsgID = msgID;

            Send(cancel);

           // DebugLog.Instance.Write<CMPP_CANCEL>("Client Send -> CMPP_CANCEL", cancel);
        }
        #endregion

        private int Ready
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
                 //   DebugLog.Instance.Write<CMPP_SUBMIT>("Send -> CMPP_SUBMIT", (CMPP_SUBMIT)send.MSG);
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
                        CMPP_SUBMIT submit = new CMPP_SUBMIT();
                        submit.Head = new CMPP_HEAD();
                        submit.Head.CommandID = (uint)CMPP_COMMAND.CMD_SUBMIT;
                        submit.Head.SequenceID = Sequence.Instance.CreateID();

                        submit.MsgID = 0;
                        submit.SrcID = _setting.SrcID + sms.SPNumber;

                        submit.PkTotal = (byte)pkNum;
                        submit.PkNumber = (byte)(i + 1);
                        submit.ServiceID = _setting.ServiceID;
                        submit.FeeUserType = _setting.FeeUserType;
                        submit.FeeTerminalID = _setting.FeeTerminalID;
                        submit.FeeTerminalType = _setting.FeeTerminalType;
                        submit.TPPID = 0;

                        submit.MsgFmt = 8;
                        submit.FeeType = _setting.FeeType;
                        submit.FeeCode = _setting.FeeCode;
                        submit.DestTerminalType = _setting.DestTerminalType;
                        submit.MsgLevel = 1;
                        submit.MsgSrc = _setting.SPID;
                        submit.ValidTime = _setting.ValidTime;
                        submit.AtTime = "";
                        submit.RegisteredDelivery = sms.StatusReport == StatusReportType.Disable ? (byte)0 : (byte)1;

                        submit.LinkID = "";

                        //信息内容
                        submit.MsgContent = SMSSplit.GetSubString(content, pkSize);
                        content = content.Substring(submit.MsgContent.Length);
                        if (submit.PkNumber == pkNum)
                        {
                            submit.MsgContent = submit.MsgContent + sms.Signature;
                        }
                        submit.SetHeader();

                        //信息内容长度
                        submit.MsgLength = Convert.Length(submit.MsgContent, submit.MsgFmt);

                        submit.DestTerminalID = new string[] { sms.Number[j] };
                        submit.DestUsrTl = (byte)1;

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
                        _send.Report = new SendEventArgs(tsms, "", false, ((ushort)PlatformCode.CMPP + (ushort)SystemCode.SendReady), "", (ushort)submit.PkTotal, (ushort)submit.PkNumber);
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
                    CMPP_SUBMIT submit = new CMPP_SUBMIT();
                    submit.Head = new CMPP_HEAD();
                    submit.Head.CommandID = (uint)CMPP_COMMAND.CMD_SUBMIT;
                    submit.Head.SequenceID = Sequence.Instance.CreateID();

                    submit.MsgID = 0;
                    submit.SrcID = _setting.SrcID + sms.SPNumber;
                    submit.PkTotal = 1;
                    submit.PkNumber = 1;
                    submit.ServiceID = _setting.ServiceID;
                    submit.FeeUserType = _setting.FeeUserType;
                    submit.FeeTerminalID = _setting.FeeTerminalID;
                    submit.FeeTerminalType = _setting.FeeTerminalType;
                    submit.TPPID = 0;
                    submit.TPUdhi = 0;
                    submit.MsgFmt = 15;
                    submit.FeeType = _setting.FeeType;
                    submit.FeeCode = _setting.FeeCode;
                    submit.DestTerminalType = _setting.DestTerminalType;
                    submit.MsgLevel = 1;
                    submit.MsgSrc = _setting.SPID;
                    submit.ValidTime = _setting.ValidTime;
                    submit.AtTime = "";
                    submit.RegisteredDelivery = sms.StatusReport == StatusReportType.Disable ? (byte)0 : (byte)1;

                    submit.LinkID = "";

                    //信息内容
                    submit.MsgContent = sms.Content + sms.Signature;
                    //信息内容长度
                    submit.MsgLength = Convert.Length(submit.MsgContent, submit.MsgFmt);

                    submit.DestTerminalID = new string[] { sms.Number[j] };
                    submit.DestUsrTl = (byte)1;

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
                    _send.Report = new SendEventArgs(sms, "", false, ((ushort)PlatformCode.CMPP + (ushort)SystemCode.SendReady), "", submit.PkTotal, submit.PkNumber);
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

        public void SendSubmit(SMS sms)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            this.Dispose();
        }
    }
}
