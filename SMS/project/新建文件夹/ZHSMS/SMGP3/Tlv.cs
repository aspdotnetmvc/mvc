using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Tlv
    {
        public short Tag;
        public short Length;
        public String Value;
        public byte[] TlvBuf;

        public Tlv(short tag, string value)
        {
            this.Tag = tag;
            this.Length = (short)value.Length;
            this.Value = value;
            if (tag == TlvId.Mserviceid || tag == TlvId.MsgSrc
                    || tag == TlvId.SrcTermPseudo || tag == TlvId.DestTermPseudo
                    || tag == TlvId.ChargeTermPseudo || tag == TlvId.LinkID)
            {
                //参数为oct string
                int i = 0;
                this.TlvBuf = new byte[4 + this.Length];

                //Tag  2
                byte[] buffer = BitConverter.GetBytes((short)Tag);
                Array.Reverse(buffer);
                buffer.CopyTo(TlvBuf, i);

                //Length  2
                i += 2;
                buffer = BitConverter.GetBytes(Length);
                Array.Reverse(buffer);
                buffer.CopyTo(TlvBuf, i);

                //Value Length
                i += 2;
                buffer = Encoding.ASCII.GetBytes(this.Value);
                Buffer.BlockCopy(buffer, 0, TlvBuf, i, buffer.Length);
            }
            else
            {
                int i = 0;
                this.TlvBuf = new byte[4 + 1];

                //Tag 2
                byte[] buffer = BitConverter.GetBytes((short)Tag);
                Array.Reverse(buffer);
                buffer.CopyTo(TlvBuf, i);

                //Length  2
                i += 2;
                buffer = BitConverter.GetBytes(Length);
                Array.Reverse(buffer);
                buffer.CopyTo(TlvBuf, i);

                //Value 1
                i += 2;
                uint tmp = 0;
                uint.TryParse(value, out tmp);
                TlvBuf[i] = (byte)tmp;
            }
        }
    }
}
