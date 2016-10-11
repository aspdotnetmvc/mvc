using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    [Serializable]
    public enum PowerElementType
    {
        Menu = 0,//菜单
        Page = 1,//页面
        Button = 2,//按钮
        Grid = 2,//表格
    }
    [Serializable]
    public enum PageOpenMode
    {
        No = 0,//无处理
        NewTab = 1,//弹出标签页
        NewPage = 2//弹出新页
    }
    public class PowerElement
    {
        /// <summary>
        /// 元素ID
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary>
        public String ProductId { get; set; }
        /// <summary>
        /// 元素上级ID
        /// </summary>
        public String ParentId { get; set; }
        /// <summary>
        /// 元素代码
        /// </summary>
        public String ElementCode { get; set; }
        /// <summary>
        /// 元素名称
        /// </summary>
        public String ElementName { get; set; }
        /// <summary>
        /// 元素类型
        /// </summary>
        public PowerElementType ElementType { get; set; }
        /// <summary>
        /// 元素图标
        /// </summary>
        public String ElementIcon { get; set; }
        /// <summary>
        /// 元素数据
        /// </summary>
        public String ElementData { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int OrderNum { get; set; }
        /// <summary>
        /// 是否有效（0否1是）
        /// </summary>
        public int IsValid { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 初始化元素数据
        /// </summary>
        public virtual void InitData()
        {
            if (ElementData == "")
            {
                IsShow = true;
                IsEnable = true;
            }
            else
            {
                JObject jobj = JObject.Parse(ElementData);
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
        /// 生成元素数据
        /// </summary>
        public virtual void SetData()
        {
            var data = new { IsShow = IsShow, IsEnable = IsEnable };
            ElementData = JsonConvert.SerializeObject(data);
        }
    }
    /// <summary>
    /// 菜单控件
    /// </summary>
    [Serializable]
    public class PowerElement_Menu : PowerElement
    {

    }
    /// <summary>
    /// 页面控件
    /// </summary>
    [Serializable]
    public class PowerElement_Page : PowerElement
    {
        /// <summary>
        /// 页面地址
        /// </summary>
        public string PageUrl { get; set; }
        /// <summary>
        /// 页面开启方式
        /// </summary>
        public PageOpenMode OpenMode { get; set; }
        public override void InitData()
        {
            if (ElementData != "")
            {
                JObject jobj = JObject.Parse(ElementData);
                if (jobj["PageUrl"] == null || jobj["PageUrl"].ToString() == "")
                    PageUrl = "";
                else
                    PageUrl = (String)jobj["PageUrl"];

                if (jobj["OpenMode"] == null || jobj["OpenMode"].ToString() == "")
                    OpenMode = PageOpenMode.No;
                else
                    OpenMode = (PageOpenMode)((int)jobj["OpenMode"]);
            }
            base.InitData();
        }
        public override void SetData()
        {
            var data = new { 
                IsShow = IsShow, 
                IsEnable = IsEnable, 
                PageUrl=PageUrl,
                OpenMode=(int)OpenMode
            };
            ElementData = JsonConvert.SerializeObject(data);
        }
    }

    /// <summary>
    /// 按钮控件
    /// </summary>
    [Serializable]
    public class PowerElement_Button : PowerElement
    {

    }
    /// <summary>
    /// 表格控件
    /// </summary>
    [Serializable]
    public class PowerElement_Grid : PowerElement
    {
        /// <summary>
        /// 隐藏的列
        /// </summary>
        public List<string> HideColumn { get; set; }

        /// <summary>
        /// 初始化属性
        /// </summary>
        public override void InitData()
        {
            if (ElementData != "")
            {
                JObject jobj = JObject.Parse(ElementData);
                if (jobj["HideColumn"] == null || jobj["HideColumn"].ToString() == "")
                    HideColumn = new List<string>();
                else
                    HideColumn = ((String)jobj["HideColumn"]).Split(',').ToList();
            }
            base.InitData();
        }
        public override void SetData()
        {
            var data = new
            {
                IsShow = IsShow,
                IsEnable = IsEnable,
                HideColumn = string.Join(",", HideColumn.ToArray())
            };
            ElementData = JsonConvert.SerializeObject(data);
        }
    }
}
