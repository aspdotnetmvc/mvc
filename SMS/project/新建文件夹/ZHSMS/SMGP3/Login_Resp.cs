using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Login_Resp
    {
        private MessageHeader _Header;
        public const int BodyLength = 4 + 16 + 1;

        private uint _Status;    // 4 Unsigned Integer 状态 
        //   0:正确 
        //   1:消息结构错 
        //   2:非法源地址 
        //   3:认证错 
        //   4:版本太高 
        //   5~:其他错误 
        private byte[] _AuthenticatorISMG; // 16 Octet String ISMG认证码,用于鉴别ISMG。 
        //   其值通过单向MD5 hash计算得出,表示如下: 
        //   AuthenticatorISMG =MD5(Status+AuthenticatorSource+shared secret),Shared secret 由中国移动与源地址实体事先商定,AuthenticatorSource为源地址实体发送给ISMG的对应消息CMPP_Connect中的值。 
        //    认证出错时,此项为空。 
        private uint _ServerVersion;    // 1 Unsigned Integer 服务器端支持的最高版本号

        public byte[] AuthenticatorISMG
        {
            get
            {
                return this._AuthenticatorISMG;
            }
        }

        /// <summary>
        /// 请求返回结果
        /// </summary>
        public uint Status
        {
            get
            {
                return this._Status;
            }
        }

        /// <summary>
        /// 服务器版本号
        /// </summary>
        public uint ServerVersion
        {
            get
            {
                return this._ServerVersion;
            }
        }

        public MessageHeader Header
        {
            get
            {
                return this._Header;
            }
        }

        public Login_Resp(byte[] bytes)
        {
            //header 12 
            int i = 0;
            byte[] buffer = new byte[MessageHeader.Length];
            Buffer.BlockCopy(bytes, 0, buffer, 0, buffer.Length);
            this._Header = new MessageHeader(buffer);

            //status 4 
            i += MessageHeader.Length;
            buffer = new byte[4];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            this._Status = BitConverter.ToUInt32(buffer, 0);

            //AuthenticatorISMG 16 
            i += 4;
            this._AuthenticatorISMG = new byte[16];
            Buffer.BlockCopy(bytes, MessageHeader.Length + 4, this._AuthenticatorISMG, 0, this._AuthenticatorISMG.Length);

            //version  1
            i += 16;
            this._ServerVersion = bytes[i];
        }

        public override string ToString()
        {
            return string.Format("Header={0}    AuthenticatorISMG={1}    BodyLength={2}    Status={3}    ServerVersion={4}"
                , this._Header.ToString()
                , this._AuthenticatorISMG
                , Login_Resp.BodyLength
                , this._Status
                , this._ServerVersion);
        }
    }
}
