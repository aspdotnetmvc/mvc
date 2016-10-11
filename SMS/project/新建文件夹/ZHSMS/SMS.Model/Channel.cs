using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class Channel
    {
        /// <summary>
        /// 通道编号
        /// </summary>
        public string ChannelID { get; set; }
        /// <summary>
        /// 通道名字
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 通道发送短信的类型 
        /// </summary>
        public SMSType SMSType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
