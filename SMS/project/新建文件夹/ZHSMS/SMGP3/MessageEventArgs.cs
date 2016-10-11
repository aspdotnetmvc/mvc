using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    /// <summary>
    /// 消息包
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        private byte[] _HeaderData;
        private MessageHeader _Header;
        private byte[] _BodyData;

        public byte[] BodyData
        {
            get
            {
                return this._BodyData;
            }
        }

        public MessageHeader Header
        {
            get
            {
                return this._Header;
            }
        }

        public byte[] HeaderData
        {
            get
            {
                return this._HeaderData;
            }
        }

        public MessageEventArgs(byte[] bytes)
        {
            this._HeaderData = new byte[MessageHeader.Length];
            Buffer.BlockCopy(bytes, 0, this._HeaderData, 0, MessageHeader.Length);
            this._Header = new MessageHeader(this._HeaderData);
            this._BodyData = new byte[this._Header.PacketLength - MessageHeader.Length];
            Buffer.BlockCopy(bytes, MessageHeader.Length, this._BodyData, 0, this._BodyData.Length);
        }
    }
}
