using GatewayInterface;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CMPP
{
    internal struct SEND
    {
        internal SendReportEventArgs Report;
        /// 数据
        /// </summary>
        internal ICMPP_MESSAGE MSG;
        /// <summary>
        /// 数据包发送时间。
        /// </summary>
        internal DateTime SendTime;
        /// <summary>
        /// 发送次数。
        /// </summary>
        internal int SendCount;
        /// <summary>
        /// 状态（0：空闲，1：使用）。
        /// </summary>
        internal int Status;
    }
}
