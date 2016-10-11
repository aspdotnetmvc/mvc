using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 产品明细接口
    /// </summary>
    public interface IProductDetail: IProductFunction
    {
        /// <summary>
        /// 抬头列表
        /// </summary>
        List<string> Headers { get; set; }
        /// <summary>
        /// 菜单列表
        /// </summary>
        List<string> Handles { get; set; }
    }
}
