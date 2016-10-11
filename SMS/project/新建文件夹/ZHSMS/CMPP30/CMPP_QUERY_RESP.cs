using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CMPP
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct CMPP_QUERY_RESP
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
        /// 从SP接收信息总数
        /// </summary>
        public uint MT_TLMsg;
        /// <summary>
        /// 从SP接收用户数
        /// </summary>
        public uint MT_Tlusr;
        /// <summary>
        /// 成功转发数
        /// </summary>
        public uint MT_Scs;
        /// <summary>
        /// 待转发数
        /// </summary>
        public uint MT_WT;
        /// <summary>
        /// 转发失败数量
        /// </summary>
        public uint MT_FL;
        /// <summary>
        /// 向SP成功送达数
        /// </summary>
        public uint MO_Scs;
        /// <summary>
        /// 向SP待送达数量
        /// </summary>
        public uint MO_WT;
        /// <summary>
        /// 向SP送达失败数量
        /// </summary>
        public uint MO_FL;
        #endregion

        #endregion


        public bool Init(byte[] buffer)
        {
            int iPos = 0;
            bool bOK = true;
            try
            {
                Time = Convert.ToString(buffer, iPos, 8, CEncoding.ASCII);
                iPos += 8;
                Query_Type = (UInt32)buffer[iPos];
                iPos += 1;
                Query_Code = Convert.ToString(buffer, iPos, 10, CEncoding.ASCII);
                iPos += 10;
                MT_TLMsg = (UInt32)BitConverter.ToUInt64(buffer, iPos);
                iPos += 4;
                MT_Tlusr = (UInt32)BitConverter.ToUInt64(buffer, iPos);
                iPos += 4;
                MT_Scs = (UInt32)BitConverter.ToUInt64(buffer, iPos);
                iPos += 4;
                MT_WT = (UInt32)BitConverter.ToUInt64(buffer, iPos);
                iPos += 4;
                MT_FL = (UInt32)BitConverter.ToUInt64(buffer, iPos);
                iPos += 4;
                MO_Scs = (UInt32)BitConverter.ToUInt64(buffer, iPos);
                iPos += 4;
                MO_WT = (UInt32)BitConverter.ToUInt64(buffer, iPos);
                iPos += 4;
                MO_FL = (UInt32)BitConverter.ToUInt64(buffer, iPos);
                iPos += 4;
            }
            catch
            {
                bOK = false;
            }
            return bOK;
        }
    }
}
