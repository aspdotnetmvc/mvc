using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpSMSService.Models
{
    public class GetBalanceResult
    {
        // 调用接口返回的结果
        public string Result { get; set; }
        // 剩余短信条数
        public string SmsBalance { get; set; }
    }
}
