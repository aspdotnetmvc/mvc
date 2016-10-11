using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMPP
{
    public class CMPPSetting
    {
        public CMPPSetting()
        {
        }

        private string _spid;
        private string _password;
        private string _ip;
        private int _port;
        private int _slidingWindowSize = 16;
        private int _sendCount = 3;
        private int _timeOut = 30;
        private int _sendSpan = 500;
        string _serviceID;
        byte _feeUserType;
        string feeTerminalID;
        byte _feeTerminalType;
        string _feeType;
        string _feeCode;
        string _validTime;
        byte _destTerminalType;
        string _srcID;
        private int _activeTestSpan = 5;//150;

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
            get { return _ip; }
            set { _ip = value; }
        } 

        /// <summary>
        /// Ip端口号
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        /// <summary>
        /// SP 密码。
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        /// <summary>
        /// SP 企业代码。
        /// </summary>
        public string SPID
        {
            get { return _spid; }
            set { _spid = value; }
        }
        /// <summary>
        /// 滑动窗口大小
        /// </summary>
        public int SlidingWindowSize
        {
            get { return _slidingWindowSize; }
            set { _slidingWindowSize = value; }
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
        /// <summary>
        /// 源终端号码，运行商分配的短信通道号
        /// </summary>
        public string SrcID
        {
            get { return _srcID; }
            set { _srcID = value; }
        }
    }
}
