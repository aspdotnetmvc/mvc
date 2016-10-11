using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class MOSMS
    {
        
        /// <summary>
        /// 服务类型
        /// </summary>
        public string Service
        {
            get;
            set;
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message
        {
            get;
            set;
        }
        /// <summary>
        /// 发送手机号码
        /// </summary>
        public string UserNumber
        {
            get;
            set;
        }
    
        /// <summary>
        /// 接收的SP号码
        /// </summary>
        public string SPNumber
        {
            get;
            set;
        }
        public string AccountID { get; set; }
        //接收时间
        public DateTime ReceiveTime
        {
            get;
            set;
        }

        /// <summary>
        /// 网关流水号
        /// </summary>
        public string SerialNumber
        {
            get;
            set;
        }

        //网关
        public string Gateway
        {
            get;
            set;
        }

        public int Status { get; set; }
    }
}
