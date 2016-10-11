using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    [Serializable]
    public class StatusReport
    {
        /// <summary>
        /// 网关返回的序列号
        /// </summary>
        public string SerialNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Number
        {
            get;
            set;
        }
        /// <summary>
        /// 短信的Id
        /// </summary>
        public string SMSID { get; set; }

        /// <summary>
        /// 短信发送日期
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        public int StatusCode
        {
            get;
            set;
        }
        /// <summary>
        /// 状态描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 是否发送成功
        /// </summary>
        public bool Succeed
        {
            get;
            set;
        }
        /// <summary>
        /// 状态报告返回时间
        /// </summary>
        public DateTime? ResponseTime
        {
            get;
            set;
        }
        //网关的ID
        public string Gateway
        {
            get;
            set;
        }
        public string Channel
        {
            get;
            set;
        }
    }
}
