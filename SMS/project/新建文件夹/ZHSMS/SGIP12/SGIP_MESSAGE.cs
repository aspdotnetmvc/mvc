using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    /// <summary>
    /// 消息头
    /// </summary>
    public class SGIP_MESSAGE
    {
        public const int Length = 4 + 4 + 4 + 4 + 4;

        /// <summary>
        /// 获取命令类型
        /// </summary>
        public SGIP_COMMAND Command_Id
        {
            get
            {
                return this._Command_Id;
            }
        }

        /// <summary>
        /// 获取序列号
        /// </summary>
        public uint Sequence_Id
        {
            get
            {
                return this._Sequence_Id;
            }
            set
            {
                this._Sequence_Id = value;
            }
        }


        /// <summary>
        /// 获取原节点
        /// </summary>
        public uint SrcNodeSequence
        {
            get { return _SrcNodeSequence; }
        }

        /// <summary>
        /// 获取命令产生的日期和时间
        /// </summary>
        public uint DateSequence
        {
            get { return _DateSequence; }
        }

        /// <summary>
        /// 获取总长度
        /// </summary>
        public uint Total_Length
        {
            get
            {
                return this._Total_Length;
            }
        }

        private uint _Total_Length;    // 4 Unsigned Integer 消息总长度(含消息头及消息体) 
        private SGIP_COMMAND _Command_Id; // 4 Unsigned Integer 命令或响应类型 
        private uint _SrcNodeSequence = SGIP12.Setting.SrcNodeSequence;  //4 Unsigned Integer 源节点的编号
        private uint _DateSequence = 0;     //4 Unsigned Integer 格式为十进制的mmddhhmmss
        private uint _Sequence_Id;    // 4 Unsigned Integer 消息流水号,顺序累加,步长为1,循环使用(一对请求和应答消息的流水号必须相同) 

        public SGIP_MESSAGE( uint Total_Length, SGIP_COMMAND Command_Id, uint Sequence_Id) 
        {
            this._Total_Length = Total_Length;
            this._Command_Id = Command_Id;
            this._SrcNodeSequence = SGIP12.Setting.SrcNodeSequence;
            this._DateSequence = uint.Parse(Util.Get_MMDDHHMMSS_String(DateTime.Now));
            this._Sequence_Id = Sequence_Id;
        }

        public SGIP_MESSAGE
         (
          uint Total_Length
          , SGIP_COMMAND Command_Id
          , uint SrcNodeSequence
          , uint DateSequence
          , uint Sequence_Id
         ) //发送前 
        {
            this._Total_Length = Total_Length;
            this._Command_Id = Command_Id;
            this._SrcNodeSequence = SrcNodeSequence;
            this._DateSequence = DateSequence;
            this._Sequence_Id = Sequence_Id;
        }

        public SGIP_MESSAGE(byte[] bytes)
        {
            int i = 0;
            byte[] buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._Total_Length = BitConverter.ToUInt32(buffer, 0);

            i += 4;
            Buffer.BlockCopy(bytes, 4, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._Command_Id = (SGIP_COMMAND)BitConverter.ToUInt32(buffer, 0);


            i += 4;
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._SrcNodeSequence = BitConverter.ToUInt32(buffer, 0);


            i += 4;
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._DateSequence = BitConverter.ToUInt32(buffer, 0);


            i += 4;
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._Sequence_Id = BitConverter.ToUInt32(buffer, 0);
        }


        public byte[] ToBytes()
        {
            int i = 0;
            byte[] bytes = new byte[SGIP_MESSAGE.Length];

            byte[] buffer = BitConverter.GetBytes(this._Total_Length);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, i, 4);

            i += 4;
            buffer = BitConverter.GetBytes((uint)this._Command_Id);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, i, 4);
            i += 4;


            buffer = BitConverter.GetBytes(this._SrcNodeSequence);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, i, 4);

            i += 4;
            buffer = BitConverter.GetBytes(this._DateSequence);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, i, 4);

            i += 4;
            buffer = BitConverter.GetBytes(this._Sequence_Id);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, i, 4);

            return bytes;
        }
        public override string ToString()
        {
            return string.Format("MessageHeader: tCommand_Id={0}    tSequence_Id={1}    tTotal_Length={2}"
              , this._Command_Id
              , this._Sequence_Id
              , this._Total_Length
             );

        }

    }

}
