using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    [Serializable]
    public class AuditRecord
    {
        /// <summary>
        /// 审核者
        /// </summary>
        public string AccountID { get; set; }
        /// <summary>
        /// 短信业务号
        /// </summary>
        public Guid SerialNumber { get; set; }
        /// <summary>
        /// 短信提交的时间
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditTime { get; set; }
        /// <summary>
        /// 审核结果
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string Content { get; set; }
    }
}
