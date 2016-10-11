using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Submit_Resp
    {
        private MessageHeader _Header;
        private string _MsgID;
        private uint _Result;

        public const int BodyLength = 10 + 4;

        /// <summary>
        /// 短消息流水号
        /// </summary>
        public string MsgID
        {
            get
            {
                return this._MsgID;
            }
        }

        /// <summary>
        /// 请求返回结果
        /// </summary>
        public uint Result
        {
            get
            {
                return this._Result;
            }
        }

        public MessageHeader Header
        {
            get
            {

                return this._Header;
            }
        }

        public Submit_Resp(byte[] bytes)
        {
            int i = 0;
            byte[] buffer = new byte[MessageHeader.Length];
            Buffer.BlockCopy(bytes, 0, buffer, 0, buffer.Length);
            this._Header = new MessageHeader(buffer);

            //string s = null;
            ////MsgId 10 
            //i += MessageHeader.Length;
            //buffer = new byte[10];
            //Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            //s = Encoding.ASCII.GetString(buffer).Trim();
            //s = s.Substring(0, s.IndexOf('\0'));
            //this._MsgID = s;

            // Msg_Id 
            i += MessageHeader.Length;
            buffer = new byte[10];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            this._MsgID = System.BitConverter.ToString(buffer).Replace("-","");
            //     string test = System.BitConverter.ToString(buffer);
            //     System.Console.WriteLine("Submit_Resp=" + test);

            // Array.Reverse(buffer);
            // this._MsgID = BitConverter.ToUInt32(buffer, 0);
            ;

            // Result 
            i += 10;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._Result = BitConverter.ToUInt32(buffer, 0);
        }

        public override string ToString()
        {
            return string.Format("Header={0}    Msg_Id={1}    tResult={2}"
                , this._Header.ToString()
                , this._MsgID
                , this._Result);
        }
    }
}
