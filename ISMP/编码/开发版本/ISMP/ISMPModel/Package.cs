using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    [Serializable]
    public class Package
    {
        /// <summary>
        /// 融合套餐ID
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// 融合套餐名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 融合套餐描述
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 融合套餐成本
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 原子套餐成本
        /// </summary>
        public decimal OldCost { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public String CreateAccount { get; set; }
        /// <summary>
        /// 子套餐
        /// </summary>
        public List<Package_Sub> Packages { get; set; }
    }
    [Serializable]
    public class Package_Sub
    {
        /// <summary>
        /// 子套餐ID
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// 子套餐名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 子套餐描述
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 子套餐成本
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 原子套餐成本
        /// </summary>
        public decimal OldCost { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        public String ProductId { get; set; }
        /// <summary>
        /// 是否主套餐
        /// </summary>
        public int IsMain { get; set; }
        /// <summary>
        /// 是否需要页面处理
        /// </summary>
        public int IsPageHand { get; set; }
        /// <summary>
        /// 终止模式
        /// </summary>
        public String Terminate { get; set; }
    }
}
