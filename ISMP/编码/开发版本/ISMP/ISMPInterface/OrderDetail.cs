using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 订单明细
    /// </summary>
    [Serializable]
    public class OrderDetail
    {
        /// <summary>
        /// 业务ID
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 业务数据
        /// </summary>
        public BusinessDetail Detail { get; set; }
        /// <summary>
        /// 状态数据
        /// </summary>
        public List<KeyValuePair<string, string>> Status { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
    }
}
