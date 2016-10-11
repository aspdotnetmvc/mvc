using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    public class SGIP_SUBMIT_RESP
    {
        private SGIP_MESSAGE _Header;
        private uint _Result;
        private string _Reserve;

        public const int BodyLength = 1 + 8;

        public uint Result
        {
            get
            {
                return this._Result;
            }
        }

        public string Reserve
        {
            get
            {
                return this._Reserve;
            }
        }

        public SGIP_MESSAGE Header
        {
            get
            {

                return this._Header;
            }
        }

        public SGIP_SUBMIT_RESP(byte[] bytes)
        {
            int i = 0;
            byte[] buffer = new byte[SGIP_MESSAGE.Length];
            Buffer.BlockCopy(bytes, 0, buffer, 0, buffer.Length);
            this._Header = new SGIP_MESSAGE(buffer);

            //Result 
            i += SGIP_MESSAGE.Length;
            this._Result = (uint)bytes[i++];

            //Reserve;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            string s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._Reserve = s;


        }
        public override string ToString()
        {
            return "[\r\n"
             + this._Header.ToString() + "\r\n"
             + string.Format
              (
               "\tMessageBody:"
               + "\r\n\t\tResult: {0}"
               + "\r\n\t\tReserve: {1}"

               , this._Result
               , this._Reserve
              )
             + "\r\n]";
        }
    }
}
