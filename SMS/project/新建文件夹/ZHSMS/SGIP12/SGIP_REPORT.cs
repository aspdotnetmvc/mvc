using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    public class SGIP_REPORT
    {
        public const int BodyLength = 12 + 1 + 21 + 1 + 1 + 8;
        private SGIP_MESSAGE _Header;

        private uint _SrcNodeSequence;  //4 Unsigned Integer 源节点的编号
        private uint _DateSequence;     //4 Unsigned Integer 格式为十进制的mmddhhmmss
        private uint _Sequence_Id;    // 4 Unsigned Integer 消息流水号,顺序累加,步长为1,循环使用(一对请求和应答消息的流水号必须相同) 

        private uint _ReportType;       //1 Unsigned Integer Report命令类型    0：对先前一条Submit命令的状态报告   1：对先前一条前转Deliver命令的状态报告
        private string _UserNumber;     //21 Octet String 接收短消息的手机号，手机号码前加“86”国别标志
        private uint _State;            //1 Unsigned Integer 该命令所涉及的短消息的当前执行状态 0：发送成功 1：等待发送 2：发送失败
        private uint _ErrorCode;        //1 Unsigned Integer 当State=2时为错误码值，否则为0
        private string _Reserve;        //8 Octet String保留，扩展用


        public SGIP_REPORT(byte[] bytes)
        {
            int i = 0;
            string s = null;

            byte[] buffer = new byte[SGIP_MESSAGE.Length];
            Buffer.BlockCopy(bytes, 0, buffer, 0, SGIP_MESSAGE.Length);
            this._Header = new SGIP_MESSAGE(buffer);

            //SrcNodeSequence 4
            i += SGIP_MESSAGE.Length;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._SrcNodeSequence = BitConverter.ToUInt32(buffer, 0);

            //DateSequence 4
            i += 4;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._DateSequence = BitConverter.ToUInt32(buffer, 0);


            //Sequence_Id 4
            i += 4;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._Sequence_Id = BitConverter.ToUInt32(buffer, 0);

            //ReportType 1
            i += 4;
            this._ReportType = (uint)bytes[i++];

            //UserNumber 21
            buffer = new byte[21];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            if (s != null && s.Length > 11 && s.Substring(0, 2) == "86")
                s = s.Substring(2, s.Length - 2);
            this._UserNumber = s;


            //State 1
            i += 21;
            this._State = (uint)bytes[i++];
            this._ErrorCode = (uint)bytes[i++];


            //Linkid 8 
            buffer = new byte[8];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._Reserve = s;
        }


        public SGIP_MESSAGE Header
        {
            get
            {
                return this._Header;
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
        /// 获取序列号
        /// </summary>
        public uint Sequence_Id
        {
            get { return this._Sequence_Id; }
        }



        /// <summary>
        /// Report命令类型
        /// </summary>
        public uint ReportType
        {
            get { return _ReportType; }
        }

        /// <summary>
        /// 接收短消息的手机号，手机号码前加“86”国别标志
        /// </summary>
        public string UserNumber
        {
            get { return _UserNumber; }
        }

        /// <summary>
        /// 该命令所涉及的短消息的当前执行状态 0：发送成功 1：等待发送 2：发送失败
        /// </summary>
        public uint State
        {
            get { return _State; }
        }

        /// <summary>
        /// 当State=2时为错误码值，否则为0
        /// </summary>
        public uint ErrorCode
        {
            get { return _ErrorCode; }
        }


        public string Reserve
        {
            get { return _Reserve; }
        }


        public override string ToString()
        {
            return string.Format
              (
               "[\r\nMessageBody:"
               + "\r\n\tBodyLength: {0}"
               + "\r\n\tSrcNodeSequence: {1}"
               + "\r\n\tDateSequence: {2}"
               + "\r\n\tSequence_Id: {3}"
               + "\r\n\tReportType: {4}"
               + "\r\n\tUserNumber: {5}"
               + "\r\n\tState {6}"
               + "\r\n\tErrorCode {7}"
               + "\r\n\tReserve {8}"
               + "\r\n]"
               , SGIP_REPORT.BodyLength
               , this._SrcNodeSequence
               , this._DateSequence
               , this._Sequence_Id
               , this._ReportType
               , this._UserNumber
               , this._State
               , this._ErrorCode
               , this._Reserve
              );
        }
    }
}
