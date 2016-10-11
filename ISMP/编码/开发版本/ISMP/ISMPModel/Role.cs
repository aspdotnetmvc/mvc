using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 角色
    /// </summary>
    /// 
    [Serializable]
    public class Role
    {
        /// <summary>
        /// 系统生成的ID
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 角色说明
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 是否系统角色，系统角色不允许操作（0否1是）
        /// </summary>
        public int IsSystem { get; set; }
        /// <summary>
        /// 角色拥有的权限
        /// </summary>
        public List<Permission> Permission { get; set; }

    }
}
