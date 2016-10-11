using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayInterface
{
    /// <summary>
    /// 提交短信时返回值
    /// </summary>
    public class SendResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string SerialNumber { get; set; }
    }
}
