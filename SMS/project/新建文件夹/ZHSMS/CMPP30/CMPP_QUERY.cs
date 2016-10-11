using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CMPP
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct CMPP_QUERY : ICMPP_MESSAGE
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
        /// 时间（YYYYMMDD 精确至日）
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Time;
        /// <summary>
        /// 查询类别（0:总数查询 1:按业务类型查询）
        /// </summary>
        public uint Query_Type;
        /// <summary>
        /// 查询码（当Query_Type为0时，此项无效;当Query_Type为1时，此项填写业务类型Service_Id.）
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Query_Code;
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Reserve;

        #endregion

        #endregion

        /// <summary>
        /// 获取 CMPP_QUERY 的字节流。
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

            temp = Convert.ToBytes(Time, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 8;


            temp = BitConverter.GetBytes(Query_Type);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + temp.Length;

            temp = Convert.ToBytes(Query_Code, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 10;

            temp = Convert.ToBytes(Reserve, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 8;

            return buffer;
        }

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
