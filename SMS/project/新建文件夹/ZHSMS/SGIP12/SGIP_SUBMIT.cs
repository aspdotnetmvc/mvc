using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    public class SGIP_SUBMIT : ISGIP_MESSAGE
    {
        private int _BodyLength;

        public const int FixedBodyLength = 21   //SPNumber
                 + 21   //ChargeNumber
                 + 1    //UserCount
                 + 21    //UserNumber
                 + 5    //CorpId
                 + 10   //ServiceType
                 + 1    //FeeType
                 + 6    //FeeValue
                 + 6    //GivenValue
                 + 1    //AgentFlag
                 + 1    //MorelatetoMTFlag
                 + 1    //Priority
                 + 16   //ExpireTime
                 + 16   //ScheduleTime
                 + 1    //ReportFlag
                 + 1    //TP_pid
                 + 1    //TP_udhi
                 + 1    //MessageCoding
                 + 1    //MessageType
                 + 4    //MessageLength
            //+ Message Length  //MessageContent
                 + 8;   //Reserve

        private string _SPNumber;       //21 Octet String   SP的接入号码
        private string _ChargeNumber;   //21 Octet String   付费号码，手机号码前加“86”国别标志
        private uint _UserCount;        //1 Unsigned Integer 接收短消息的手机数量，取值范围1至100
        private string _UserNumber;     //21 Octet String   接收该短消息的手机号 手机号码前加“86”国别标志
        private string _CorpId;         //5  Octet String  企业代码，取值范围0-99999
        private string _ServiceType;    //10  Octet String  业务代码，由SP定义
        private uint _FeeType;          //1 Unsigned Integer 计费类型
        private string _FeeValue;       //6  Octet String  取值范围0-99999，该条短消息的收费值，单位为分，由SP定义对于包月制收费的用户，该值为月租费的值
        private string _GivenValue;     //6  Octet String  赠送用户的话费，单位为分
        private uint _AgentFlag;        //1 Unsigned Integer  代收费标志，0：应收；1：实收
        private uint _MorelatetoMTFlag;  //1 Unsigned Integer   引起MT消息的原因
        private uint _Priority;         //1 Unsigned Integer   优先级0-9从低到高，默认为0
        private string _ExpireTime;     //16  Octet String     短消息寿命的终止时间，如果为空，表示使用短消息中心的缺省值
        private string _ScheduleTime;   //16  Octet String     短消息定时发送的时间，如果为空，表示立刻发送该短消息
        private uint _ReportFlag;       //1 Unsigned Integer    状态报告标记
        private uint _TP_pid;           //1 Unsigned Integer
        private uint _TP_udhi;          //1 Unsigned Integer
        private uint _MessageCoding;    //1 Unsigned Integer    短消息的编码格式
        private uint _MessageType;      //1 Unsigned Integer    信息类型：0-短消息信息  其它：待定
        private uint _MessageLength;    //4 Unsigned Integer    短消息的长度
        private string _MessageContent;    // MessageLength Octet String 信息内容。 
        private string _LinkID;

        private SGIP_MESSAGE _Header;
        private uint _Sequence_Id;

        /// <summary>
        /// 协议范围外，长短信总数。
        /// </summary>
        public uint Pk_total
        {
            get;
            set;
        }

        /// <summary>
        /// 协议范围外，长短信序号。
        /// </summary>
        public uint Pk_number
        {
            get;
            set;
        }

        /// <summary>
        /// 协议范围外WAPPUSH
        /// </summary>
        public string WapURL
        {
            get;
            set;
        }

        public SGIP_SUBMIT(uint Sequence_Id)
        {
            this._Sequence_Id = Sequence_Id;
        }

        private byte[] _Msg_Content_Bytes;

        internal void SetHeader()
        {
            if (string.IsNullOrEmpty(WapURL))
            {
                //byte[] buf;
                switch (this._MessageCoding)
                {
                    case 8:
                        _Msg_Content_Bytes = Encoding.BigEndianUnicode.GetBytes(this._MessageContent);
                        break;
                    case 15: //gb2312
                        _Msg_Content_Bytes = Encoding.GetEncoding("gb2312").GetBytes(this._MessageContent);
                        break;
                    case 0: //ascii
                    case 3: //短信写卡操作
                    case 4: //二进制信息
                    default:
                        _Msg_Content_Bytes = Encoding.ASCII.GetBytes(this._MessageContent);
                        break;
                }

                if (this.Pk_total > 1) //长短信
                {
                    this.TP_udhi = 1;
                    byte[] tp_udhiHead = new byte[6];
                    tp_udhiHead[0] = 0x05;
                    tp_udhiHead[1] = 0x00;
                    tp_udhiHead[2] = 0x03;
                    tp_udhiHead[3] = 0x0A;
                    tp_udhiHead[4] = (byte)this.Pk_total;
                    tp_udhiHead[5] = (byte)this.Pk_number;
                    byte[] Msg_Content_Bytes_Temp = new byte[_Msg_Content_Bytes.Length + 6];
                    int index = 0;
                    tp_udhiHead.CopyTo(Msg_Content_Bytes_Temp, index);
                    index += tp_udhiHead.Length;
                    _Msg_Content_Bytes.CopyTo(Msg_Content_Bytes_Temp, index);
                    _Msg_Content_Bytes = Msg_Content_Bytes_Temp;
                }
            }
            else
            {
                _Msg_Content_Bytes = WapPush.GetInstance().toBytes(this._MessageContent, WapURL);
                this._MessageCoding = 0x04;
                this.TP_pid = 1;
                this.TP_udhi = 1;
            }

            this._MessageLength = (uint)_Msg_Content_Bytes.Length;
            this._BodyLength = (int)(FixedBodyLength + this._MessageLength);
            this._Header = new SGIP_MESSAGE((uint)(SGIP_MESSAGE.Length + this._BodyLength), SGIP_COMMAND.SGIP_SUBMIT, this._Sequence_Id);
        }

        public byte[] GetBytes()
        {
            //Msg_Length Msg_Content 
            int i = 0;
            byte[] bytes = new byte[SGIP_MESSAGE.Length + this._BodyLength];
            byte[] buffer = this._Header.ToBytes();
            Buffer.BlockCopy(buffer, 0, bytes, 0, buffer.Length);
            i += SGIP_MESSAGE.Length;

            //SPNumber //21
            buffer = Encoding.ASCII.GetBytes(this._SPNumber);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            i += 21;

            //ChargeNumber //21
            buffer = Encoding.ASCII.GetBytes(this._ChargeNumber);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            i += 21;

            //UserCount //1
            bytes[i++] = (byte)this._UserCount;

            //UserNumber //21
            buffer = Encoding.ASCII.GetBytes(this._UserNumber);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            i += 21;

            //CorpId    //5
            buffer = Encoding.ASCII.GetBytes(this._CorpId);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            i += 5;

            //ServiceType;    //10
            buffer = Encoding.ASCII.GetBytes(this._ServiceType);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            i += 10;

            //FeeType          //1
            bytes[i++] = (byte)this._FeeType;

            //FeeValue   //6
            buffer = Encoding.ASCII.GetBytes(this._FeeValue);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            i += 6;

            //GivenValue    //6
            buffer = Encoding.ASCII.GetBytes(this._GivenValue);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            i += 6;

            bytes[i++] = (byte)this._AgentFlag;
            bytes[i++] = (byte)this._MorelatetoMTFlag;
            bytes[i++] = (byte)this._Priority;

            //ExpireTime;     //16 
            buffer = Encoding.ASCII.GetBytes(this._ExpireTime);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            i += 16;

            //ScheduleTime;   //16 
            buffer = Encoding.ASCII.GetBytes(this._ScheduleTime);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            i += 16;

            bytes[i++] = (byte)this._ReportFlag;
            bytes[i++] = (byte)this._TP_pid;
            bytes[i++] = (byte)this._TP_udhi;
            bytes[i++] = (byte)this._MessageCoding;
            bytes[i++] = (byte)this._MessageType;

            //MessageLength //4
            buffer = BitConverter.GetBytes(this._MessageLength);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            i += 4;

            //Msg_Content 
            Buffer.BlockCopy(this._Msg_Content_Bytes, 0, bytes, i, this._Msg_Content_Bytes.Length);
            i += (int)this._MessageLength;

            //LinkID 
            buffer = Encoding.ASCII.GetBytes(this._LinkID);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            return bytes;
        }

        public SGIP_MESSAGE Header
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

        /// <summary>
        /// SP的接入号码
        /// </summary>
        public string SPNumber
        {
            get { return _SPNumber; }
            set { _SPNumber = value; }
        }
        /// <summary>
        /// 付费号码,设置时自动在号码前加“86”
        /// </summary>
        public string ChargeNumber
        {
            get { return _ChargeNumber; }
            set { _ChargeNumber = value; }
        }
        /// <summary>
        /// 接收短消息的手机数量 ，必须为1
        /// </summary>
        public uint UserCount
        {
            get { return _UserCount; }
            set { _UserCount = value; }
        }
        /// <summary>
        /// 接收该短消息的手机号,设置时自动在号码前加“86”
        /// </summary>
        public string UserNumber
        {
            get { return _UserNumber; }
            set
            {
                _UserNumber = value;
            }
        }
        /// <summary>
        /// 企业代码
        /// </summary>
        public string CorpId
        {
            get { return _CorpId; }
            set { _CorpId = value; }
        }
        /// <summary>
        /// 业务代码，由SP定义
        /// </summary>
        public string ServiceType
        {
            get { return _ServiceType; }
            set { _ServiceType = value; }
        }
        /// <summary>
        /// 计费类型
        /// </summary>
        public uint FeeType
        {
            get { return _FeeType; }
            set { _FeeType = value; }
        }
        /// <summary>
        /// 该条短消息的收费值，单位为分
        /// </summary>
        public string FeeValue
        {
            get { return _FeeValue; }
            set { _FeeValue = value; }
        }

        public string GivenValue
        {
            get { return _GivenValue; }
            set { _GivenValue = value; }
        }
        /// <summary>
        /// 代收费标志，0：应收；1：实收
        /// </summary>
        public uint AgentFlag
        {
            get { return _AgentFlag; }
            set { _AgentFlag = value; }
        }
        /// <summary>
        /// 引起MT消息的原因
        /// </summary>
        public uint MorelatetoMTFlag
        {
            get { return _MorelatetoMTFlag; }
            set { _MorelatetoMTFlag = value; }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public uint Priority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }

        /// <summary>
        /// 短消息寿命的终止时间
        /// </summary>
        public string ExpireTime
        {
            get { return _ExpireTime; }
            set { _ExpireTime = value; }
        }

        /// <summary>
        /// 短消息定时发送的时间
        /// </summary>
        public string ScheduleTime
        {
            get { return _ScheduleTime; }
            set { _ScheduleTime = value; }
        }

        /// <summary>
        /// 状态报告标记
        /// </summary>
        public uint ReportFlag
        {
            get { return _ReportFlag; }
            set { _ReportFlag = value; }
        }

        public uint TP_pid
        {
            get { return _TP_pid; }
            set { _TP_pid = value; }
        }

        public uint TP_udhi
        {
            get { return _TP_udhi; }
            set { _TP_udhi = value; }
        }

        /// <summary>
        /// 短消息的编码格式
        /// </summary>
        public uint MessageCoding
        {
            get { return _MessageCoding; }
            set
            {
                _MessageCoding = value;
            }
        }

        /// <summary>
        /// 信息类型：0-短消息信息  其它：待定
        /// </summary>
        public uint MessageType
        {
            get { return _MessageType; }
            set { _MessageType = value; }
        }

        /// <summary>
        /// 短消息的长度
        /// </summary>
        public uint MessageLength
        {
            get { return _MessageLength; }
            set { _MessageLength = value; }
        }
        /// <summary>
        /// 短消息的内容
        /// </summary>
        public string MessageContent
        {
            get
            {
                return this._MessageContent;
            }
            set
            {
                this._MessageContent = value;
            }
        }

        public string LinkID
        {
            get { return _LinkID; }
            set { _LinkID = value; }
        }

        public override string ToString()
        {
            return "[\r\n"
               + this._Header.ToString() + "\r\n"
               + string.Format
               (
                "\tMessageBody:"
                + "\r\n\t\tSPNumber: {0}"
                + "\r\n\t\tChargeNumber: {1}"
                + "\r\n\t\tUserCount: {2}"
                + "\r\n\t\tUserNumber: {3}"
                + "\r\n\t\tCorpId: {4}"
                + "\r\n\t\tServiceType {5}"
                + "\r\n\t\tFeeType: {6}"
                + "\r\n\t\tFeeValue: {7}"
                + "\r\n\t\tGivenValue: {8}"
                + "\r\n\t\tAgentFlag: {9}"
                + "\r\n\t\tMorelatetoMTFlag {10}"
                + "\r\n\t\tPriority: {11}"
                + "\r\n\t\tExpireTime: {12}"
                + "\r\n\t\tScheduleTime: {13}"
                + "\r\n\t\tReportFlag: {14}"
                + "\r\n\t\tTP_pid: {15}"
                + "\r\n\t\tTP_udhi: {16}"
                + "\r\n\t\tMessageCoding: {17}"
                + "\r\n\t\tMessageType: {18}"
                + "\r\n\t\tMessageLength: {19}"
                + "\r\n\t\tMessageContent: {20}"
                + "\r\n\t\tlinkID: {21}"
                + "\r\n\t\tSequence_Id: {22}"
                , this._SPNumber
                , this._ChargeNumber
                , this._UserCount
                , this._UserNumber
                , this._CorpId
                , this._ServiceType
                , this._FeeType
                , this._FeeValue
                , this._GivenValue
                , this._AgentFlag
                , this._MorelatetoMTFlag
                , this._Priority
                , this._ExpireTime
                , this._ScheduleTime
                , this._ReportFlag
                , this._TP_pid
                , this._TP_udhi
                , this._MessageCoding
                , this._MessageType
                , this._MessageLength
                , this._MessageContent
                , this._LinkID
                , this._Sequence_Id
               )
             + "\r\n]";
        }


        public uint SequenceID
        {
            get
            {
                return this._Sequence_Id;
            }
            set
            {
                this. _Sequence_Id = value;
                _Header.Sequence_Id = value;
            }
        }

        public SGIP_COMMAND Command
        {
            get { return SGIP_COMMAND.SGIP_SUBMIT; }
        }
    }
}
