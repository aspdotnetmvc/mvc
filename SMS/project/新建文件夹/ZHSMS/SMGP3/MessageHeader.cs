using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    /// <summary>
    /// 消息头
    /// </summary>
    public class MessageHeader
    {
        public const int Length = 4 + 4 + 4;

        public SMGP3_COMMAND Command
        {
            get
            {
                return this._Command;
            }
            set
            {
                this._Command = value;
            }
        }

        public uint SequenceID
        {
            get
            {
                return this._SequenceID;
            }
            set
            {
                this._SequenceID = value;
            }
        }

        public uint PacketLength
        {
            get
            {
                return this._PacketLength;
            }
        }

        private uint _PacketLength;             // 4 Unsigned Integer 数据包长度 
        private SMGP3_COMMAND _Command;     // 4 Unsigned Integer 命令或响应类型 
        private uint _SequenceID;               // 4 Unsigned Integer 消息流水号

        public MessageHeader(uint PacketLength, SMGP3_COMMAND command, uint SequenceID) //发送前 
        {
            this._PacketLength = PacketLength;
            this._Command = command;
            this._SequenceID = SequenceID;
        }

        public MessageHeader(byte[] bytes)
        {
            byte[] buffer = new byte[4];
            Buffer.BlockCopy(bytes, 0, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._PacketLength = BitConverter.ToUInt32(buffer, 0);

            Buffer.BlockCopy(bytes, 4, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._Command = (SMGP3_COMMAND)BitConverter.ToUInt32(buffer, 0);

            Buffer.BlockCopy(bytes, 8, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._SequenceID = BitConverter.ToUInt32(buffer, 0);
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[MessageHeader.Length];

            byte[] buffer = BitConverter.GetBytes(this._PacketLength);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, 0, 4);

            buffer = BitConverter.GetBytes((uint)this._Command);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, 4, 4);

            buffer = BitConverter.GetBytes(this._SequenceID);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, 8, 4);

            return bytes;
        }

        public override string ToString()
        {
            return string.Format("MessageHeader: RequestId={0}    Sequence_Id={1}    Total_Length={2}"
              , this._Command
              , this._SequenceID
              , this._PacketLength
             );

        }
    }
}
