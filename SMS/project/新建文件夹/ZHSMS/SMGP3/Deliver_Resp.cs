using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Deliver_Resp:ISMGP_MESSAGE
    {
        private MessageHeader _Header;
        private ulong _MsgID;
        private uint _Result;
        public const int Bodylength = 10 + 4;

        private uint _Sequence_Id;

        public Deliver_Resp(ulong MsgID, uint Result, uint Sequence_Id)
        {
            this._MsgID = MsgID;
            this._Result = Result;
            this._Sequence_Id = Sequence_Id;
        }

        public byte[] GetBytes()
        {
            int i = 0;
            byte[] bytes = new byte[MessageHeader.Length + Bodylength];

            byte[] buffer = new byte[MessageHeader.Length];
            //header 
            this._Header = new MessageHeader(MessageHeader.Length + Bodylength, SMGP3_COMMAND.Deliver_Resp, _Sequence_Id);
            buffer = this._Header.ToBytes();
            Buffer.BlockCopy(buffer, 0, bytes, 0, buffer.Length);

            //MsgID 10
            i += MessageHeader.Length;
            //buffer = Encoding.ASCII.GetBytes(this._MsgID);
            // Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            buffer = BitConverter.GetBytes(this._MsgID);
            Array.Reverse(buffer);
            buffer.CopyTo(bytes, i);

            //Status 4 
            i += 10;
            buffer = BitConverter.GetBytes(this._Result);
            Array.Reverse(buffer);
            buffer.CopyTo(bytes, i);
            return bytes;
        }

        public override string ToString()
        {
            return this._Header.ToString() + "\r\n"
             + string.Format
              (
               "[\r\nMessageBody:"
               + "\r\n\tMsgID: {0}"
               + "\r\n\tResult: {1}"
               + "\r\n]"
               , this._MsgID
               , this._Result
              );
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

        public SMGP3_COMMAND Command
        {
            get
            {
                return SMGP3_COMMAND.Deliver_Resp;
            }
        }

    }
}
