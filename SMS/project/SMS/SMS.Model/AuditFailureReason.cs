using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    /// <summary>
    /// 审核失败原因
    /// </summary>
    [Serializable]
    public class AuditFailureReason
    {
        /// <summary>
        /// 审核失败原因
        /// </summary>
        public string FailureReason { get; set; }
        /// <summary>
        /// 排序序号
        /// </summary>
        public int OrderNum { get; set; }
    }
}
