using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    public enum AnnunciateType : ushort
    {
        /// <summary>
        /// 全体可见
        /// </summary>
        All = 0,
        /// <summary>
        /// 代理商可见
        /// </summary>
        Agent = 1,
        /// <summary>
        /// 终端客户可见
        /// </summary>
        NoAgent = 2,
        /// <summary>
        /// 部分用户可见
        /// </summary>
        Person=3,
        /// <summary>
        /// 某角色用户可见
        /// </summary>
        Role=4,

    }
}
