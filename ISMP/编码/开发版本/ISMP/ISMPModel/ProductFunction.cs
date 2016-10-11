using ISMPInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 产品配置接口
    /// </summary>
    [Serializable]
    public class ProductFunction : IProductFunction
    {
        /// <summary>
        /// 接口标示
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 接口名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 接口Url
        /// </summary>
        public string Url { get; set; }
        public FunctionType FunctionType { get; set; }
        public FunctionRole FunctionRole { get; set; }

        public virtual void SetData(string data)
        { }

        public virtual string GetData()
        {
            return "";
        }
    }
    /// <summary>
    /// 产品配置接口-审核
    /// </summary>
    [Serializable]
    public class ProductAudit : ProductFunction, IProductAudit
    {
        /// <summary>
        /// 审核类型列表
        /// </summary>
        public List<string> AuditsType
        {
            get;
            set;
        }

        public override void SetData(string data)
        {
            try
            {
                AuditsType = new List<string>(data.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
            }
            catch (Exception ex)
            { }
        }
        public override string GetData()
        {
            if (AuditsType != null && AuditsType.Count > 0)
                return string.Join(",", AuditsType.ToArray());
            else
                return "";
        }
    }
    /// <summary>
    /// 产品配置接口-明细
    /// </summary>
    [Serializable]
    public class ProductDetail : ProductFunction,IProductDetail
    {
        /// <summary>
        /// 表头
        /// </summary>
        public List<string> Headers
        {
            get;
            set;
        }
        /// <summary>
        /// 操作
        /// </summary>
        public List<string> Handles
        {
            get;
            set;
        }
        public override void SetData(string data)
        {
            string titles = "";
            string menus = "";
            try
            {
                JObject jobj = JObject.Parse(data);
                if (jobj["Titles"] != null && jobj["Titles"].ToString() != "")
                    titles = (String)jobj["Titles"];
                if (jobj["Menus"] != null && jobj["Menus"].ToString() != "")
                    menus = (String)jobj["Menus"];

                Headers = new List<string>(titles.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
                Handles = new List<string>(menus.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
            }
            catch (Exception ex)
            { }
        }
        public override string GetData()
        {
            string titles = string.Join(",", Headers.ToArray());
            string menus = string.Join(",", Handles.ToArray());
            var datas = new { Titles = titles, Menus = menus };
            return JsonConvert.SerializeObject(datas);
        }
    }
}
