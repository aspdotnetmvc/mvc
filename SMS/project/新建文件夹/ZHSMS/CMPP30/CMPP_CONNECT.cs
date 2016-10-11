using System;
using System.Runtime.InteropServices;

namespace CMPP
{
    /// <summary>
    /// SP 请求连接到 ISMG（CMPP_CONNECT）操作（SP->ISMG）。
    /// </summary>
    /// <remarks>
    /// CMPP_CONNECT 操作的目的是 SP 向 ISMG 注册作为一个合法 SP 身份，若注册成功后即建立了“应用层”的连接，此后 SP 可以通过此 ISMG 接收和发送短信。ISMG 以 CMPP_CONNECT_RESP 消息响应SP的请求。
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct CMPP_CONNECT : ICMPP_MESSAGE
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
        /// 源地址，此处为SP_Id，即 SP 的企业代码（长度为 6 字节）。
        /// </summary>
        /// <remarks>
        /// SP_Id（SP 的企业代码）：网络中 SP 地址和身份的标识、地址翻译、计费、结算等均以企业代码为依据。企业代码以数字表示，共 6 位，从“9XY000”至“9XY999”，其中“XY”为各移动公司代码。
        /// </remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string SourceAddress;
        /// <summary>
        /// 用于鉴别源地址。其值通过单向 MD5 hash 计算得出，表示如下：AuthenticatorSource =MD5(Source_Addr + 9 字节的 0 + shared secret + timestamp)，Shared secret 由中国移动与源地址实体事先商定，timestamp 格式为：MMDDHHMMSS，即月日时分秒，10位。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] AuthenticatorSource;
        /// <summary>
        /// 双方协商的版本号（高位 4 bit 表示主版本号,低位 4 bit 表示次版本号），对于 3.0 的版本，高 4bit 为 3，低 4 位为 0。
        /// </summary>
        public byte Version;
        /// <summary>
        /// 时间戳的明文，由客户端产生，格式为 MMDDHHMMSS，即：月日时分秒，10 位数字的整型，右对齐。
        /// </summary>
        public uint TimeStamp;
        #endregion

        #endregion

        #region 公有方法
        /// <summary>
        /// 获取 CMPP_CONNECT 的字节流。
        /// </summary>
        public byte[] GetBytes()
        {
            Byte[] temp = null;
            int iPos = 0;
            Head.TotalLength = 39;
            Byte[] buffer = new Byte[39];

            Byte[] HeadBuffer = this.Head.GetBytes();
            Array.Copy(HeadBuffer, 0, buffer, 0, HeadBuffer.Length);
            iPos = iPos + HeadBuffer.Length;

            temp = Convert.ToBytes(SourceAddress, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 6;

            Array.Copy(AuthenticatorSource, 0, buffer, iPos, AuthenticatorSource.Length);
            iPos = iPos + AuthenticatorSource.Length;

            buffer[iPos] = Version;
            iPos++;

            //temp=BitConverter.GetBytes(TimeStamp);
            temp = Convert.ToBytes(TimeStamp);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + temp.Length;

            return buffer;
        }
        public override string ToString()
        {
            return "";
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
