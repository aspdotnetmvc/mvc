using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    public enum AccountAuditType:ushort
    {
        /// <summary>
        /// 人工审核
        /// </summary>
        Manual = 0,
        /// <summary>
        /// 自动
        /// </summary>
        Auto = 1,
    }
}
