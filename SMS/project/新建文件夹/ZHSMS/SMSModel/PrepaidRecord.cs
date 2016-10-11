using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    [Serializable]
    public class PrepaidRecord
    {
        /// <summary>
        /// 操作人
        /// </summary>
        public string AccountID { get; set; }
        /// <summary>
        /// 充值帐号
        /// </summary>
        public string PrepaidAccount { get; set; }
        /// <summary>
        /// 充值数
        /// </summary>
        public uint Quantity { get; set; }
        /// <summary>
        /// 充值时间
        /// </summary>
        public DateTime PrepaidTime { get; set; }
    }
}
