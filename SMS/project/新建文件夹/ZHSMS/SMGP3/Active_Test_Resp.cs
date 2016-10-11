using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Active_Test_Resp:ISMGP_MESSAGE
    {
        private MessageHeader _Header;
        
        public MessageHeader Header
        {
            get
            {
                return this._Header;
            }
            set
            {
                _Header = value;
            }
        }

        public Active_Test_Resp(uint Sequence_Id)
        {
            this._Header = new MessageHeader(MessageHeader.Length, SMGP3_COMMAND.Active_Test_Resp, Sequence_Id);
        }

        public Active_Test_Resp(byte[] bytes)
        {
            byte[] buffer = new byte[MessageHeader.Length];
            Buffer.BlockCopy(bytes, 0, buffer, 0, buffer.Length);
            this._Header = new MessageHeader(buffer);
        }

        public byte[] GetBytes()
        {
            return this._Header.ToBytes();
        }

        public override string ToString()
        {
            return this._Header.ToString();
        }


        public uint SequenceID
        {
            get
            {
                return this._Header.SequenceID;
            }
            set
            {
                this._Header.SequenceID = value;
            }
        }

        public SMGP3_COMMAND Command
        {
            get { return SMGP3_COMMAND.Active_Test_Resp; }
        }
    }
}
