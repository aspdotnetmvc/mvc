using GatewayInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    internal struct SEND
    {
        internal SendReportEventArgs Report;
        /// 数据
        /// </summary>
        internal ISGIP_MESSAGE MSG;
        /// <summary>
        /// 数据包发送时间。
        /// </summary>
        internal DateTime SendTime;
        /// <summary>
        /// 发送次数。
        /// </summary>
        internal int SendCount;
        /// <summary>
        /// 数据包状态（0：空，1：已发送）。
        /// </summary>
        internal int Status;
    }
}
