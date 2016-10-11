using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpSMSService.Models
{
    public class SendSMSResult
    {
        // 接口调用结果
        public string Result { get; set; }
        // 调用发送接口成功后返回的短信标识
        public string MsgID { get; set; }
    }
}
