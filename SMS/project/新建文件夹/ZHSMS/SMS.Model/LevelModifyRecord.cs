using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class LevelModifyRecord
    {
        /// <summary>
        /// 操作者
        /// </summary>
        public string AccountID { get; set; }
        /// <summary>
        /// 短信业务号
        /// </summary>
        public Guid SerialNumber { get; set; }
        /// <summary>
        /// 短信提交的时间
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// 调整内容
        /// </summary>
        public string ModifyContent { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string Content { get; set; }
    }
}
