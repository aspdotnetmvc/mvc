using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    public enum AccountAuditType
    {
        /// <summary>
        /// 人工审核
        /// </summary>
        Audit = 0,
        /// <summary>
        /// 免审
        /// </summary>
        NonAudit = 1,
        /// <summary>
        /// 单条免审
        /// </summary>
        SingleNonAudit = 2,
        /// <summary>
        /// 通过接口发送时免审
        /// </summary>
        InterfaceNonAudit = 3
    }
}
