using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Query
    {
        private MessageHeader _Header;

        private string _QueryTime; // 8 Octet String 时间YYYYMMDD(精确至日)。 
        private uint _QueryType; // 1 Unsigned Integer 查询类别: 
        //   0:总数查询; 
        //   1:按业务类型查询。 
        private string _QueryCode; // 10 Octet String 查询码。 

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

        public const int BodyLength = 8 + 1 + 10;

        public Query(DateTime Time, uint Query_Type, string Query_Code, uint Sequence_Id)
        {
            this._QueryTime = Util.Get_YYYYMMDD_String(Time);
            this._QueryType = Query_Type;
            this._QueryCode = Query_Code;
            this._Header = new MessageHeader((uint)(MessageHeader.Length + BodyLength), SMGP3_COMMAND.Query, Sequence_Id);
        }

        public byte[] ToBytes()
        {
            int i = 0;
            byte[] bytes = new byte[MessageHeader.Length + BodyLength];
            //header 

            byte[] buffer = new byte[MessageHeader.Length];
            buffer = this._Header.ToBytes();
            buffer.CopyTo(bytes, 0);

            //Time 8 
            i += MessageHeader.Length;
            buffer = new byte[10];
            buffer = Encoding.ASCII.GetBytes(this._QueryTime);
            buffer.CopyTo(bytes, i);

            //Query_Type 1 
            i += 8;
            bytes[i++] = (byte)this._QueryType;

            //Query_Code 10 
            buffer = new byte[10];
            buffer = Encoding.ASCII.GetBytes(this._QueryCode);
            buffer.CopyTo(bytes, i);

            return bytes;
        }

        public override string ToString()
        {
            return "[\r\n" + this._Header.ToString() + "\r\n"
             + string.Format
              (
               "\tMessageBody:"
               + "\r\n\t\tQueryCode: {0}"
               + "\r\n\t\tQueryType: {1}"
               + "\r\n\t\tQueryTime: {2}"
               + "\r\n]"
               , this._QueryCode
               , this._QueryType
               , this._QueryTime
              )
             + "\r\n]";
        }
    }
}
