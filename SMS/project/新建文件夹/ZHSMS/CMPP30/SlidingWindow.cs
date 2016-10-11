using GatewayInterface;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CMPP
{
    internal enum WindowStatus:short
    {
        /// <summary>
        /// 空闲
        /// </summary>
        Idle = 0,
        /// <summary>
        /// 填充
        /// </summary>
        Fill =1,
    }

    internal struct SlidingWindow
    {
        internal SendEventArgs Report;
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
        /// 状态
        /// </summary>
        internal WindowStatus Status;
    }
}
