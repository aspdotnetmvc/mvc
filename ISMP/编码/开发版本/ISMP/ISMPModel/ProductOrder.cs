using ISMPInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 终止模式
    /// </summary>
    public enum TerminateType
    {
        /// <summary>
        /// 独立的
        /// </summary>
        Independent,
        /// <summary>
        /// 余额清零
        /// </summary>
        BalancesToZero,
        /// <summary>
        /// 停用
        /// </summary>
        Disabled

    }
    /// <summary>
    /// 产品订单
    /// </summary>
    [Serializable]
    public class ProductOrder
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 企业AccountId
        /// </summary>
        public string EnterpriseAccountId { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 套餐Id
        /// </summary>
        public string PackageId { get; set; }
        /// <summary>
        /// 子订单
        /// </summary>
        public List<SubProductOrder> SubOrder { get; set; }
        /// <summary>
        /// 开通时间
        /// </summary>
        public DateTime ActivateTime { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 状态列表
        /// </summary>
        public Dictionary<string, DisplayValue<string, string>> StatusList { get; set; }

        public string getStatusList()
        {
            if (StatusList == null)
                return "";
            return JsonConvert.SerializeObject(StatusList);
        }
        public void setStatusList(string status)
        {
            try
            {
                StatusList = JsonConvert.DeserializeObject<Dictionary<string, DisplayValue<string, string>>>(status);
            }
            catch (Exception e) { }
        }
    }
    [Serializable]
    public class SubProductOrder : ProductOrder
    {
        /// <summary>
        /// 父订单Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 主订单
        /// </summary>
        public bool IsMain { get; set; }
        /// <summary>
        /// 终止模式
        /// </summary>
        public TerminateType Terminate { get; set; }
    }
}
