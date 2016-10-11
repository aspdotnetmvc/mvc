using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 系统公告
    /// </summary>
    [Serializable]
    public class SystemNotice
    {
        /// <summary>
        /// Id
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 公告标题
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PublishTime { get; set; }

        /// <summary>
        /// 公告内容
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// 发布人
        /// </summary>
        public String Author { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        public Int16 IsPublish { get; set; }

        /// <summary>
        /// 是否过期
        /// </summary>
        public Int16 IsExpired { get; set; }

        /// <summary>
        /// 公告接收类型 （0 全体，1中呼，2一级经销商，3，全体经销商，4，企业，5,自定义）
        /// </summary>
        public NoticeRecieveType NoticeRecieveType { get; set; }

    }
    /// <summary>
    /// 公告接收类型 （0 全体，1中呼，2一级经销商，3，全体经销商，4，企业，5,自定义）
    /// </summary>
    public enum NoticeRecieveType
    {
        ALL = 0,
        ZH = 1,
        Agent_FirstLevel = 2,
        Agent_ALL = 3,
        Enterprise = 4,
        ByAccount = 5

    }

}

