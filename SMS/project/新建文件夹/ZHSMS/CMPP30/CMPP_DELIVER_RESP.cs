using System;
using System.Runtime.InteropServices;

namespace CMPP
{
    /// <summary>
    /// CMPP_DELIVER_RESP 消息定义（SP->ISMG）。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CMPP_DELIVER_RESP : ICMPP_MESSAGE
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
        /// 信息标识（CMPP_DELIVER中的 Msg_Id 字段）。
        /// </summary>
        public ulong MsgID;
        /// <summary>
        /// 结果（0：正确；1：消息结构错；2：命令字错；3：消息序号重复；4：消息长度错；5：资费代码错；6：超过最大信息长；7：业务代码错；8: 流量控制错；9~ ：其他错误）。
        /// </summary>
        public uint Result;
        #endregion

        #endregion

        #region 公有方法
        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            switch (Result)
            {
                case 0:
                    return "正确";
                case 1:
                    return "消息结构错";
                case 2:
                    return "命令字错";
                case 3:
                    return "消息序号重复";
                case 4:
                    return "消息长度错";
                case 5:
                    return "资费代码错";
                case 6:
                    return "超过最大信息长";
                case 7:
                    return "业务代码错";
                case 8:
                    return "流量控制错";
                default:
                    return "其他错误";
            }
        }
        #endregion

        #region ICMPP_MESSAGE 成员
        /// <summary>
        /// 获取 CMPP_DELIVER_RESP 的字节流。
        /// </summary>
        public byte[] GetBytes()
        {
            int iPos = 0;
            Head.TotalLength = (UInt32)Marshal.SizeOf(this);
            Byte[] buffer = new Byte[Head.TotalLength];
            Byte[] temp = null;

            Byte[] HeadBuffer = this.Head.GetBytes();
            Array.Copy(HeadBuffer, 0, buffer, 0, HeadBuffer.Length);
            iPos = HeadBuffer.Length;

            temp = BitConverter.GetBytes(MsgID);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + temp.Length;

            temp = Convert.ToBytes(Result);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + temp.Length;

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
