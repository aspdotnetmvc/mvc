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
    public struct CMPP_ACTIVE_TEST : ICMPP_MESSAGE
    {
        public CMPP_ACTIVE_TEST(CMPP_HEAD head)
        {
            Head = head;
        }

        #region 字段

        #region 消息头
        /// <summary>
        /// 消息头（所有消息公共包头）。
        /// </summary>
        public CMPP_HEAD Head;
        #endregion

        #endregion

        #region 公有方法

        public byte[] GetBytes()
        {
            return Head.GetBytes();
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
