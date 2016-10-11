using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    /// <summary>
    /// 运营商类别,因历史原因，本系统中不使用int值，而使用string值
    /// </summary>
    public enum OperatorType
    {
        /// <summary>
        /// 未定义
        /// </summary>
        notdefined = 0,
        /// <summary>
        /// 电信
        /// </summary>
        telecom = 1,
        /// <summary>
        /// 移动
        /// </summary>
        mobile = 2,
        /// <summary>
        /// 联通
        /// </summary>
        unicom = 3
    }
}
