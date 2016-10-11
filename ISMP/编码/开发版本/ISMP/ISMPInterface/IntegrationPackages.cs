using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 套餐包
    /// </summary>
    [Serializable]
    public class IntegrationPackages
    {
        /// <summary>
        /// 套餐Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否页面处理
        /// </summary>
        public int IsPageHand { get; set; }
        /// <summary>
        /// 套餐成本
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }
    }
}
