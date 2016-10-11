using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    [Serializable]
    public class NumOperators
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 运营商代码
        /// </summary>
        public string Operators { get; set; }
        /// <summary>
        /// 号段
        /// </summary>
        public string NumberSect { get; set; }
    }
}
