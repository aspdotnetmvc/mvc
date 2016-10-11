using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 待处理事项
    /// </summary>
    [Serializable]
    public class SystemToDoList
    {
        /// <summary>
        /// 
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 任务所有者
        /// </summary>
        public String AccountId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 处理URL
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// 处理页面标题
        /// </summary>
        public String PageTitle { get; set; }
        /// <summary>
        /// 处理页面Id
        /// </summary>
        public String PageId { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? DealTime { get; set; }

        /// <summary>
        /// 是否已处理
        /// </summary>
        public bool IsDealed { get; set; }

        public string ProjectId { get; set; }
        public string TableName { get; set; }
        public string RowId { get; set; }
        public string ToDoType { get; set; }

    }
}

