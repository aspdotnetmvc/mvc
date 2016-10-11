using ISMPInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{

    [Serializable]
    public enum PermissionType
    {
        Normal=0,//默认
        Menu = 1,//菜单
        Page = 2,//页面
        Button = 3,//按钮
        Grid = 4,//表格
    }

    [Serializable]
    public enum PageOpenMode
    {
        No = 0,//无处理
        NewTab = 1,//弹出标签页
        NewPage = 2//弹出新页
    }

    [Serializable]
    public class Permission : IPermission
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// 权限上级
        /// </summary>
        public Permission Parent { get; set; }
        /// <summary>
        /// 是否有效（0否1是）
        /// </summary>
        public int IsValid { get; set; }

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

        public Permission()
        { }
        /// <summary>
        /// 初始化权限独特属性
        /// </summary>
        public Permission(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                IsShow = true;
                IsEnable = true;
            }
            else
            {
                JObject jobj = JObject.Parse(data);
                if (jobj["IsShow"] == null || jobj["IsShow"].ToString() == "")
                    IsShow = true;
                else
                    IsShow = (bool)jobj["IsShow"];
                if (jobj["IsEnable"] == null || jobj["IsEnable"].ToString() == "")
                    IsEnable = true;
                else
                    IsEnable = (bool)jobj["IsEnable"];
            }
        }
        /// <summary>
        /// 生成权限独特属性数据
        /// </summary>
        public virtual string GetPermissionData()
        {
            var data = new { IsShow = IsShow, IsEnable = IsEnable };
            return JsonConvert.SerializeObject(data);
        }
        /// <summary>
        /// 获取权限类型
        /// </summary>
        /// <returns></returns>
        public virtual PermissionType GetPermissionType()
        {
            return PermissionType.Normal;
        }

        public void SetGetPermissionData(string data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 菜单控件
    /// </summary>
    [Serializable]
    public class Permission_Menu : Permission
    {
        /// <summary>
        /// 权限图标
        /// </summary>
        public String Icon { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int OrderNumber { get; set; }
        /// <summary>
        /// 分类 System、Agent、Enterprise
        /// </summary>
        public String Groups { get; set; }
        /// <summary>
        /// 是否是子产品菜单
        /// </summary>
        public bool IsProduct { get; set; }
        /// <summary>
        /// 初始化权限独特属性
        /// </summary>
        public Permission_Menu(string data)
            : base(data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                Icon = "";
                OrderNumber = 0;
                IsProduct = false;
            }
            else
            {
                JObject jobj = JObject.Parse(data);
                if (jobj["Icon"] == null || jobj["Icon"].ToString() == "")
                    Icon = "";
                else
                    Icon = (String)jobj["Icon"];
                if (jobj["OrderNumber"] == null || jobj["OrderNumber"].ToString() == "")
                    OrderNumber = 0;
                else
                    OrderNumber = (int)jobj["OrderNumber"];
                if (jobj["Groups"] == null || jobj["Groups"].ToString() == "")
                    Groups = "";
                else
                    Groups = (String)jobj["Groups"];

                if (jobj["IsProduct"] == null || jobj["IsProduct"].ToString() == "")
                    IsProduct = false;
                else
                    IsProduct = (bool)jobj["IsProduct"];


            }
        }
        /// <summary>
        /// 生成权限独特属性数据
        /// </summary>
        public override string GetPermissionData()
        {
            var data = new { IsShow = IsShow, IsEnable = IsEnable, Icon = Icon, OrderNumber = OrderNumber, Groups = Groups,IsProduct=IsProduct };
            return JsonConvert.SerializeObject(data);
        }
        /// <summary>
        /// 获取权限类型
        /// </summary>
        /// <returns></returns>
        public override PermissionType GetPermissionType()
        {
            return PermissionType.Menu;
        }
    }
    /// <summary>
    /// 页面控件
    /// </summary>
    [Serializable]
    public class Permission_Page : Permission_Menu
    {
        /// <summary>
        /// 页面地址
        /// </summary>
        public string PageUrl { get; set; }
        /// <summary>
        /// 页面开启方式
        /// </summary>
        public PageOpenMode OpenMode { get; set; }
        /// <summary>
        /// 页面依托（系统无依托、经销商管理、企业管理）
        /// </summary>
        public FunctionRole RelyOn { get; set; }
        /// <summary>
        /// 初始化权限独特属性
        /// </summary>
        public Permission_Page(string data)
            : base(data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                PageUrl = "";
                OpenMode = PageOpenMode.No;
                RelyOn = FunctionRole.System;
            }
            else
            {
                JObject jobj = JObject.Parse(data);
                if (jobj["PageUrl"] == null || jobj["PageUrl"].ToString() == "")
                    PageUrl = "";
                else
                    PageUrl = (String)jobj["PageUrl"];

                if (jobj["OpenMode"] == null || jobj["OpenMode"].ToString() == "")
                    OpenMode = PageOpenMode.No;
                else
                    OpenMode = (PageOpenMode)((int)jobj["OpenMode"]);

                if (jobj["RelyOn"] == null || jobj["RelyOn"].ToString() == "")
                    RelyOn = FunctionRole.System;
                else
                    RelyOn = (FunctionRole)((int)jobj["RelyOn"]);
            }
        }
        /// <summary>
        /// 生成权限独特属性数据
        /// </summary>
        /// <returns></returns>
        public override string GetPermissionData()
        {
            var data = new { IsShow = IsShow, IsEnable = IsEnable, PageUrl = PageUrl, OpenMode = (int)OpenMode, RelyOn = (int)RelyOn, Icon = Icon, IsProduct = IsProduct, OrderNumber = OrderNumber };
            return JsonConvert.SerializeObject(data);
        }
        /// <summary>
        /// 获取权限类型
        /// </summary>
        /// <returns></returns>
        public override PermissionType GetPermissionType()
        {
            return PermissionType.Page;
        }
    }

    /// <summary>
    /// 按钮控件
    /// </summary>
    [Serializable]
    public class Permission_Button : Permission, IButtonPermission
    {
        public Permission_Button()
        { }

        public Permission_Button(string data)
            : base(data)
        { }

        /// <summary>
        /// 获取权限类型
        /// </summary>
        /// <returns></returns>
        public override PermissionType GetPermissionType()
        {
            return PermissionType.Button;
        }
    }
    /// <summary>
    /// 表格控件
    /// </summary>
    [Serializable]
    public class Permission_Grid : Permission, IGridPermission
    {
        public Permission_Grid()
        {

        }
        public Permission_Grid(string data)
            : base(data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                HideColumns = new string[] { };
            }
            else
            {
                JObject jobj = JObject.Parse(data);
                if (jobj["HideColumns"] == null || jobj["HideColumns"].ToString() == "")
                    HideColumns = new string[] { };
                else
                    HideColumns = ((String)jobj["HideColumns"]).Split(',').ToArray();
            }
        }
        /// <summary>
        /// 隐藏的列
        /// </summary>
        public string[] HideColumns { get; set; }

        /// <summary>
        /// 初始化权限独特属性
        /// </summary>

        /// <summary>
        /// 生成权限独特属性数据
        /// </summary>
        /// <returns></returns>
        public override string GetPermissionData()
        {
            var data = new { IsShow = IsShow, IsEnable = IsEnable, HideColumns = string.Join(",", HideColumns.ToArray()) };
            return JsonConvert.SerializeObject(data);
        }
        /// <summary>
        /// 获取权限类型
        /// </summary>
        /// <returns></returns>
        public override PermissionType GetPermissionType()
        {
            return PermissionType.Grid;
        }
    }
}
