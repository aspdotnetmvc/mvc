using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    public class SGIP_REPORT_RESP : ISGIP_MESSAGE
    {
        public const int Bodylength = 1 + 8;
        private SGIP_MESSAGE _Header;

        private uint _Result;
        private string _Reserve;


        public SGIP_REPORT_RESP
        (
          uint Result
          , string Reserve
          , uint SrcNodeSequence
          , uint DateSequence
          , uint Sequence_Id
         )
        {
            this._Header = new SGIP_MESSAGE(SGIP_MESSAGE.Length + Bodylength, SGIP_COMMAND.SGIP_REPORT_RESP, SrcNodeSequence, DateSequence, Sequence_Id);
            this._Result = Result;
            this._Reserve = Reserve;
        }

        public byte[] GetBytes()
        {
            int i = 0;
            byte[] bytes = new byte[SGIP_MESSAGE.Length + Bodylength];

            byte[] buffer = this._Header.ToBytes();
            Buffer.BlockCopy(buffer, 0, bytes, 0, buffer.Length);
            i += SGIP_MESSAGE.Length;

            //Result 1 
            bytes[i++] = (byte)this._Result;

            //Reserve 8 
            buffer = Encoding.ASCII.GetBytes(this._Reserve);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            return bytes;

        }

        public override string ToString()
        {
            return this._Header.ToString() + "\r\n"
             + string.Format
              (
               "[\r\nMessageBody:"
               + "\r\n\tResult: {0}"
               + "\r\n\tReserve: {1}"
               + "\r\n]"
               , this._Result
               , this._Reserve
              );
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
            get { return SGIP_COMMAND.SGIP_REPORT_RESP; }
        }

    }
}
