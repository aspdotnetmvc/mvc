using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    [Serializable]
    public class BasePermission
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public String Identifier { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnable { get; set; }

    }

    /// <summary>
    /// 按钮控件
    /// </summary>
    [Serializable]
    public class ButtonPermission : BasePermission
    {

    }
    /// <summary>
    /// 表格控件
    /// </summary>
    [Serializable]
    public class GridPermission : BasePermission
    {
        /// <summary>
        /// 隐藏的列
        /// </summary>
        public string[] HideColumns { get; set; }

    }

}
