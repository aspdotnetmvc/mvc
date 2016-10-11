using System;
using System.Collections.Generic;
using System.Text;

namespace SMGP
{
    /// <summary>
    /// CMPP 数据包。
    /// </summary>
    public interface ISMGP_MESSAGE
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
        SMGP3_COMMAND Command { get; }
        /// <summary>
        /// 获取“数据包”的字节流。
        /// </summary>
        byte[] GetBytes();
    }
}
