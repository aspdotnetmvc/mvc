using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    public class SGIP_BIND_RESP:ISGIP_MESSAGE // 双向 
    {
        private SGIP_MESSAGE _Header;
        public const int BodyLength = 1 + 8;

        private uint _Result;       // 1 Unsigned Integer 执行命令是否成功  0：执行成功  其它：错误码
        private string _Reserve;    // 8 Octet String  保留，扩展用

        public SGIP_BIND_RESP(uint Result, uint SrcNodeSequence, uint DateSequence, uint Sequence_Id)
        {
            this._Header = new SGIP_MESSAGE(SGIP_MESSAGE.Length + BodyLength, SGIP_COMMAND.SGIP_BIND_RESP, SrcNodeSequence, DateSequence, Sequence_Id);

            this._Result = Result;
            this._Reserve = "null";
        }

        public SGIP_BIND_RESP(byte[] bytes)
        {
            //header 20
            int i = 0;
            string s = null;
            byte[] buffer = new byte[SGIP_MESSAGE.Length];
            Buffer.BlockCopy(bytes, 0, buffer, 0, buffer.Length);
            this._Header = new SGIP_MESSAGE(buffer);


            //Result 1 
            i += SGIP_MESSAGE.Length;
            this._Result = (uint)bytes[i++];

            //Result 8
            buffer = new byte[8];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._Reserve = s;
        }

        public byte[] GetBytes()
        {
            int i = 0;
            byte[] bytes = new byte[SGIP_MESSAGE.Length + BodyLength];

            //header 20
            byte[] buffer = this._Header.ToBytes();
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);

            //Result 1
            i += SGIP_MESSAGE.Length;
            bytes[i++] = (byte)this._Result;

            //Result 8
            buffer = Encoding.ASCII.GetBytes(this._Reserve);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            return (bytes);
        }

        public override string ToString()
        {
            return string.Format("Header={0}   Result={1}    Reserve={2}"
                , this._Header.ToString()
                , this._Result
                , this._Reserve);
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public uint Result
        {
            get { return _Result; }
        }

        /// <summary>
        /// 获取保留信息
        /// </summary>
        public string Reserve
        {
            get { return _Reserve; }
        }

        public SGIP_MESSAGE Header
        {
            get
            {
                return this._Header;
            }
        }

        public uint SequenceID
        {
            get
            {
                return _Header.Sequence_Id;
            }
            set
            {
                _Header.Sequence_Id = value;
            }
        }

        public SGIP_COMMAND Command
        {
            get { return SGIP_COMMAND.SGIP_BIND_RESP; }
        }

    }
}
