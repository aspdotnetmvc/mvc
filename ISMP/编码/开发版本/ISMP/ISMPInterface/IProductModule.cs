using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    public interface IProductModule:IProductFunction,IProductPermission
    {
        /// <summary>
        /// 顺序
        /// </summary>
        int OrderNumber { get; set; }
    }
}
