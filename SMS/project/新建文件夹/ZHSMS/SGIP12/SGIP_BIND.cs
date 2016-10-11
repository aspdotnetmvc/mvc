using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    public class SGIP_BIND       //  双向
    {
        public const int BodyLength = 1 + 16 + 16 + 8;

        private uint _LoginType;      // 1 Unsigned Integer 登录类型。
        private string _LoginName;      // 16 Octet String  服务器端给客户端分配的登录名
        private string _LoginPassword;  // 16 Octet String  服务器端和Login Name对应的密码
        private string _Reserve;        // 8 Octet String  保留，扩展用

        private SGIP_MESSAGE _Header;

        public SGIP_BIND(uint LoginType, string LoginName, string LoginPassword, uint Sequence_Id)
        {
            this._Header = new SGIP_MESSAGE(SGIP_MESSAGE.Length + BodyLength, SGIP_COMMAND.SGIP_BIND, Sequence_Id);

            this._LoginType = LoginType;
            this._LoginName = LoginName;
            this._LoginPassword = LoginPassword;
            this._Reserve = "";
        }

        //public SGIP_Bind
        // (
        //  uint LoginType
        //  , string LoginName
        //  , string LoginPassword
        //  , uint SrcNodeSequence
        //  , uint DateSequence
        //  , uint Sequence_Id
        // )
        //{
        //    this._Header = new SIGP_MESSAGE(SIGP_MESSAGE.Length + BodyLength, SIGP_COMMAND.SGIP_BIND,SrcNodeSequence,DateSequence, Sequence_Id);

        //    this._LoginType = LoginType;
        //    this._LoginName = LoginName;
        //    this._LoginPassword = LoginPassword;
        //    this._Reserve = "";
        //}

        public SGIP_BIND(byte[] bytes)
        {
            int i = 0;
            string s = null;
            byte[] buffer = new byte[SGIP_MESSAGE.Length];
            Buffer.BlockCopy(bytes, 0, buffer, 0, SGIP_MESSAGE.Length);
            this._Header = new SGIP_MESSAGE(buffer);

            //LoginType 1 
            i += SGIP_MESSAGE.Length;
            this._LoginType = (uint)bytes[i++];

            //LoginName 16
            buffer = new byte[16];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._LoginName = s;

            //LoginPassword 16
            i += 16;
            buffer = new byte[16];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._LoginPassword = s;

            //LoginPassword 16
            i += 16;
            buffer = new byte[8];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            s = Encoding.ASCII.GetString(buffer).Trim();
            if (s.IndexOf('\0') > 0)
                s = s.Substring(0, s.IndexOf('\0'));
            this._Reserve = s;
        }

        public byte[] ToBytes()
        {
            int i = 0;
            byte[] bytes = new byte[SGIP_MESSAGE.Length + BodyLength];

            //header 20
            byte[] buffer = this._Header.ToBytes();
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);

            //LoginType 1
            i += SGIP_MESSAGE.Length;
            bytes[i++] = (byte)this._LoginType;

            //LoginName 16 
            buffer = Encoding.ASCII.GetBytes(this._LoginName);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);

            //LoginPassword 16 
            i += 16;
            buffer = Encoding.ASCII.GetBytes(this._LoginPassword);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);

            //Reserve 8
            i += 16;
            buffer = Encoding.ASCII.GetBytes(this._Reserve);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
            return (bytes);
        }

        public override string ToString()
        {
            return string.Format("Header={0}    LoginType={1}    LoginName={2}    LoginPassword={3}    Reserve={4}"
                , this._Header.ToString()
                , this._LoginType
                , this._LoginName
                , this._LoginPassword
                , this._Reserve);
        }

        /// <summary>
        /// 获取 登录类型
        /// </summary>
        public uint LoginType
        {
            get { return _LoginType; }
        }

        /// <summary>
        /// 获取登陆用户名
        /// </summary>
        public string LoginName
        {
            get { return _LoginName; }
        }

        /// <summary>
        /// 获取登陆密码
        /// </summary>
        public string LoginPassword
        {
            get { return _LoginPassword; }
        }

        /// <summary>
        /// 获取保留信息
        /// </summary>
        public string Reserve
        {
            get { return _Reserve; }
        }

        public SGIP_MESSAGE Header
        {
            get
            {
                return this._Header;
            }
        }
    }
}
