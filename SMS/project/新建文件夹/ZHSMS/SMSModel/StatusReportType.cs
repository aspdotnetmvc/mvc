using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    public enum StatusReportType:ushort
    {
        /// <summary>
        /// 启用
        /// </summary>
        Enabled = 1,
        /// <summary>
        /// 不启用
        /// </summary>
        Disable = 0,
        /// <summary>
        /// 推送
        /// </summary>
        Push=2,
    }
}
