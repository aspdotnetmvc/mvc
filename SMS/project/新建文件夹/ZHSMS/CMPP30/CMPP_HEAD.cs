using System;
using System.Runtime.InteropServices;

namespace CMPP
{
    /// <summary>
    /// 消息头格式（Message Header）。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CMPP_HEAD
    {
        #region 字段
        /// <summary>
        /// 消息总长度(含消息头及消息体)。
        /// </summary>
        public uint TotalLength;
        /// <summary>
        /// 命令或响应类型。
        /// </summary>
        public uint CommandID;
        /// <summary>
        /// 消息流水号,顺序累加,步长为1,循环使用（每对请求和应答消息的流水号必须相同）。
        /// </summary>
        public uint SequenceID;
        #endregion

        #region 公有方法
        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            return "";
        }
        #endregion

        #region ICMPP_MESSAGE 成员
        /// <summary>
        /// 获取 CMPP_HEAD 的字节流。
        /// </summary>
        public byte[] GetBytes()
        {
            Byte[] buffer = new Byte[Marshal.SizeOf(this)];//12;
            Byte[] temp = null;
            temp = BitConverter.GetBytes(TotalLength);
            buffer[3] = temp[0];
            buffer[2] = temp[1];
            buffer[1] = temp[2];
            buffer[0] = temp[3];
            temp = BitConverter.GetBytes(CommandID);
            buffer[7] = temp[0];
            buffer[6] = temp[1];
            buffer[5] = temp[2];
            buffer[4] = temp[3];
            temp = BitConverter.GetBytes(SequenceID);
            buffer[11] = temp[0];
            buffer[10] = temp[1];
            buffer[9] = temp[2];
            buffer[8] = temp[3];
            return buffer;
        }
        #endregion
    }
}
