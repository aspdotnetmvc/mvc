using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    public class SGIP_UNBIND_RESP:ISGIP_MESSAGE
    {
        private SGIP_MESSAGE _Header;

        public SGIP_MESSAGE Header
        {
            get
            {
                return this._Header;
            }
        }


        public SGIP_UNBIND_RESP
        (
           uint SrcNodeSequence
          , uint DateSequence
          , uint Sequence_Id
         )
        {
            this._Header = new SGIP_MESSAGE(SGIP_MESSAGE.Length, SGIP_COMMAND.SGIP_UNBIND_RESP, SrcNodeSequence, DateSequence, Sequence_Id);
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
                return _Header.Sequence_Id;
            }
            set
            {
                _Header.Sequence_Id = value;
            }
        }

        public SGIP_COMMAND Command
        {
            get { return SGIP_COMMAND.SGIP_UNBIND_RESP; }
        }

    }
}
