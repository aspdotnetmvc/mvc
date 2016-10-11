using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class Keywords
    {
        /// <summary>
        /// 敏感词词组
        /// </summary>
        public string KeyGroup { get; set; }
        /// <summary>
        /// 敏感词
        /// </summary>
        public string Words { get; set; }
        /// <summary>
        /// 敏感词类型
        /// </summary>
        public string KeywordsType { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 敏感词替换成其他词
        /// </summary>
        public string ReplaceKeywords { get; set; }
    }
}
