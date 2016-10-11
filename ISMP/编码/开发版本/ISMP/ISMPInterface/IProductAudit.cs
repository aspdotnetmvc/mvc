using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    public interface IProductAudit:IProductFunction
    {
        /// <summary>
        /// 审核类型列表
        /// </summary>
        List<string> AuditsType { get; set; }
    }
}
