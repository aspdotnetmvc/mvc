using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{

    /// <summary>
    /// 订单余额清零
    /// </summary>
   [Serializable]
    public class OrderBalancesToZeroParameter : FunctionParameter
    {
        /// <summary>
        /// 指令数据
        /// </summary>
        public string Data { get; set; }
        public string OrderId { get; set; }
    }
    /// <summary>
    /// 订单停用
    /// </summary>
    [Serializable]
    public class OrderDisabledParameter : FunctionParameter
    {
        /// <summary>
        /// 指令数据
        /// </summary>
        public string Data { get; set; }
        public string OrderId { get; set; }
    }
}
