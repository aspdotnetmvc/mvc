using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 部门表
    /// </summary>
    [Serializable]
    public class Department
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        public int OrderNum { get; set; }
        /// <summary>
        /// 上级部门Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 部门层级
        /// </summary>
        public int Level { get; set; }
    }
}