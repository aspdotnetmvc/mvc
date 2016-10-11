using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    public class SGIP_DELIVER
    {

        private SGIP_MESSAGE _Header;

        private string _UserNumber; //发送短消息的用户手机号，手机号码前加“86”国别标志
        private string _SPNumber;   //SP的接入号码
        private uint _TP_pid; // 1 Unsigned Integer GSM协议类型。详细解释请参考GSM03.40中的9.2.3.9。 
        private uint _TP_udhi; // 1 Unsigned Integer GSM协议类型。详细解释请参考GSM03.40中的9.2.3.23,仅使用1位,右对齐。 
        private uint _MessageCoding;
        private uint _MessageLength;
        private string _MessageContent;
        private string _LinkID;



        public const int FixedBodyLength = 21 // UserNumber Octet String    信息标识。 
                + 21    // SPNumber Octet String SP的接入号码
                 + 1    // TP_pid Unsigned Integer GSM协议类型。详细解释请参考GSM03.40中的9.2.3.9。 
                 + 1    // TP_udhi Unsigned Integer GSM协议类型。详细解释请参考GSM03.40中的9.2.3.23,仅使用1位,右对齐。 
                 + 1    // MessageCoding  Unsigned Integer
                 + 4    // MessageLength Unsigned Integer
            //+Message Length // MessageLength Octet String 消息内容。 
                + 8;      //LinkID Octet String

        private int _BodyLength;
        public int BodyLength
        {
            get { return this._BodyLength; }
        }

        public SGIP_DELIVER(byte[] bytes)
        {
            int i = 0;
            string s = null;

            byte[] buffer = new byte[SGIP_MESSAGE.Length];
            Buffer.BlockCopy(bytes, 0, buffer, 0, SGIP_MESSAGE.Length);
            this._Header = new SGIP_MESSAGE(buffer);

            //UserNumber 21
            i += SGIP_MESSAGE.Length;
            buffer = new byte[21];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            if (s != null && s.Length > 11 && s.Substring(0, 2) == "86")
                s = s.Substring(2, s.Length - 2);
            this._UserNumber = s;

            //SPNumber 21
            i += 21;
            buffer = new byte[21];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._SPNumber = s;

            //
            i += 21;
            this._TP_pid = (uint)bytes[i++];
            this._TP_udhi = (uint)bytes[i++];
            this._MessageCoding = (uint)bytes[i++];

            //MessageLength 4
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._MessageLength = BitConverter.ToUInt32(buffer, 0);

            //MessageContent 
            i += 4;
            buffer = new byte[this._MessageLength];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            switch (this._MessageCoding)
            {
                case 8:
                    this._MessageContent = Encoding.BigEndianUnicode.GetString(buffer).Trim();
                    break;
                case 15: //gb2312 
                    this._MessageContent = Encoding.GetEncoding("gb2312").GetString(buffer).Trim();
                    break;
                case 0: //ascii 
                case 3: //短信写卡操作 
                case 4: //二进制信息 
                default:
                    this._MessageContent = Encoding.ASCII.GetString(buffer).ToString();
                    break;
            }

            //Linkid 8 
            i += (int)this._MessageLength;
            buffer = new byte[8];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._LinkID = s;
        }

        public SGIP_MESSAGE Header
        {
            get
            {
                return this._Header;
            }
        }

        public string UserNumber
        {
            get { return _UserNumber; }
        }

        public string SPNumber
        {
            get { return _SPNumber; }
        }

        public uint TP_pid
        {
            get { return _TP_pid; }
        }

        public uint TP_udhi
        {
            get { return _TP_udhi; }
            set { _TP_udhi = value; }
        }

        public uint MessageCoding
        {
            get { return _MessageCoding; }
        }

        public uint MessageLength
        {
            get { return _MessageLength; }
        }

        public string MessageContent
        {
            get { return _MessageContent; }
        }

        public string LinkID
        {
            get { return _LinkID; }
        }

        public override string ToString()
        {
            return "[\r\n"
             + this._Header.ToString() + "\r\n"
             + string.Format
              (
               "\tMessageBody:"
               + "\r\n\t\tBodyLength: {0}"
               + "\r\n\t\tUserNumber: {1}"
               + "\r\n\t\tTP_pid: {2}"
               + "\r\n\t\tTP_udhi: {3}"
               + "\r\n\t\tMessageCoding: {4}"
               + "\r\n\t\tMessageLength: {5}"
               + "\r\n\t\tMessageContent: {6}"
               + "\r\n\t\tLinkID: {7}"
               , this._BodyLength
               , this._SPNumber
               , this._TP_pid
               , this._TP_udhi
               , this._MessageCoding
               , this._MessageLength
               , this._MessageContent
               , this._LinkID
              )
             + "\r\n]";
        }
    }
}
