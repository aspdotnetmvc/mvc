using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIGP
{
    public class SIGPSetting
    {
        public SIGPSetting()
        {
            _strSPID = "915044";
            _strPassword = "915044";
            _Ip = "192.168.2.231";
            _Port = 7891;
            _gateway = "001";

            //////////////////
            //业务标识
            ServiceID = "0101WFGJJ0";
            //计费类型
            FeeUserType = 2;
            //被计费号码
            FeeTerminalID = "915044";
            //被计费号码类型
            FeeTerminalType = 0;
            //GSM协议类型
            TPPID = 0;
            //GSM协议类型
            TPUdhi = 0;
            //信息格式
            MsgFmt = 15;
            //资费类别
            FeeType = "02";
            //字符代码
            FeeCode = "10";
            //真实号码
            DestTerminalType = 0;
            SrcID = "10658666";
            //存活有效期
            ValidTime = DateTime.Now.ToString();
            LinkID = "";
        }

        private readonly string _strSPID;
        private readonly string _strPassword;
        private readonly string _Ip;
        private readonly int _Port;
        private readonly int _slidingWindowSize = 16;
        private int _sendCount = 3;
        private int _timeOut = 1;
        private int _sendSpan = 10;
        string _serviceID;
        byte _feeUserType;
        string feeTerminalID;
        byte _feeTerminalType;
        byte _TPPID;
        byte _TPUdhi;
        string _feeType;
        string _feeCode;
        private string _validTime;
        byte _destTerminalType;
        string _srcID;
        string _linkID;
        string _gateway;

        private int _activeTestSpan = 10;//150;

        /// <summary>
        /// ACTIVETEST 的时间间隔（C，以秒为单位；标准为 3 分钟）。
        /// </summary>
        /// <remarks>
        /// 当信道上没有数据传输时，通信双方应每隔时间 C 发送链路检测包以维持此连接。
        /// </remarks>
        public int ActiveTestSpan
        {
            get { return _activeTestSpan; }
            set { _activeTestSpan = value; }
        }

        /// <summary>
        /// 网关ID
        /// </summary>
        public string Gateway
        {
            get { return _gateway; }
            set { _gateway = value; }
        }

        /// <summary>
        /// 点播业务使用的LinkID，非点播类业务的MT流程不使用该字段。
        /// </summary>
        public string LinkID
        {
            get { return _linkID; }
            set { _linkID = value; }
        }

        /// <summary>
        /// 源号码（SP的服务代码或前缀为服务代码的长号码, 网关将该号码完整的填到SMPP协议Submit_SM消息相应的source_addr字段，该号码最终在用户手机上显示为短消息的主叫号码）。
        /// </summary>
        public string SrcID
        {
            get { return _srcID; }
            set { _srcID = value; }
        }

        /// <summary>
        /// 接收短信的用户的号码类型(0：真实号码；1：伪码）。
        /// </summary>
        public byte DestTerminalType
        {
            get { return _destTerminalType; }
            set { _destTerminalType = value; }
        }

        /// <summary>
        /// 存活有效期，格式遵循SMPP3.3协议。
        /// </summary>
        public string ValidTime
        {
            get { return _validTime; }
            set { _validTime = value; }
        }

        /// <summary>
        /// 资费代码（以分为单位）。
        /// </summary>
        public string FeeCode
        {
            get { return _feeCode; }
            set { _feeCode = value; }
        }

        /// <summary>
        /// 资费类别（01：对“计费用户号码”免费；02：对“计费用户号码”按条计信息费；03：对“计费用户号码”按包月收取信息费）。
        /// </summary>
        public string FeeType
        {
            get { return _feeType; }
            set { _feeType = value; }
        }

        private byte _msgFmt;

        /// <summary>
        /// 信息格式（0：ASCII串；3：短信写卡操作；4：二进制信息；8：UCS2编码；15：含GB汉字）。
        /// </summary>
        public byte MsgFmt
        {
            get { return _msgFmt; }
            set { _msgFmt = value; }
        }

        /// <summary>
        /// GSM协议类型（详细解释请参考GSM03.40中的9.2.3.9）。
        /// </summary>
        public byte TPPID
        {
            get { return _TPPID; }
            set { _TPPID = value; }
        }

        
        /// <summary>
        /// GSM协议类型（详细是解释请参考GSM03.40中的9.2.3.23,仅使用1位，右对齐）。
        /// </summary>
        public byte TPUdhi
        {
            get { return _TPUdhi; }
            set { _TPUdhi = value; }
        }


        /// <summary>
        /// 被计费用户的号码类型，0：真实号码；1：伪码。
        /// </summary>
        public byte FeeTerminalType
        {
            get { return _feeTerminalType; }
            set { _feeTerminalType = value; }
        }

        /// <summary>
        /// 被计费用户的号码，当Fee_UserType为3时该值有效，当Fee_UserType为0、1、2时该值无意义。
        /// </summary>
        public string FeeTerminalID
        {
            get { return feeTerminalID; }
            set { feeTerminalID = value; }
        }

        /// <summary>
        /// 计费用户类型字段（0：对目的终端MSISDN计费；1：对源终端MSISDN计费；2：对SP计费；3：表示本字段无效，对谁计费参见Fee_terminal_Id字段）。
        /// </summary>
        public byte FeeUserType
        {
            get { return _feeUserType; }
            set { _feeUserType = value; }
        }
        /// <summary>
        /// 业务标识
        /// </summary>
        public string ServiceID
        {
            get { return _serviceID; }
            set { _serviceID = value; }
        }


        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip
        {
            get { return _Ip; }
        } 

        /// <summary>
        /// Ip端口号
        /// </summary>
        public int Port
        {
            get { return _Port; }
        }
        /// <summary>
        /// SP 密码。
        /// </summary>
        public string Password
        {
            get { return _strPassword; }
        }
        /// <summary>
        /// SP 企业代码。
        /// </summary>
        public string SPID
        {
            get { return _strSPID; }
        }
        /// <summary>
        /// 滑动窗口大小
        /// </summary>
        public int SlidingWindowSize
        {
            get { return _slidingWindowSize; }
        }
        /// <summary>
        /// 最大发送次数（N）。
        /// </summary>
        /// <remarks>
        /// 网关与 SP 之间、网关之间的消息发送后等待 T 秒后未收到响应，应立即重发，再连续发送 N-1 次后仍未得到响应则停发。
        /// </remarks>
        public int SendCount
        {
            get { return _sendCount; }
            set { _sendCount = value; }
        }

        /// <summary>
        /// 响应超时时间（T,以秒为单位）。
        /// </summary>
        /// <remarks>
        /// 网关与 SP 之间、网关之间的消息发送后等待 T 秒后未收到响应，应立即重发，再连续发送 N-1 次后仍未得到响应则停发。
        /// </remarks>
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        /// <summary>
        /// TcpClient 发送间隔，以毫秒为单位。
        /// </summary>
        public int SendSpan
        {
            get { return _sendSpan; }
            set { _sendSpan = value; }
        }
    }
}
