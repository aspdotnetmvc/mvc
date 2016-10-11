using System;

namespace ISMPInterface
{
    public interface IPermission
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        String Identifier { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        String Name { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        bool IsShow { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        bool IsEnable { get; set; }

    }

    /// <summary>
    /// 按钮控件
    /// </summary>
    public interface IButtonPermission: IPermission
    {

    }
    /// <summary>
    /// 表格控件
    /// </summary>
    public interface IGridPermission: IPermission
    {
        /// <summary>
        /// 隐藏的列
        /// </summary>
        string[] HideColumns { get; set; }

    }
}