using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 任务类别（产品）
    /// </summary>
    [Serializable]
    public class PlanType
    {
        public string Id { get; set; }

        /// <summary>
        /// 对应的产品
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 任务类别名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        public int OrderNum { get; set; }
    }
}
