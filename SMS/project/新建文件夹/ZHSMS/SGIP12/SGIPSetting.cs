using BXM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    public class SGIPSetting
    {
        public SGIPSetting()
        {
        }

        #region 网络
        private string _loginName;
        private string _password;
        private string _ip;
        private string _localIp;
        private int _port;
        private int _localPort;
        private int _slidingWindowSize = 1;
        private int _sendCount = 3;
        //单位秒
        private int _timeOut = 30;
        //单位毫秒
        private int _sendSpan = 500;

        /// <summary>
        /// IP地址
        /// </summary>
        public string ServerIp
        {
            get { return _ip; }
            set { _ip = value; }
        }
        /// <summary>
        /// Ip端口号
        /// </summary>
        public int ServerPort
        {
            get { return _port; }
            set { _port = value; }
        }
        /// <summary>
        /// 服务端IP地址
        /// </summary>
        public string LocalIP
        {
            get { return _localIp; }
            set { _localIp = value; }
        }
        /// <summary>
        /// 服务端端口
        /// </summary>
        public int LocalPort
        {
            get { return _localPort; }
            set { _localPort = value; }
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
        #endregion

        #region Submit
        string _serviceType;
        uint _feeType;
        string _chargeNumber;
        string _feeValue;
        string _expireTime;
        string _corpId;
        string _srcID;
        #endregion

        uint _srcNodeSequence;
        /// <summary>
        /// 源节点编号
        /// </summary>
        public uint SrcNodeSequence
        {
            get { return _srcNodeSequence; }
            set { _srcNodeSequence = value; }
        }

        //企业ID*
        public string CorpId
        {
            get { return _corpId; }
            set { _corpId = value; }
        }


        //短信的收费值*
        public string FeeValue
        {
            get { return _feeValue; }
            set { _feeValue = value; }
        }

        /// <summary>
        /// 计费类型*
        /// </summary>
        public uint FeeType
        {
            get { return _feeType; }
            set { _feeType = value; }
        }

        /// <summary>
        /// 付费号码，加"86"国别标志*
        /// </summary>
        public string ChargeNumber
        {
            get { return _chargeNumber; }
            set {
                _chargeNumber = value;
                if (_chargeNumber.Length == 11)
                    _chargeNumber = "86" + _chargeNumber;
            }
        }

        /// <summary>
        /// 业务标识*
        /// </summary>
        public string ServiceType
        {
            get { return _serviceType; }
            set { _serviceType = value; }
        }

        /// <summary>
        /// SP 登陆用户名。*
        /// </summary>
        public string LoginName
        {
            get { return _loginName; }
            set { _loginName = value; }
        }
        /// <summary>
        /// 源终端号码，运行商分配的短信通道号
        /// </summary>
        public string SrcID
        {
            get { return _srcID; }
            set { _srcID = value; }
        }

        //本地登陆用户名
        public string LocalLoginName { get; set; }
        //本地登陆密码
        public string LocalLoginPassword { get; set; }
    }
}
