using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 系统消息（通知，发给单个人的）
    /// </summary>
    [Serializable]
    public class SystemMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 详情
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// 接收人
        /// </summary>
        public String RecieveAccountId { get; set; }

        /// <summary>
        /// 接收人
        /// </summary>
        public String RecieveName { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        public String SenderName { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        public String SenderAccountId { get; set; }

        /// <summary>
        /// 发送日期
        /// </summary>
        public DateTime? SendTime { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// 阅读日期
        /// </summary>
        public DateTime? ReadTime { get; set; }

        /// <summary>
        /// 是否重要消息
        /// </summary>
        public bool IsImportmant { get; set; }


        public string ProjectId { get; set; }
        public string TableName { get; set; }
        public string RowId { get; set; }
        public string MessageType { get; set; }

    }
}

