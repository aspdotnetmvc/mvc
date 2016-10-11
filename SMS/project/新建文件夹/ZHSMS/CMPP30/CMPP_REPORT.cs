using System;
using System.Runtime.InteropServices;

namespace CMPP
{
    /// <summary>
    /// ISMG 向 SP 送交的状态报告（只有在 CMPP_SUBMIT 中的 RegisteredDelivery 被设置为1时，ISMG才会向SP发送状态报告）。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CMPP_REPORT
    {

        #region 字段
        /// <summary>
        /// 信息标识；SP提交短信（CMPP_SUBMIT）操作时，与SP相连的ISMG产生的Msg_Id。
        /// </summary>
        public ulong MsgID;
        /// <summary>
        /// 发送短信的应答结果，含义详见表一（SP根据该字段确定CMPP_SUBMIT消息的处理状态）。
        /// </summary>
        public string Stat;
        /// <summary>
        /// YYMMDDHHMM（YY为年的后两位00-99，MM：01-12，DD：01-31，HH：00-23，MM：00-59）。
        /// </summary>
        public string SubmitTime;
        /// <summary>
        /// YYMMDDHHMM。
        /// </summary>
        public string DoneTime;
        /// <summary>
        /// 目的终端MSISDN号码（SP发送CMPP_SUBMIT消息的目标终端）。
        /// </summary>
        public string DestTerminalID;
        /// <summary>
        /// 取自SMSC发送状态报告的消息体中的消息标识。
        /// </summary>
        public uint SMSCSequence;
        #endregion

        #region 公有方法
        /// <summary>
        /// 初始化 CMPP_REPORT。
        /// </summary>
        public bool Init(byte[] buffer)
        {
            int iPos = 0;
            bool bOK = true;
            try
            {
                MsgID = Convert.ToUInt64(buffer, 0);
                iPos += 8;

                Stat = Convert.ToString(buffer, iPos, 7, CEncoding.ASCII);
                iPos += 7;

                SubmitTime = Convert.ToString(buffer, iPos, 10, CEncoding.ASCII);
                iPos += 10;

                DoneTime = Convert.ToString(buffer, iPos, 10, CEncoding.ASCII);
                iPos += 10;

                DestTerminalID = Convert.ToString(buffer, iPos, 21, CEncoding.ASCII);
                iPos += 21;

                SMSCSequence = Convert.ToUInt32(buffer, iPos);
            }
            catch
            {
                bOK = false;
            }
            return bOK;
        }
        #endregion

    }
}
