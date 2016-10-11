using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    public enum SysPlatType:ushort
    {
        /// <summary>
        /// 管理平台
        /// </summary>
        SysPlat=0,
        /// <summary>
        /// 直客端平台
        /// </summary>
        ZKDPlat=1,
    }
}
