using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    public class WapPush
    {
        private static WapPush push = null;
        public static WapPush GetInstance()
        {
            if (null == push)
            {
                push = new WapPush();

            }
            return push;
        }


        //第一部分
        static private readonly byte[] WapPushHeader1 = new byte[] 
        {
            0x0B, 0x05, 0x04, 0x0B, 0x84, 0x23, 0xF0, 0x00, 0x03, 0x03, 0x01, 0x01
        };
        //第二部分
        static private readonly byte[] WapPushHeader2 = new byte[]
        {
            0x29, 0x06, 0x06, 0x03, 0xAE, 0x81, 0xEA, 0x8D, 0xCA
        };

        //第三部分
        static private readonly byte[] WapPushIndicator = new byte[] 
        {
            0x02, 0x05, 0x6A, 0x00, 0x45, 0xC6, 0x0C, 0x03
        };
        //第四部分:URL去掉http://后的UTF8编码
        //第五部分
        static private readonly byte[] WapPushDisplayTextHeader = new byte[] 
        { 
            0x00, 0x01, 0x03, 
        };
        //第六部分:消息文字的UTF8编码
        //第七部分:
        static private readonly byte[] EndOfWapPush = new byte[] 
        { 
            0x00, 0x01, 0x01, 
        };
        public byte[] toBytes(string WAP_Msg, string WAP_URL)
        {
            byte[] submitData = new byte[400];
            int index = 0;
            WapPushHeader1.CopyTo(submitData, index);
            index += WapPushHeader1.Length;

            WapPushHeader2.CopyTo(submitData, index);
            index += WapPushHeader2.Length;

            WapPushIndicator.CopyTo(submitData, index);
            index += WapPushIndicator.Length;

            byte[] url = Encoding.UTF8.GetBytes(WAP_URL);
            url.CopyTo(submitData, index);
            index += url.Length;

            WapPushDisplayTextHeader.CopyTo(submitData, index);
            index += WapPushDisplayTextHeader.Length;

            byte[] msg2 = Encoding.UTF8.GetBytes(WAP_Msg);
            msg2.CopyTo(submitData, index);
            index += msg2.Length;

            EndOfWapPush.CopyTo(submitData, index);
            index += 3;

            byte[] reVal = new byte[index];
            for (int i = 0; i < reVal.Length; i++)
            {
                reVal = submitData;
            }

            return (reVal);
        }
    }
}
