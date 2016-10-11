using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    public class Report
    {
        List<string> per = new List<string>();
        public Report(byte[] bytes)
        {
            int i = 3;
            byte[] buffer;
            buffer = new byte[10];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            _id = System.BitConverter.ToString(buffer).Replace("-", "");

            i += 11;
            buffer = new byte[bytes.Length - 15];
            Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
            string str = Encoding.GetEncoding("gb2312").GetString(buffer);

            string[] sper = str.Split(' ');
            foreach (string s in sper)
            {
                if (s.IndexOf(':') > 0)
                    per.Add(s);
            }
            per.AddRange(sper);
        }

        private string _id;
        /// <summary>
        /// 状态报告对应原短消息的MsgID 
        /// </summary>
        public string Id
        {
            get { return _id; }
        }
        /// <summary>
        /// 取缺省值001
        /// </summary>
        public string Sub
        {
            get { return per[0].Split(':')[1]; }
        }
        /// <summary>
        /// 取缺省值001
        /// </summary>
        public string Dlvrd
        {
            get { return per[1].Split(':')[1]; }
        }

        /// <summary>
        /// 短消息提交时间（格式：年年月月日日时时分分，例如010331200000）
        /// </summary>
        public string Submit_date
        {
            get { return per[2].Split(':')[1]; }
        }


        /// <summary>
        /// 短消息下发时间（格式：年年月月日日时时分分，例如010331200000）
        /// </summary>
        public string Done_date
        {
            get { return per[3].Split(':')[1]; }
        }
        /// <summary>
        /// 短消息的最终状态
        /// </summary>
        public string Stat
        {
            get { return per[4].Split(':')[1]; }
        }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string Err
        {
            get { return per[5].Split(':')[1]; }
        }
        /// <summary>
        /// 前3个字节，表示短消息长度（用ASCII码表示），后17个字节表示短消息的内容（保证内容不出现乱码）
        /// </summary>
        public string Txt
        {
            get { return per[6].Split(':')[1]; }
        }
    }
}
