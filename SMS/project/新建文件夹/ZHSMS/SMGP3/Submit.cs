using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Submit:ISMGP_MESSAGE
    {
        private int _BodyLength;

        public const int FixedBodyLength = 1
                 + 1
                 + 1
                 + 10
                 + 2
                 + 6
                 + 6
                 + 1
                 + 17
                 + 17
                 + 21
                 + 21
                 + 1
            //+ 21*DestTermIDCount （_DestTermID字段）
                 + 1
            //+ MsgLength (Msg_Content字段)
                 + 8;
        // tlv

        private uint _MsgType;          // 1 Unsigned Integer 短消息类型。 
        private uint _NeedReport;       // 1 Unsigned Integer 是否要求返回状态报。 
        private uint _Priority;         // 1 Unsigned Integer 短消息发送优先级。 
        private string _ServiceID;      // 10 Octet String 业务代码
        private string _FeeType;          // 2 Octet String 收费类型。 
        private string _FeeCode;        // 6 Octet String 资费代码。 
        private string _FixedFee;         // 6 Octet String 包月费/封顶费 
        private uint _MsgFormat;        // 1 Unsigned Integer 短消息格式 
        private string _ValidTime;      // 17 Octet String 短消息有效时间。 
        private string _AtTime;         // 17 Octet String 短消息定时发送时间,格式遵循SMPP3.3协议。
        private string _SrcTermID;      // 21 Octet String 短信息发送方号码
        private string _ChargeTermID;   // 21 Octet String 计费用户号码
        private uint _DestTermIDCount;  // 1 Unsigned Integer 短消息接收号码总数。 
        private string[] _DestTermID;   // 21*DestTermIDCount Octet String 短消息接收号码。 
        private uint _MsgLength;        // 1 Unsigned Integer 短消息长度
        private string _MsgContent;     // MsgLength Octet String 短消息内容。 
        private string _Reserve;        // 8 Octet String 保留
        private string _MsgSrc;         //信息内容来源
        private string _WapURL = ""; //WAPPUSH
        /// <summary>
        /// 协议范围外WAPPUSH
        /// </summary>
        public string WapURL
        {
            get { return _WapURL; }
            set
            {
                _WapURL = value;
            }
        }

        /// <summary>
        /// 协议范围外，长短信总数。
        /// </summary>
        public uint Pk_total
        {
            get;
            set;
        }

        private uint _pk_number;

        /// <summary>
        /// 协议范围外，长短信序号。
        /// </summary>
        public uint Pk_number
        {
            get
            {
                return _pk_number;
            }
            set
            {
                _pk_number = value;

            }
        }

        // tlv
        private string _ProductID = "";
        private string _LinkID = "";
        private Tlv[] _OtherTlvArray = null;

        public MessageHeader Header
        {
            get
            {
                return _Header;
            }
            set
            {
                _Header = value;
            }
        }
        private MessageHeader _Header;

        private uint _Sequence_Id;

        public Submit(uint Sequence_Id)
        {
            this._Sequence_Id = Sequence_Id;
        }

        private byte[] _MsgContent_Bytes;

        public void SetHeader()
        {
            //if (string.IsNullOrEmpty(_MsgContent))
            //    return;

            if (string.IsNullOrEmpty(_WapURL))
            {
                //byte[] buf;
                switch (this._MsgFormat)
                {
                    case 8:
                        _MsgContent_Bytes = Encoding.BigEndianUnicode.GetBytes(this._MsgContent);
                        break;
                    case 15: //gb2312
                        _MsgContent_Bytes = Encoding.GetEncoding("gb2312").GetBytes(this._MsgContent);
                        break;
                    case 0: //ascii
                    case 3: //短信写卡操作
                    case 4: //二进制信息
                    default:
                        _MsgContent_Bytes = Encoding.ASCII.GetBytes(this._MsgContent);
                        break;
                }

                if (this.Pk_total > 1) //长短信
                {
                    AddTlv(TlvId.TP_udhi, "1");
                    AddTlv(TlvId.PkTotal, this.Pk_total.ToString());
                    AddTlv(TlvId.PkNumber, this.Pk_number.ToString());
                    AddTlv(TlvId.MsgSrc, this._MsgSrc);
                    byte[] tp_udhiHead = new byte[6];
                    tp_udhiHead[0] = 0x05;
                    tp_udhiHead[1] = 0x00;
                    tp_udhiHead[2] = 0x03;
                    tp_udhiHead[3] = 0x0A;
                    tp_udhiHead[4] = (byte)this.Pk_total;
                    tp_udhiHead[5] = (byte)this.Pk_number;
                    byte[] Msg_Content_Bytes_Temp = new byte[_MsgContent_Bytes.Length + 6];
                    int index = 0;
                    tp_udhiHead.CopyTo(Msg_Content_Bytes_Temp, index);
                    index += tp_udhiHead.Length;
                    _MsgContent_Bytes.CopyTo(Msg_Content_Bytes_Temp, index);
                    _MsgContent_Bytes = Msg_Content_Bytes_Temp;
                }
            }
            else
            {
                AddTlv(TlvId.TP_pid, "1");
                AddTlv(TlvId.TP_udhi, "1");
                AddTlv(TlvId.MsgSrc, this._MsgSrc);
                _MsgContent_Bytes = WapPush.GetInstance().toBytes(this._MsgContent, this._WapURL);
                this._MsgFormat = 0x04;
            }

            this._MsgLength = (uint)_MsgContent_Bytes.Length;
            this._BodyLength = (int)(FixedBodyLength + 21 * this._DestTermID.Length + this._MsgLength);
            if (_OtherTlvArray != null)
            {
                for (int i = 0; i < _OtherTlvArray.Length; i++)
                {
                    if (_OtherTlvArray[i].Length > 0)
                        _BodyLength += _OtherTlvArray[i].Length + 4;
                }
            }
            this._Header = new MessageHeader((uint)(MessageHeader.Length + this._BodyLength), SMGP3_COMMAND.Submit, this._Sequence_Id);
        }

        public byte[] GetBytes()
        {
            //Msg_Length Msg_Content 

            int i = 0;
            byte[] bytes = new byte[MessageHeader.Length + this._BodyLength];
            byte[] buffer = this._Header.ToBytes();
            Buffer.BlockCopy(buffer, 0, bytes, 0, buffer.Length);
            i += MessageHeader.Length;

            bytes[i++] = (byte)this._MsgType;       //[12,12] 
            bytes[i++] = (byte)this._NeedReport;    //[13,13] 
            bytes[i++] = (byte)this._Priority;      //[14,14] 

            //ServiceId  
            buffer = Encoding.ASCII.GetBytes(this._ServiceID);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); //10 //[15,24] 

            //FeeType 
            i += 10;
            buffer = Encoding.ASCII.GetBytes(this._FeeType);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); //2 //[25,26] 

            //FeeCode 
            i += 2;
            buffer = Encoding.ASCII.GetBytes(this._FeeCode);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); //6 //[27,32] 

            //FixedFee 
            i += 6;
            buffer = Encoding.ASCII.GetBytes(this._FixedFee);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); //6 //[33,38] 

            //MsgFormat
            i += 6;
            bytes[i++] = (byte)this._MsgFormat;      //1 //[39,39] 

            //ValId_Time 
            buffer = Encoding.ASCII.GetBytes(this._ValidTime);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); //17 //[40,56] 

            //AtTime 
            i += 17;
            buffer = Encoding.ASCII.GetBytes(this._AtTime);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); //17 //[57,73] 

            //SrcTermID
            i += 17;
            buffer = Encoding.ASCII.GetBytes(this._SrcTermID);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); //21 //[74,94] 

            //ChargeTermID
            i += 21;
            buffer = Encoding.ASCII.GetBytes(this._ChargeTermID);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); //21 //[95,115] 

            //DestTermIDCount
            i += 21;
            this._DestTermIDCount = (uint)this._DestTermID.Length;
            bytes[i++] = (byte)this._DestTermIDCount;            //1 //[116,116] 

            //DestTermID 
            foreach (string s in this._DestTermID)
            {
                buffer = Encoding.ASCII.GetBytes(s);
                Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
                i += 21;
            }

            //MsgLength 
            bytes[i++] = (byte)this._MsgLength;

            //MsgContent 
            //buffer = Encoding. 
            Buffer.BlockCopy(this._MsgContent_Bytes, 0, bytes, i, this._MsgContent_Bytes.Length);

            //Reserve 
            i += (int)this._MsgLength;
            buffer = Encoding.ASCII.GetBytes(this._Reserve);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); //8 

            //tlv
            i += 8;

            if (_OtherTlvArray != null)
            {
                for (int j = 0; j < _OtherTlvArray.Length; j++)
                {
                    if (_OtherTlvArray[j].Length > 0)
                    {
                        Buffer.BlockCopy(_OtherTlvArray[j].TlvBuf, 0, bytes, i, _OtherTlvArray[j].Length + 4); //8 
                        i += _OtherTlvArray[j].Length + 4;
                    }
                }
            }

            return bytes;
        }

        public void AddTlv(short tag, string value)
        {
            if (this._OtherTlvArray == null)
            {
                Tlv[] tmp = new Tlv[1];
                tmp[0] = new Tlv(tag, value);
                this._OtherTlvArray = tmp;
            }
            else
            {
                int find = 0;
                for (int i = 0; i < _OtherTlvArray.Length; i++)
                {
                    if (_OtherTlvArray[i].Tag == tag)
                    {
                        find = 1;
                        _OtherTlvArray[i] = new Tlv(tag, value);
                    }
                }
                if (find == 0)
                {
                    Tlv[] tmp = new Tlv[_OtherTlvArray.Length + 1];
                    Array.Copy(_OtherTlvArray, tmp, _OtherTlvArray.Length);
                    tmp[_OtherTlvArray.Length] = new Tlv(tag, value);
                    this._OtherTlvArray = tmp;
                }
            }
            // this.SetHeader();
        }

        /// <summary>
        /// 短消息类型
        /// 
        /// 对于回执消息该字段无效；对于文本短消息，该字段表示短消息的消息流向：
        /// 0＝MO 消息（终端发给SP）；
        /// 6＝MT 消息（SP 发给终端，包括WEB 上发送的点对点短消息）；
        /// 7＝点对点短消息；
        /// </summary>
        public uint MsgType
        {
            get
            {
                return this._MsgType;
            }
            set
            {
                this._MsgType = value;
            }
        }

        /// <summary>
        /// 是否要求返回状态报告
        /// 0＝不要求返回状态报告；
        /// 1＝要求返回状态报告；
        /// </summary>
        public uint NeedReport
        {
            get
            {
                return this._NeedReport;
            }
            set
            {
                this._NeedReport = value;
            }
        }

        /// <summary>
        /// 短消息发送优先级。
        /// 0＝低优先级；
        /// 1＝普通优先级；
        /// 2＝较高优先级；
        /// 3＝高优先级；
        /// </summary>
        public uint Priority
        {
            get
            {
                return this._Priority;
            }
            set
            {
                this._Priority = value;
            }
        }

        /// <summary>
        /// 业务代码
        /// </summary>
        public string ServiceID
        {
            get
            {
                return this._ServiceID;
            }
            set
            {
                this._ServiceID = value;
            }
        }

        /// <summary>
        /// 对计费用户采取的收费类型。
        /// 
        /// 00＝免费，此时FixedFee 和FeeCode 无效；
        /// 01＝按条计信息费，此时FeeCode 表示每条费用，FixedFee 无效；
        /// 02＝按包月收取信息费，此时FeeCode 无效，FixedFee 表示包月费用；
        /// 03＝按封顶收取信息费，若按条收费的费用总和达到或超过封顶费后，则按照封顶费用收取信息费；
        ///     若按条收费的费用总和没有达到封顶费用，则按照每条费用总和收取信息费。FeeCode表示每条费用，FixedFee 表示封顶费用。
        /// </summary>
        public string FeeType
        {
            get
            {
                return this._FeeType;
            }
            set
            {
                this._FeeType = value;
            }
        }

        /// <summary>
        /// 每条短消息费率
        /// 
        /// 单位为“分”。
        /// </summary>
        public string FeeCode
        {
            get
            {
                return this._FeeCode;
            }
            set
            {
                this._FeeCode = value;
            }
        }

        /// <summary>
        /// 短消息的包月费/封顶费
        /// 
        /// 单位为“分”
        /// </summary>
        public string FixedFee
        {
            get
            {
                return this._FixedFee;
            }
            set
            {
                this._FixedFee = value;
            }
        }

        /// <summary>
        /// 短消息内容体的编码格式。
        /// 
        /// 0＝ASCII 编码；
        /// 3＝短消息写卡操作；
        /// 4＝二进制短消息；
        /// 8＝UCS2 编码；
        /// 15＝GB18030 编码；
        /// 246（F6）＝(U)SIM 相关消息；
        /// 对于文字短消息，要求MsgFormat＝15。对于回执消息，要求MsgFormat＝0。
        /// </summary>
        public uint MsgFormat
        {
            get
            {
                return this._MsgFormat;
            }
            set
            {
                this._MsgFormat = value;
            }
        }

        /// <summary>
        /// 短消息有效时间
        /// 
        /// 格式遵循SMPP3.3 以上版本协议
        /// 短消息有效时间在转发过程中保持不变
        /// </summary>
        public string ValidTime
        {
            get
            {
                return this._ValidTime;
            }
            set
            {
                this._ValidTime = value;
            }
        }

        /// <summary>
        /// 短消息定时发送时间
        /// 
        /// 格式遵循SMPP3.3 以上版本协议
        /// 短消息定时发送时间在转发过程中保持不变
        /// </summary>
        public string AtTime
        {
            get
            {
                return this._AtTime;
            }
            set
            {
                this._AtTime = value;
            }
        }

        /// <summary>
        /// 短消息发送方号码。
        /// 
        /// 对于MT 消息，SrcTermID 格式为“118＋SP 服务代码＋其它（可选）”
        /// 例如SP 服务代码为1234 时，SrcTermID 可以为1181234 或118123456 等。
        /// </summary>
        public string SrcTermID
        {
            get
            {
                return this._SrcTermID;
            }
            set
            {
                this._SrcTermID = value;
            }
        }

        /// <summary>
        /// 计费用户号码
        /// 
        /// ChargeTermID 为空时，表示对被叫用户号码计费；
        /// ChargeTermID 为非空时，表示对计费用户号码计费。
        /// </summary>
        public string ChargeTermID
        {
            get
            {
                return this._ChargeTermID;
            }
            set
            {
                this._ChargeTermID = value;
            }
        }

        /// <summary>
        /// 短消息接收号码总数（≤100），用于SP 实现群发短消息。
        /// </summary>
        public uint DestTermIDCount
        {
            get
            {
                return this._DestTermIDCount;
            }
            set
            {
                this._DestTermIDCount = value;
            }
        }

        /// <summary>
        /// 短消息接收号码。
        /// </summary>
        public string[] DestTermID
        {
            get
            {
                return this._DestTermID;
            }
            set
            {
                this._DestTermID = value;
            }
        }

        /// <summary>
        /// 短消息长度
        /// 
        /// 指MsgContent 域的长度，取值大于或等于0。对于MT 消息，取值应小于或等于140
        /// </summary>
        public uint MsgLength
        {
            get
            {
                return this._MsgLength;
            }
            set
            {
                _MsgLength = value;
            }
        }

        /// <summary>
        /// 短消息内容
        /// </summary>
        public string MsgContent
        {
            get
            {
                return this._MsgContent;
            }
            set
            {
                this._MsgContent = value;
            }
        }

        /// <summary>
        /// 保留字段
        /// </summary>
        public string Reserve
        {
            get
            {
                return this._Reserve;
            }
            set
            {
                this._Reserve = value;
            }
        }

        /// <summary>
        /// 产品ID   =MServiceID
        /// </summary>
        public string ProductID
        {
            get { return _ProductID; }
            set
            {
                _ProductID = value;
                AddTlv(TlvId.Mserviceid, _ProductID);
                // this.SetHeader();
            }
        }

        public string LinkID
        {
            get { return _LinkID; }
            set
            {
                _LinkID = value;
                if (_LinkID == null) _LinkID = "";
                AddTlv(TlvId.LinkID, _LinkID);
                //   this.SetHeader();
            }
        }

        public Tlv[] OtherTlvArray
        {
            get { return _OtherTlvArray; }
            //set { _OtherTlvArray = value; }
        }

        public override string ToString()
        {
            return "[\r\n"
               + this._Header.ToString() + "\r\n"
               + string.Format
               (
                "\tMessageBody:"
                + "\r\n\t\tMsgType: {0}"
                + "\r\n\t\tNeedReport: {1}"
                + "\r\n\t\tPriority: {2}"
                + "\r\n\t\tServiceID: {3}"
                + "\r\n\t\tFeeType: {4}"
                + "\r\n\t\tFeeCode: {5}"
                + "\r\n\t\tFixedFee: {6}"
                + "\r\n\t\tMsgFormat: {7}"
                + "\r\n\t\tValidTime: {8}"
                + "\r\n\t\tAtTime: {9}"
                + "\r\n\t\tSrcTermID: {10}"
                + "\r\n\t\tChargeTermID: {11}"
                + "\r\n\t\tDestTermIDCount: {12}"
                + "\r\n\t\tDestTermID: {13}"
                + "\r\n\t\tMsgLength: {14}"
                + "\r\n\t\tMsgContent: {15}"
                + "\r\n\t\tReserve: {16}"
                + "\r\n\t\tProductID: {17}"
                + "\r\n\t\tLinkID: {18}"
                + "\r\n\t\tOtherTlvArray: {19}"
                , this._MsgType
                , this._NeedReport
                , this._Priority
                , this._ServiceID
                , this._FeeType
                , this._FeeCode
                , this._FixedFee
                , this._MsgFormat
                , this._ValidTime
                , this._AtTime
                , this._SrcTermID
                , this._ChargeTermID
                , this._DestTermIDCount
                , String.Join(",", this._DestTermID)
                , this._MsgLength
                , this._MsgContent
                , this._Reserve
                , this._ProductID
                , this._LinkID
                , this._OtherTlvArray.ToString()
               )
             + "\r\n]";
        }

        public uint SequenceID
        {
            get
            {
                return _Header.SequenceID;
            }
            set
            {
                _Header.SequenceID = value;
            }
        }

        public string MsgSrc
        {
            get
            {
                return this._MsgSrc;
            }
            set
            {
                this._MsgSrc = value;
            }
        }

        public SMGP3_COMMAND Command
        {
            get { return SMGP3_COMMAND.Submit; }
        }

    }
}
