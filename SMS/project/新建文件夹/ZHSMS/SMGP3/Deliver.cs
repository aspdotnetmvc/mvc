using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SMGP
{
    public class Deliver
    {
        private ulong _MsgID;
        /// <summary>
        /// 短消息流水号
        /// </summary>
        public ulong MsgID
        {
            get { return _MsgID; }
        }

        private uint _IsReport;
        /// <summary>
        /// 是否为状态报告
        /// 
        /// 0＝不是状态报告；
        /// 1＝是状态报告；
        /// </summary>
        public uint IsReport
        {
            get { return _IsReport; }
        }

        private uint _MsgFormat;
        /// <summary>
        /// 短消息格式
        /// 
        /// 0＝ASCII 编码；
        /// 3＝短消息写卡操作
        /// 4＝二进制短消息；
        /// 8＝UCS2 编码；
        /// 15＝GB18030 编码；
        /// 246（F6）＝(U)SIM 相关消息；
        /// </summary>
        public uint MsgFormat
        {
            get { return _MsgFormat; }
        }


        private string _RecvTime;
        /// <summary>
        /// 短消息接收时间
        /// 
        /// 格式为YYYYMMDDHHMMSS（年年年年月月日日时时分分秒秒
        /// </summary>
        public string RecvTime
        {
            get { return _RecvTime; }
        }


        private string _SrcTermID;
        /// <summary>
        /// 短消息发送号码
        /// </summary>
        public string SrcTermID
        {
            get { return _SrcTermID; }
        }

        private string _DestTermID;
        /// <summary>
        /// 短消息接收号码
        /// 
        /// 格式为“118＋SP 服务代码＋其它（可选）
        /// </summary>
        public string DestTermID
        {
            get { return _DestTermID; }
        }


        private uint _MsgLength;
        /// <summary>
        /// 短消息长度
        /// </summary>
        public uint MsgLength
        {
            get { return _MsgLength; }
        }


        private string _MsgContent;
        /// <summary>
        /// 短消息内容
        /// </summary>
        public string MsgContent
        {
            get { return _MsgContent; }
        }

        private byte[] _Report;
        /// <summary>
        /// 状态报告
        /// </summary>
        public byte[] Report
        {
            get { return _Report; }

        }

        private string _Reserve;
        /// <summary>
        /// 保留
        /// </summary>
        public string Reserve
        {
            get { return _Reserve; }
        }

        private string _LinkID;
        public string LinkID
        {
            get { return _LinkID; }
        }

        private uint _TP_udhi;
        public uint TP_udhi
        {
            get { return _TP_udhi; }
        }

        //private string _ReportMsgID;
        //public string ReportMsgID
        //{
        //    get { return _ReportMsgID; }
        //}
        private Tlv[] _OtherTlv;
        public Tlv[] OtherTlv
        {
            get { return _OtherTlv; }
        }

        private MessageHeader _Header;

        public MessageHeader Header
        {
            get
            {
                return this._Header;
            }
        }

        public const int FixedBodyLength = 10 // MsgId Octet String 信息标识。 
                 + 1    // IsReport Unsigned Integer 是否为状态报告。 
                 + 1    // MsgFormat Unsigned Integer 短消息格式。 
                 + 14   // RecvTime Octet String 短消息接收时间 
                 + 21   // SrcTermID Unsigned String 短消息发送号码。 
                 + 21   // DestTermID Unsigned String 短消息接收号码
                 + 1    // MsgLength Unsigned Integer 短消息长度。 
            //MsgLength  // MsgContent Octet String 短消息内容。 
                 + 8;   // Reserve Octet String 保留。 
        // tlv
        private int _BodyLength;
        public int BodyLength
        {
            get
            {
                return this._BodyLength;
            }
        }



        public Deliver(byte[] bytes)
        {
            int i = 0;

            byte[] buffer = new byte[MessageHeader.Length];
            Buffer.BlockCopy(bytes, 0, buffer, 0, MessageHeader.Length);
            this._Header = new MessageHeader(buffer);

            string s = null;
            //MsgId 10 
            //i += MessageHeader.Length;
            //buffer = new byte[10];
            //Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            //s = Encoding.ASCII.GetString(buffer).Trim();
            //s = s.Substring(0, s.IndexOf('\0'));
            //this._MsgID = s;

            i += MessageHeader.Length;
            buffer = new byte[10];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._MsgID = BitConverter.ToUInt32(buffer, 0);

            //IsReport 1 
            i += 10;
            this._IsReport = (uint)bytes[i++];
            this._MsgFormat = (uint)bytes[i++];

            //RecvTime 14 
            buffer = new byte[14];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._RecvTime = s;

            //SrcTermID 21 
            i += 14;
            buffer = new byte[21];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._SrcTermID = s;

            // DestTermID 21 
            i += 21;
            buffer = new byte[21];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._DestTermID = s;

            // MsgLength 1
            i += 21;
            this._MsgLength = (uint)bytes[i++];


            // MsgContent 
            buffer = new byte[this._MsgLength];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);

            if (this._IsReport == 1)
            {   
                // 状态报告 保存至Report
                this._MsgContent = "";
                this._Report = new byte[_MsgLength];
                Buffer.BlockCopy(buffer, 0, _Report, 0, _Report.Length);
            }
            else
            {
                // 短信 保存至Msg_Content
                switch (this._MsgFormat)
                {
                    case 8:
                        this._MsgContent = Encoding.BigEndianUnicode.GetString(buffer).Trim();
                        break;
                    case 15: // gb2312 
                        this._MsgContent = Encoding.GetEncoding("gb2312").GetString(buffer).Trim();
                        break;
                    case 0: // ascii 
                    case 3: // 短信写卡操作 
                    case 4: // 二进制信息 
                    default:
                        this._MsgContent = Encoding.ASCII.GetString(buffer).ToString();
                        break;
                }
            }
            // Reserve 8
            i += (int)this._MsgLength;
            buffer = new byte[8];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._Reserve = s;

            // tlv  --剩余全部
            i += 8;
            //  buffer = new byte[bytes.Length - i];
            //  Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            ArrayList tmptlv = new ArrayList();
            for (; i < bytes.Length; )
            {
                // Tag 2
                buffer = new byte[2];
                Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
                Array.Reverse(buffer);
                short tlv_Tag = BitConverter.ToInt16(buffer, 0);

                // Length 2
                i += 2;
                buffer = new byte[2];
                Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
                Array.Reverse(buffer);
                short tlv_Length = BitConverter.ToInt16(buffer, 0);
                i += 2;

                String tlv_Value = "";
                if (tlv_Tag == TlvId.Mserviceid || tlv_Tag == TlvId.SrcTermPseudo
                        || tlv_Tag == TlvId.DestTermPseudo
                        || tlv_Tag == TlvId.ChargeTermPseudo
                        || tlv_Tag == TlvId.LinkID)
                {

                    buffer = new byte[tlv_Length];
                    Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
                    s = Encoding.ASCII.GetString(buffer).Trim();
                    if (s.IndexOf('\0') > 0)
                        s = s.Substring(0, s.IndexOf('\0'));
                    tlv_Value = s;
                    i += tlv_Length;
                }
                else
                {
                    tlv_Value = ((uint)bytes[i++]).ToString();
                }

                if (tlv_Tag == (short)TlvId.TP_udhi)
                {
                    this._TP_udhi = uint.Parse(tlv_Value);
                }
                else if (tlv_Tag == (short)TlvId.LinkID)
                {
                    this._LinkID = tlv_Value;
                }
                else
                {
                    tmptlv.Add(new Tlv(tlv_Tag, tlv_Value));
                }

                if (tmptlv.Count > 0)
                {
                    this._OtherTlv = new Tlv[tmptlv.Count];
                    int j = 0;
                    foreach (Object tmp in tmptlv)
                    {
                        this.OtherTlv[j++] = (Tlv)tmp;
                    }
                }
                break;
            }
        }

        public override string ToString()
        {
            return "[\r\n"
             + this._Header.ToString() + "\r\n"
             + string.Format
              (
               "\tMessageBody:"
               + "\r\n\t\tMsgID: {0}"
               + "\r\n\t\tIsReport: {1}"
               + "\r\n\t\tMsgFormat: {2}"
               + "\r\n\t\tRecvTime: {3}"
               + "\r\n\t\tSrcTermID: {4}"
               + "\r\n\t\tDestTermID: {5}"
               + "\r\n\t\tMsgLength: {6}"
               + "\r\n\t\tMsgContent: {7}"
               + "\r\n\t\tReserve: {8}"
               + "\r\n\t\tLinkID: {9}"
               + "\r\n\t\tTP_udhi: {10}"
               + "\r\n\t\tOtherTlv: {11}"
               , this._MsgID
               , this._IsReport
               , this._MsgFormat
               , this._RecvTime
               , this._SrcTermID
               , this._DestTermID
               , this._MsgLength
               , this._MsgContent
               , this._Reserve
               , this._LinkID
               , this._TP_udhi
               , this._OtherTlv.ToString()
              )
             + "\r\n]";
        }
    }
}
