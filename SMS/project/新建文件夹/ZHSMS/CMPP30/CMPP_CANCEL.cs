using System;
using System.Runtime.InteropServices;

namespace CMPP
{
    /// <summary>
    /// SP 向 ISMG 发起删除短信（CMPP_CANCEL）操作（SP->ISMG）。
    /// </summary>
    /// <remarks>
    /// CMPP_CANCEL 操作的目的是 SP 通过此操作可以将已经提交给 ISMG 的短信删除，ISMG 将以 CMPP_CANCEL_RESP 回应删除操作的结果。
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CMPP_CANCEL : ICMPP_MESSAGE
    {

        #region 字段

        #region 消息头
        /// <summary>
        /// 消息头（所有消息公共包头）。
        /// </summary>
        public CMPP_HEAD Head;
        #endregion

        #region 消息体
        /// <summary>
        /// 信息标识（SP 想要删除的信息标识）。
        /// </summary>
        public ulong MsgID;
        #endregion

        #endregion

        #region 公有方法
        /// <summary>
        /// 获取 CMPP_CONNECT 的字节流。
        /// </summary>
        public byte[] GetBytes()
        {
            Byte[] buffer = new Byte[Marshal.SizeOf(this)];
            Byte[] HeadBuffer = this.Head.GetBytes();
            int iPos = HeadBuffer.Length;
            Array.Copy(HeadBuffer, 0, buffer, 0, iPos);
            Byte[] temp = BitConverter.GetBytes(MsgID);
            buffer[iPos + 7] = temp[0];
            buffer[iPos + 6] = temp[1];
            buffer[iPos + 5] = temp[2];
            buffer[iPos + 4] = temp[3];
            buffer[iPos + 3] = temp[4];
            buffer[iPos + 2] = temp[5];
            buffer[iPos + 1] = temp[6];
            buffer[iPos + 0] = temp[7];

            return buffer;
        }
        #endregion

        public uint SequenceID
        {
            get
            {
                return Head.SequenceID;
            }
            set
            {
                Head.SequenceID = value;
            }
        }

        public CMPP_COMMAND Command
        {
            get { return (CMPP_COMMAND)Head.CommandID; }
        }
    }
}
