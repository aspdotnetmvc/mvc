using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    public enum TempletAuditType : ushort
    {
        /// <summary>
        /// 待审核
        /// </summary>
        NoAudit = 0,
        /// <summary>
        /// 审核成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 审核失败
        /// </summary>
        Failure = 2,
    }
}
