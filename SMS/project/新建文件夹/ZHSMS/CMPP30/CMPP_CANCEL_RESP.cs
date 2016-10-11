using System;
using System.Runtime.InteropServices;

namespace CMPP
{
    /// <summary>
    /// CMPP_CANCEL_RESP 消息定义（ISMG->SP）
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CMPP_CANCEL_RESP
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
        /// 成功标识（0：成功；1：失败）。
        /// </summary>
        public uint SuccessID;
        #endregion

        #endregion
        
    }
}
