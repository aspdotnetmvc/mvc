using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    public interface IProductPermission
    {
        /// <summary>
        /// 权限
        /// </summary>
        List<IPermission> Permissions { get; set; }
    }
}
