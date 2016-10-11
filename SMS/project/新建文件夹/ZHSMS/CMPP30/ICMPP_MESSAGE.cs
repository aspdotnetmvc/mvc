using System;
using System.Collections.Generic;
using System.Text;

namespace CMPP
{
    /// <summary>
    /// CMPP 数据包。
    /// </summary>
    public interface ICMPP_MESSAGE
    {
        /// <summary>
        /// 流水号
        /// </summary>
        /// <returns></returns>
        uint SequenceID { get; set; }
        /// <summary>
        /// 指令
        /// </summary>
        /// <returns></returns>
        CMPP_COMMAND Command { get; }
        /// <summary>
        /// 获取“数据包”的字节流。
        /// </summary>
        byte[] GetBytes();
    }
}
