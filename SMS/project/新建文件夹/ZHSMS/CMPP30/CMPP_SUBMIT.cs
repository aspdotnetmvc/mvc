using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CMPP
{
    /// <summary>
    /// SP 向 ISMG 提交短信（CMPP_SUBMIT）操作（SP->ISMG）。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CMPP_SUBMIT : ICMPP_MESSAGE
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
        /// 信息标识（由ISMG生成，发送时不填，供ISMG传输时使用，可以从返回的RESP中获得本次发送的MSG_ID，通过 HEAD 中的 SequenceID 字段对应）。
        /// </summary>
        public ulong MsgID;
        /// <summary>
        /// 相同 Msg_Id 的信息总条数，从 1 开始。
        /// </summary>
        public byte PkTotal;
        /// <summary>
        /// 相同 Msg_Id 的信息序号，从 1 开始。
        /// </summary>
        public byte PkNumber;
        /// <summary>
        /// 是否要求返回状态确认报告（0：不需要；1：需要）。
        /// </summary>
        public byte RegisteredDelivery;
        /// <summary>
        /// 信息级别。
        /// </summary>
        public byte MsgLevel;
        /// <summary>
        /// 业务标识，是数字、字母和符号的组合（长度为 10，SP的业务类型，数字、字母和符号的组合，由SP自定，如图片传情可定为TPCQ，股票查询可定义为11）。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string ServiceID;
        /// <summary>
        /// 计费用户类型字段（0：对目的终端MSISDN计费；1：对源终端MSISDN计费；2：对SP计费；3：表示本字段无效，对谁计费参见Fee_terminal_Id字段）。
        /// </summary>
        public byte FeeUserType;
        /// <summary>
        /// 被计费用户的号码，当Fee_UserType为3时该值有效，当Fee_UserType为0、1、2时该值无意义。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FeeTerminalID;
        /// <summary>
        /// 被计费用户的号码类型，0：真实号码；1：伪码。
        /// </summary>
        public byte FeeTerminalType;
        /// <summary>
        /// GSM协议类型（详细解释请参考GSM03.40中的9.2.3.9）。
        /// </summary>
        public byte TPPID;
        /// <summary>
        /// GSM协议类型（详细是解释请参考GSM03.40中的9.2.3.23,仅使用1位，右对齐）。
        /// </summary>
        public byte TPUdhi;
        /// <summary>
        /// 信息格式（0：ASCII串；3：短信写卡操作；4：二进制信息；8：UCS2编码；15：含GB汉字）。
        /// </summary>
        public byte MsgFmt;
        /// <summary>
        /// 信息内容来源（SP_Id：SP的企业代码：网络中SP地址和身份的标识、地址翻译、计费、结算等均以企业代码为依据。企业代码以数字表示，共6位，从“9XY000”至“9XY999”，其中“XY”为各移动公司代码）。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string MsgSrc;
        /// <summary>
        /// 资费类别（01：对“计费用户号码”免费；02：对“计费用户号码”按条计信息费；03：对“计费用户号码”按包月收取信息费）。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string FeeType;
        /// <summary>
        /// 资费代码（以分为单位）。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string FeeCode;
        /// <summary>
        /// 存活有效期，格式遵循SMPP3.3协议。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string ValidTime;
        /// <summary>
        /// 定时发送时间，格式遵循SMPP3.3协议。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string AtTime;
        /// <summary>
        /// 企业代码。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string SrcID;
        /// <summary>
        /// 接收信息的用户数量（小于100个用户）。
        /// </summary>
        public byte DestUsrTl;
        /// <summary>
        /// 接收短信的MSISDN号码。
        /// </summary>
        public string[] DestTerminalID;
        /// <summary>
        /// 接收短信的用户的号码类型(0：真实号码；1：伪码）。
        /// </summary>
        public byte DestTerminalType;
        /// <summary>
        /// 信息长度（Msg_Fmt值为0时：&lt; 160个字节；其它 &gt;= 140个字节)，取值大于或等于0。
        /// </summary>
        public byte MsgLength;
        /// <summary>
        /// 信息内容。
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
        /// 
        /// </summary>
        public override string ToString()
        {
            return MsgContent;
        }
        #endregion

        #region ICMPP_MESSAGE 成员
        /// <summary>
        /// 获取 CMPP_SUBMIT 的字节流。
        /// </summary>
        public byte[] GetBytes()
        {
            int iPos = 0;
            MsgLength = Convert.Length(MsgContent.ToString(), MsgFmt);
            Head.TotalLength = (UInt32)(163 + 32 * DestUsrTl + MsgLength);
            Byte[] buffer = new Byte[Head.TotalLength];
            Byte[] temp = null;

            Byte[] HeadBuffer = this.Head.GetBytes();
            Array.Copy(HeadBuffer, 0, buffer, 0, HeadBuffer.Length);
            iPos = HeadBuffer.Length;

            temp = Convert.ToBytes(MsgID);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + temp.Length;

            buffer[iPos] = PkTotal;
            iPos++;

            buffer[iPos] = PkNumber;
            iPos++;

            buffer[iPos] = RegisteredDelivery;
            iPos++;

            buffer[iPos] = MsgLevel;
            iPos++;

            temp = Convert.ToBytes(ServiceID, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 10;

            buffer[iPos] = FeeUserType;
            iPos++;

            temp = Convert.ToBytes(FeeTerminalID, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 32;

            buffer[iPos] = FeeTerminalType;
            iPos++;

            buffer[iPos] = TPPID;
            iPos++;

            buffer[iPos] = TPUdhi;
            iPos++;

            buffer[iPos] = MsgFmt;
            iPos++;

            temp = Convert.ToBytes(MsgSrc, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 6;

            temp = Convert.ToBytes(FeeType, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 2;

            temp = Convert.ToBytes(FeeCode, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 6;

            temp = Convert.ToBytes(ValidTime, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 17;

            temp = Convert.ToBytes(AtTime, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 17;

            temp = Convert.ToBytes(SrcID, CMPP30.CODING_ASCII);
            Array.Copy(temp, 0, buffer, iPos, temp.Length);
            iPos = iPos + 21;

            buffer[iPos] = DestUsrTl;
            iPos++;

            for (int i = 0; i < DestUsrTl; i++)
            {
                temp = Convert.ToBytes(DestTerminalID[i], CMPP30.CODING_ASCII);
                Array.Copy(temp, 0, buffer, iPos, temp.Length);
                iPos = iPos + 32;
            }

            buffer[iPos] = DestTerminalType;
            iPos++;

            buffer[iPos] = MsgLength;
            iPos++;

            if (PkTotal == 1)
            {
                temp = Convert.ToBytes(MsgContent, MsgFmt);
                Array.Copy(temp, 0, buffer, iPos, temp.Length);
                iPos = iPos + temp.Length;
            }
            else
            {
                Buffer.BlockCopy(this._Msg_Content_Bytes, 0, buffer, iPos, this._Msg_Content_Bytes.Length);
                iPos = iPos + _Msg_Content_Bytes.Length;
            }

            temp = Convert.ToBytes(LinkID, CMPP30.CODING_ASCII);
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
        byte[] _Msg_Content_Bytes;

        public void SetHeader()
        {
            _Msg_Content_Bytes = Encoding.BigEndianUnicode.GetBytes(this.MsgContent);
            this.TPUdhi = 1;
            byte[] tp_udhiHead = new byte[6];
            tp_udhiHead[0] = 0x05;
            tp_udhiHead[1] = 0x00;
            tp_udhiHead[2] = 0x03;
            tp_udhiHead[3] = 0x0A;
            tp_udhiHead[4] = (byte)this.PkTotal;
            tp_udhiHead[5] = (byte)this.PkNumber;
            byte[] Msg_Content_Bytes_Temp = new byte[_Msg_Content_Bytes.Length + 6];
            int index = 0;
            tp_udhiHead.CopyTo(Msg_Content_Bytes_Temp, index);
            index += tp_udhiHead.Length;
            _Msg_Content_Bytes.CopyTo(Msg_Content_Bytes_Temp, index);
            _Msg_Content_Bytes = Msg_Content_Bytes_Temp;
        }

        public CMPP_COMMAND Command
        {
            get { return (CMPP_COMMAND)Head.CommandID; }
        }
    }
}
