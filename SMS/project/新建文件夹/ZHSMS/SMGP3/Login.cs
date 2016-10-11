using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Login
    {
        public const int BodyLength = 8 + 16 + 1 + 4 + 1;

        private string _ClientID;               // 8 Octet String 客户端用来登录服务器端的用户账号。 
        private string _Password;
        private byte[] _AuthenticatorSource;    // 16 Octet String 用于鉴别源地址。其值通过单向MD5 hash计算得出,表示如下: 
        private uint _LoginMode;                // 1  Unsigned Integer  客户端用来登录服务器端的登录类型。
        //   AuthenticatorSource = 
        //   MD5(Source_Addr+9 字节的0 +shared secret+timestamp) 
        //   Shared secret 由中国移动与源地址实体事先商定,timestamp格式为:MMDDHHMMSS,即月日时分秒,10位。 
        private uint _ClientVersion;        // 1 Unsigned Integer 客户端支持的协议版本号
        private uint _Timestamp;       // 4 Unsigned Integer 时间戳的明文,由客户端产生,格式为MMDDHHMMSS,即月日时分秒,10位数字的整型,右对齐 。 

        private MessageHeader _Header;

        public MessageHeader Header
        {
            get
            {
                return this._Header;
            }
        }

        public byte[] AuthenticatorSource
        {
            get
            {
                return this._AuthenticatorSource;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClientID">客户端用来登录服务器端的用户账号</param>
        /// <param name="Password">登录密码</param>
        /// <param name="LoginMode">客户端用来登录服务器端的登录类型。0＝发送短消息（send mode）；1＝接收短消息（receive mode）；2＝收发短消息（transmit mode）；</param>
        /// <param name="Timestamp">时间戳</param>
        /// <param name="Version">客户端支持的协议版本号。</param>
        /// <param name="Sequence_Id"></param>
        public Login(string ClientID,string Password,uint LoginMode,DateTime Timestamp,uint ClientVersion,uint Sequence_Id)
        {
            this._Header = new MessageHeader(MessageHeader.Length + BodyLength, SMGP3_COMMAND.Login, Sequence_Id);

            this._ClientID = ClientID;
            this._Password = Password;
            this._LoginMode = LoginMode;

            string s = Util.Get_MMDDHHMMSS_String(Timestamp);
            this._Timestamp = UInt32.Parse(s);

            //byte[] buffer = new byte[8 + 7 + this._Password.Length + 10];
            //Encoding.ASCII.GetBytes(this._ClientID).CopyTo(buffer, 0);
            //Encoding.ASCII.GetBytes(this._Password).CopyTo(buffer, 8 + 7);
            //Encoding.ASCII.GetBytes(s).CopyTo(buffer, 8 + 7 + this._Password.Length);
            //this._AuthenticatorSource = new MD5CryptoServiceProvider().ComputeHash(buffer, 0, buffer.Length);

            byte[] clientid = System.Text.Encoding.ASCII.GetBytes(ClientID);
            byte[] password = System.Text.Encoding.ASCII.GetBytes(Password);
            byte[] timestamp = System.Text.Encoding.ASCII.GetBytes(s);
            byte[] data = new byte[clientid.Length + 7 + password.Length + timestamp.Length];
            Array.Copy(clientid, data, clientid.Length);
            Array.Copy(password, 0, data, clientid.Length + 7, password.Length);
            Array.Copy(timestamp, 0, data, clientid.Length + 7 + password.Length, timestamp.Length);
            System.Security.Cryptography.MD5 MD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            _AuthenticatorSource = MD5.ComputeHash(data);

            this._ClientVersion = ClientVersion;
        }

        public byte[] ToBytes()
        {
            int i = 0;
            byte[] bytes = new byte[MessageHeader.Length + BodyLength];

            //header 12 
            byte[] buffer = this._Header.ToBytes();
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);

            //ClientID 8 
            i += MessageHeader.Length;
            buffer = Encoding.ASCII.GetBytes(this._ClientID);
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);

            //AuthenticatorSource 16 
            i += 8;
            buffer = this._AuthenticatorSource;
            Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); //16 

            //LoginMode 1 
            i += 16;
            bytes[i++] = (byte)this._LoginMode; //登录类型

            //Timestamp 
            buffer = BitConverter.GetBytes(this._Timestamp);
            Array.Reverse(buffer);
            buffer.CopyTo(bytes, i);

            //version 1 
            i += 4;
            bytes[i] = (byte)this._ClientVersion; //客户端版本 
            return (bytes);
        }

        public override string ToString()
        {
            return string.Format("Header={0}    AuthenticatorSource={1}    Password={2}    ClientID={3}   LoginMode={4}    Timestamp={5}    ClientVersion={6}"
                , this._Header.ToString()
                , this._AuthenticatorSource
                , this._Password
                , this._ClientID
                , this._LoginMode
                , this._Timestamp
                , this._ClientVersion);
        }
    }
}
