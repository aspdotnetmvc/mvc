using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    [Serializable]
    public class UserBalance
    {
        /// <summary>
        /// 短信条数剩余数
        /// </summary>
        public int SmsBalance { get; set; }
        /// <summary>
        /// 彩信条数剩余数
        /// </summary>
        public int MmsBalance { get; set; }
    }
}
