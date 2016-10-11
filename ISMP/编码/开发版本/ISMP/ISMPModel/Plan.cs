using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 计划
    /// </summary>
    [Serializable]
    public class Plan
    {
        /// <summary>
        /// 
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 任务接收人账号Id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public String PlanTypeId { get; set; }

        /// <summary>
        /// 周期类型(年 1，季度2，月度3，周4，其他5）
        /// </summary>
        public CycleType CycleType { get; set; }

        /// <summary>
        /// 任务金额(单位：万元）
        /// </summary>
        public Decimal Amount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 完成金额(单位：万元）
        /// </summary>
        public Decimal Complete { get; set; }

        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime? RepTime { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 周期类型(年 1，季度2，月度3，周4，其他5）
    /// </summary>
    public enum CycleType
    {
        Year=1,
        Quarter=2,
        Month=3,
        Week=4,
        Other=5
    }
}
