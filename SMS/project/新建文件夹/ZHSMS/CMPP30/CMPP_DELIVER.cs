using System;
using System.Runtime.InteropServices;

namespace CMPP
{
    /// <summary>
    /// ISMG 向 SP 送交短信（CMPP_DELIVER）操作（ISMG->SP）。
    /// </summary>
    /// <remarks>
    /// CMPP_DELIVER 操作的目的是 ISMG 把从短信中心或其它 ISMG 转发来的短信送交 SP，SP 以 CMPP_DELIVER_RESP 消息回应。
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CMPP_DELIVER
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
        /// 信息标识；生成算法如下：采用64位（8字节）的整数：（1）时间（格式为MMDDHHMMSS，即月日时分秒）：bit64~bit39，其中bit64~bit61：月份的二进制表示；bit60~bit56：日的二进制表示；bit55~bit51：小时的二进制表示；bit50~bit45：分的二进制表示；bit44~bit39：秒的二进制表示；（2）短信网关代码：bit38~bit17，把短信网关的代码转换为整数填写到该字段中；（3）序列号：bit16~bit1，顺序增加，步长为1，循环使用。各部分如不能填满，左补零，右对齐。
        /// </summary>
        public ulong MsgID;
        /// <summary>
        /// 目的号码（SP的服务代码，一般4--6位，或者是前缀为服务代码的长号码；该号码是手机用户短消息的被叫号码；SP的服务代码：服务代码是在使用短信方式的上行类业务中，提供给用户使用的服务提供商代码。服务代码以数字表示，全国业务服务代码长度为4位，即“1000”－“9999”；本地业务服务代码长度统一为5位，即“01000”－“09999”；信产部对新的SP的服务代码分配提出了新的要求，要求以“1061”－“1069”作为前缀，目前中国移动进行了如下分配：1062：用于省内SP服务代码1066：用于全国SP服务代码其它号段保留）。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string DestID;
        /// <summary>
        /// 业务标识，是数字、字母和符号的组合（SP的业务类型，数字、字母和符号的组合，由SP自定，如图片传情可定为TPCQ，股票查询可定义为11）。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string ServiceID;
        /// <summary>
        /// GSM协议类型（详细解释请参考GSM03.40中的9.2.3.9）。
        /// </summary>
        public byte TPPID;
        /// <summary>
        /// GSM协议类型（详细解释请参考GSM03.40中的9.2.3.23，仅使用1位，右对齐）。
        /// </summary>
        public byte TPUdhi;
        /// <summary>
        /// 信息格式（0：ASCII串；3：短信写卡操作；4：二进制信息；8：UCS2编码；15：含GB汉字）。
        /// </summary>
        public byte MsgFmt;
        /// <summary>
        /// 源终端MSISDN号码（状态报告时填为CMPP_SUBMIT消息的目的终端号码）。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string SrcTerminalID;
        /// <summary>
        /// 源终端号码类型（0：真实号码；1：伪码）。
        /// </summary>
        public byte SrcTerminalType;
        /// <summary>
        /// 是否为状态报告（0：非状态报告；1：状态报告）。
        /// </summary>
        public byte RegisteredDelivery;
        /// <summary>
        /// 消息长度，取值大于或等于0。
        /// </summary>
        public byte MsgLength;
        /// <summary>
        /// 消息内容。
        /// </summary>
        public string MsgContent;
        /// <summary>
        /// 点播业务使用的LinkID，非点播类业务的MT流程不使用该字段。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string LinkID;
        #endregion

        #endregion

        #region 公有方法
        /// <summary>
        /// 初始化 CMPP_DELIVER。
        /// </summary>
        public bool Init(byte[] buffer)
        {
            int iPos = 0;
            bool bOK = true;
            try
            {
                MsgID = (UInt64)BitConverter.ToUInt64(buffer, 0);
                iPos = iPos + 8;

                DestID = Convert.ToString(buffer, iPos, 21, CEncoding.ASCII);
                iPos = iPos + 21;

                ServiceID = Convert.ToString(buffer, iPos, 10, CEncoding.ASCII);
                iPos = iPos + 10;

                TPPID = buffer[iPos];
                iPos++;

                TPUdhi = buffer[iPos];
                iPos++;

                MsgFmt = buffer[iPos];
                iPos++;

                SrcTerminalID = Convert.ToString(buffer, iPos, 32, CEncoding.ASCII);
                iPos = iPos + 32;

                SrcTerminalType = buffer[iPos];
                iPos++;

                RegisteredDelivery = buffer[iPos];
                iPos++;

                MsgLength = buffer[iPos];
                iPos++;

                if (RegisteredDelivery == 0)//是短消息
                {
                    MsgContent = Convert.ToString(buffer, iPos, MsgLength, (CEncoding)MsgFmt);

                }
                else//是状态报告,先转为BASE64 String 存储
                    MsgContent = System.Convert.ToBase64String(buffer, iPos, MsgLength);

                iPos = iPos + MsgLength;
                LinkID = Convert.ToString(buffer, iPos, 20, CEncoding.ASCII);
            }
            catch
            {
                bOK = false;
            }
            return bOK;
        }
        /// <summary>
        /// 获取 CMPP_SUBMIT 的状态报告（只有在 CMPP_SUBMIT 中的 RegisteredDelivery 被设置为1时，ISMG才会向SP发送状态报告）。
        /// </summary>
        /// <returns></returns>
        public CMPP_REPORT GetReport()
        {
            CMPP_REPORT Report = new CMPP_REPORT();
            if (RegisteredDelivery == 1)//是状态报告
            {
                Byte[] bytes = System.Convert.FromBase64String(MsgContent);
                if ((bytes != null) && (bytes.Length > 0))
                    Report.Init(bytes);
            }
            return Report;
        }
        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            return MsgContent;
        }
        #endregion

    }
}
