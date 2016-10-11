using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 企业产品
    /// </summary>
    [Serializable]
    public class EnterpriseProduct
    {
        /// <summary>
        /// 企业产品ID
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// 企业Info
        /// </summary>
        public Enterprise Enterprise { get; set; }

        /// <summary>
        /// 产品Info
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// 开通时间
        /// </summary>
        public DateTime OpenTime { get; set; }
    }
}
