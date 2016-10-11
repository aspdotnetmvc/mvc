using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.DTO
{
    /// <summary>
    /// 对Model 的又一次封装
    /// </summary>
    [Serializable]
    public class SMSDTO
    {
        public SMS.Model.SMSMessage Message { get; set; }
        public List<SMS.Model.SMSNumber> SMSNumbers { get; set; }
    }
}
