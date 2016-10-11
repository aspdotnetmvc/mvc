using System;
using System.Runtime.InteropServices;

namespace CMPP
{
    /// <summary>
    /// CMPP_ACTIVE_TEST_RESP 定义（SP->ISMG 或 ISMG->SP）。
    /// </summary>
    /// <remarks>
    /// 链路检测（CMPP_ACTIVE_TEST）操作：本操作仅适用于通信双方采用长连接通信方式时用于保持连接。
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct CMPP_ACTIVE_TEST_RESP : ICMPP_MESSAGE
    {

        #region 字段

        #region 消息头
        /// <summary>
        /// 消息头（所有消息公共包头）。
        /// </summary>
        public CMPP_HEAD Head;
        #endregion

        #region 消息体
        public byte Reserved;
        #endregion

        #endregion

        #region 公有方法
        public override string ToString()
        {
            return "";
        }
        #endregion

        #region ICMPP_MESSAGE 成员
        /// <summary>
        /// 获取 CMPP_ACTIVE_TEST_RESP 的字节流。
        /// </summary>
        public byte[] GetBytes()
        {
            int iPos = 0;
            Head.TotalLength = (UInt32)Marshal.SizeOf(this);
            Byte[] buffer = new Byte[Head.TotalLength];

            Byte[] HeadBuffer = this.Head.GetBytes();
            Array.Copy(HeadBuffer, 0, buffer, 0, HeadBuffer.Length);
            iPos = HeadBuffer.Length;

            buffer[iPos] = Reserved;

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
