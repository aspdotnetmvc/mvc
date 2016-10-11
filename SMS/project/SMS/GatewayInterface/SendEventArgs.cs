using SMS.Model;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayInterface
{
    [Serializable]
    public class SendEventArgs : EventArgs
    {
        private PlainSMS _message;
        private string _serial;
        private ushort _statusCode;

        private bool _succeed;


        /// <summary>
        /// 状态码
        /// </summary>
        public ushort StatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        //发送的短消息
        public PlainSMS Message
        {
            get { return _message; }
        }
        public List<string> Numbers
        {
            get;
            private set;
        }
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime SubmitTime
        {
            get;
            set;
        }
        //网关返回的序列号
        public string SerialNumber
        {
            get { return _serial; }
            set { _serial = value; }
        }
        //状态描述
        public string Description
        {
            get;
            set;
        }
        //发送状态
        public bool Succeed
        {
            get { return _succeed; }
            set { _succeed = value; }
        }
        public SendEventArgs(PlainSMS message, string serial, bool succeed, ushort statusCode, string description)
        {
            _statusCode = statusCode;
            _message = message;
            Numbers = message.Numbers.Split(',').ToList();
            _serial = serial;
            Description = description;
            _succeed = succeed;
            SubmitTime = DateTime.Now;
        }
    }
}
