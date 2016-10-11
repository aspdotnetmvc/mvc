using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    /// <summary>
    /// 权限
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string MenuID { get; set; }
        /// <summary>
        /// 权限操作
        /// </summary>
        public List<PermissionAction> Actions { get; set; }
    }
    public class PermissionAction
    {
        /// <summary>
        /// 权限编号
        /// </summary>
        public string PermissionID { get; set; }
        /// <summary>
        /// 权限码
        /// </summary>
        public string PermissionCode { get; set; }
        /// <summary>
        /// 权限标题
        /// </summary>
        public string PermissionTitle { get; set; }
        /// <summary>
        /// 权限码操作类型
        /// </summary>
        public PermissionOperateType OperateType { get; set; }
        /// <summary>
        /// 权限码操作的内容
        /// </summary>
        public string Content { get; set; }
    }

    public enum PermissionOperateType : short
    {
        Visible = 0,
        DisVisible = 1,
        Check = 2,
        Add = 3,
    }
}