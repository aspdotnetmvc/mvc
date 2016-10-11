using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class MOSMS
    {
        public MOSMS()
        {

        }
        public MOSMS(string gateway,string serial, DateTime receiveTime, string message, string userNumber, string spNumber,string service)
        {
            _service = service;
            _SPNumber = spNumber;
            _userNumber = userNumber;
            _message = message;
            _gateway = gateway;
            _serial = serial;
            _receiveTime = receiveTime;
        }

        private string _serial;
        private string _gateway;
        private DateTime _receiveTime;
        private string _message;
        private string _service;

        /// <summary>
        /// 服务类型
        /// </summary>
        public string Service
        {
            get { return _service; }
            set { _service = value; }
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        private string _userNumber;
        /// <summary>
        /// 发送手机号码
        /// </summary>
        public string UserNumber
        {
            get { return _userNumber; }
            set { _userNumber = value; }
        }
        private string _SPNumber;
        /// <summary>
        /// 接收的SP号码
        /// </summary>
        public string SPNumber
        {
            get { return _SPNumber; }
            set { _SPNumber = value; }
        }

        //接收时间
        public DateTime ReceiveTime
        {
            get { return _receiveTime; }
            set { _receiveTime = value; }
        }

        /// <summary>
        /// 网关流水号
        /// </summary>
        public string Serial
        {
            get { return _serial; }
            set { _serial = value; }
        }

        //网关
        public string Gateway
        {
            get { return _gateway; }
            set { _gateway = value; }
        }

        public string LinkID { get; set; }
    }
}
