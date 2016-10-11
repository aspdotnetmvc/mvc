using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    public enum FilterType : ushort
    {
        /// <summary>
        /// 无操作
        /// </summary>
        NoOperation =0,
        /// <summary>
        /// 替换
        /// </summary>
        Replace = 1,
        /// <summary>
        /// 发送失败
        /// </summary>
        Failure = 2,
    }
}
