using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 产品的订单接口
    /// </summary>
    public interface IProductOrder
    {
        /// <summary>
        /// 订单号
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        string EnterpriseId { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        string ProductId { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// 状态列表
        /// </summary>
        Dictionary<string, DisplayValue<string, string>> StatusList { get; set; }
    }
}
