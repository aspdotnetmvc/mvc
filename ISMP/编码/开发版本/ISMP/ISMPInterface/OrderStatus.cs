using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 到期
        /// </summary>
        Expire,
        /// <summary>
        /// 余额告警
        /// </summary>
        BalanceAlarm,
        /// <summary>
        /// 欠款
        /// </summary>
        Owed,
        /// <summary>
        /// 清零
        /// </summary>
        BalancesToZero,
        /// <summary>
        /// 禁用
        /// </summary>
        Disabled
    }
}
