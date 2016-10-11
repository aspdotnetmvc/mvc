using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    /// <summary>
    /// 号段编码表
    /// </summary>
    [Serializable]
    public class NumSect:BaseModel
    {
        /// <summary>
        /// 号段
        /// </summary>
        public string NumberSect { get; set; }
        /// <summary>
        /// 运营商
        /// </summary>
        public OperatorType OperatorType { get; set; }
    }
}
