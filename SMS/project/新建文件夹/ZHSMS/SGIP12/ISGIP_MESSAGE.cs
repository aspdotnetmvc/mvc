using System;
using System.Collections.Generic;
using System.Text;

namespace SGIP
{
    public interface ISGIP_MESSAGE
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
        SGIP_COMMAND Command { get; }
        /// <summary>
        /// 获取“数据包”的字节流。
        /// </summary>
        byte[] GetBytes();
    }
}
