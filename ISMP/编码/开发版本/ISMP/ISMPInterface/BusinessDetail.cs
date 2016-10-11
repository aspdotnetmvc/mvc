using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 业务明细
    /// </summary>
    [Serializable]
    public class BusinessDetail
    {
        /// <summary>
        /// 明细类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 业务明细
        /// </summary>
        public List<string> Content { get; set; }
        /// <summary>
        /// 生成日期
        /// </summary>
        public DateTime Datetime { get; set; }
    }
}
