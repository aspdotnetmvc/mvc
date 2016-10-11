using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 首页显示需要的子产品统计数据
    /// </summary>
    [Serializable]
    public class HomePageStatistics
    {
        /// <summary>
        /// 对应子产品Id
        /// </summary>
        public string productId { get; set; }
        /// <summary>
        /// 对应子产品名称
        /// </summary>
        public string productName { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 总剩余量
        /// </summary>
        public decimal TotalRemain { get; set; }

        /// <summary>
        /// 结转总量
        /// </summary>
        public decimal LastTotal { get; set; }

        /// <summary>
        /// 结转用量
        /// </summary>
        public decimal LastUsage { get; set; }

        /// <summary>
        /// 结转余量
        /// </summary>
        public decimal LastRemain { get; set; }

        /// <summary>
        /// 本期总量
        /// </summary>
        public decimal CurrentTotal { get; set; }

        /// <summary>
        /// 本期用量
        /// </summary>
        public decimal CurrentUsage { get; set; }

        /// <summary>
        /// 本期余量
        /// </summary>
        public decimal CurrentRemain { get; set; }

    }

    /// <summary>
    /// 经销商及员工首页显示需要的子产品统计数据
    /// </summary>
    [Serializable]
    public class HomePageStatistics_Agent
    {
        /// <summary>
        /// 应续费数（余量少于或等于最后一期充值量的10%）
        /// </summary>
        public int NeedRechargeCount { get; set; }
        /// <summary>
        /// 新开数
        /// </summary>
        public int NewOpenCount { get; set; }
    }
}