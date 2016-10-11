using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Query_Resp
    {
        public MessageHeader Header
        {
            get
            {
                return this._Header;
            }
        }

        public string QueryTime
        {
            get
            {
                return this._QueryTime;
            }
        }

        public uint QueryType
        {
            get
            {
                return this._QueryType;
            }
        }

        public string QueryCode
        {
            get
            {
                return this._QueryCode;
            }
        }

        public uint Mt_TlMsg
        {
            get
            {
                return this._MT_TLMsg;
            }
        }

        public uint Mt_Tlusr
        {
            get
            {
                return this._MT_Tlusr;
            }
        }

        public uint Mt_Scs
        {
            get
            {
                return this._MT_Scs;
            }
        }

        public uint MT_WT
        {
            get
            {
                return this._MT_WT;
            }
        }

        public uint MT_FL
        {
            get
            {
                return this._MT_FL;
            }
        }

        public uint MO_Scs
        {
            get
            {
                return this._MO_Scs;
            }
        }

        public uint MO_WT
        {
            get
            {
                return this._MO_WT;
            }
        }

        public uint MO_FL
        {
            get
            {
                return this._MO_FL;
            }
        }

        private MessageHeader _Header;
        private string _QueryTime; // 8 Octet String 时间(精确至日)。 
        private uint _QueryType; // 1 Unsigned Integer 查询类别: 
        //   0:总数查询; 
        //   1:按业务类型查询。 
        private string _QueryCode; // 10 Octet String 查询码。 
        private uint _MT_TLMsg; // 4 Unsigned Integer 从SP接收信息总数。 
        private uint _MT_Tlusr; // 4 Unsigned Integer 从SP接收用户总数。 
        private uint _MT_Scs; // 4 Unsigned Integer 成功转发数量。 
        private uint _MT_WT; // 4 Unsigned Integer 待转发数量。 
        private uint _MT_FL; // 4 Unsigned Integer 转发失败数量。 
        private uint _MO_Scs; // 4 Unsigned Integer 向SP成功送达数量。 
        private uint _MO_WT; // 4 Unsigned Integer 向SP待送达数量。 
        private uint _MO_FL; // 4 Unsigned Integer 向SP送达失败数量。 

        public const int BodyLength = 8 // Octet String 时间(精确至日)。 
           + 1 // Unsigned Integer 查询类别: 
            //  0:总数查询; 
            //  1:按业务类型查询。 
           + 10 // Octet String 查询码。 
           + 4 // Unsigned Integer 从SP接收信息总数。 
           + 4 // Unsigned Integer 从SP接收用户总数。 
           + 4 // Unsigned Integer 成功转发数量。 
           + 4 // Unsigned Integer 待转发数量。 
           + 4 // Unsigned Integer 转发失败数量。 
           + 4 // Unsigned Integer 向SP成功送达数量。 
           + 4 // Unsigned Integer 向SP待送达数量。 
           + 4; // Unsigned Integer 向SP送达失败数量。 

        public Query_Resp(byte[] bytes)
        {
            int i = 0;
            //header 12 
            byte[] buffer = new byte[MessageHeader.Length];
            Buffer.BlockCopy(bytes, 0, buffer, 0, buffer.Length);
            this._Header = new MessageHeader(buffer);

            //Time 8 
            i += MessageHeader.Length;
            buffer = new byte[8];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            this._QueryTime = Encoding.ASCII.GetString(buffer);

            //Query_Type 1 
            i += 8;
            this._QueryType = (uint)bytes[i++];

            //Query_Code 10 
            buffer = new byte[10];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            this._QueryCode = Encoding.ASCII.GetString(buffer);

            //MT_TLMsg 4 
            i += 10;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._MT_TLMsg = BitConverter.ToUInt32(buffer, 0);

            //MT_Tlusr 4 
            i += 4;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._MT_Tlusr = BitConverter.ToUInt32(buffer, 0);

            //MT_Scs 4 
            i += 4;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._MT_Scs = BitConverter.ToUInt32(buffer, 0);

            //MT_WT 4 
            i += 4;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._MT_WT = BitConverter.ToUInt32(buffer, 0);

            //MT_FL 4 
            i += 4;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._MT_FL = BitConverter.ToUInt32(buffer, 0);

            //MO_Scs 4 
            i += 4;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._MO_Scs = BitConverter.ToUInt32(buffer, 0);

            //MO_WT 4 
            i += 4;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._MO_WT = BitConverter.ToUInt32(buffer, 0);

            //MO_FL 4 
            i += 4;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._MO_FL = BitConverter.ToUInt32(buffer, 0);
        }

        public override string ToString()
        {
            return "[\r\n"
             + this._Header.ToString() + "\r\n"
             + string.Format
              (
               "\tMessageBody:"
               + "\r\n\t\tBodyLength: {0}"
               + "\r\n\t\tMO_FL: {1}"
               + "\r\n\t\tMO_Scs: {2}"
               + "\r\n\t\tMO_WT: {3}"
               + "\r\n\t\tMT_FL: {4}"
               + "\r\n\t\tMT_Scs: {5}"
               + "\r\n\t\tMT_TLMsg: {6}"
               + "\r\n\t\tMT_Tlusr: {7}"
               + "\r\n\t\tMT_WT: {8}"
               + "\r\n\t\tQueryCode: {9}"
               + "\r\n\t\tQueryType: {10}"
               + "\r\n\t\tQueryTime: {11}"

               , Query_Resp.BodyLength
               , this._MO_FL
               , this._MO_Scs
               , this._MO_WT
               , this._MT_FL
               , this._MT_Scs
               , this._MT_TLMsg
               , this._MT_Tlusr
               , this._MT_WT
               , this._QueryCode
               , this._QueryType
               , this._QueryTime
              )
                  + "\r\n]";
        }
    }
}
