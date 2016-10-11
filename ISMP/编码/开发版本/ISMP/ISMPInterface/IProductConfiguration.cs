using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 产品配置
    /// </summary>
    public interface IProductConfiguration
    {
        string ProductId { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        /// <returns></returns>
        List<string> PaymentsType{ get; set; }
        /// <summary>
        /// 功能模块
        /// </summary>
        /// <returns></returns>
        List<IProductModule> Modules { get; set; }
        /// <summary>
        /// 功能接口
        /// </summary>
        List<IProductFunction> Functions { get; set; }
        /// <summary>
        /// 获取指定类型接口
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        List<IProductFunction> GetFunction(FunctionType type);
    }
}
