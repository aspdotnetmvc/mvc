using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Active_Test:ISMGP_MESSAGE
    {
        private MessageHeader _Header;

        public MessageHeader Header
        {
            get
            {
                return this._Header;
            }
        }

        public Active_Test(uint Sequence_Id)
        {
            this._Header = new MessageHeader(MessageHeader.Length, SMGP3_COMMAND.Active_Test, Sequence_Id);
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
                return _Header.SequenceID;
            }
            set
            {
                _Header.SequenceID = value;
            }
        }

        public SMGP3_COMMAND Command
        {
            get { return SMGP3_COMMAND.Active_Test; }
        }

    }
}
