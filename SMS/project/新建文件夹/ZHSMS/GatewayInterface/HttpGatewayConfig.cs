using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayInterface
{
    [Serializable]
    public class HttpGatewayConfig
    {
        /// <summary>
        /// 网关名
        /// </summary>
        public string GatewayName { get; set; }
        /// <summary>
        /// 网关类名
        /// </summary>
        public string GatewayClass { get; set; }
        public string Account { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        //接入号
        public string SrcId { get; set; }
        public string SendUrl { get; set; }
        public string StatusReportUrl { get; set; }
        public string MOUrl { get; set; }
        public string BalanceUrl { get; set; }
        /// <summary>
        /// 获取Mo 的时间间隔  ms
        /// </summary>
        public int MOInterval { get; set; }
        /// <summary>
        /// 获取状态报告的时间间隔 ms
        /// </summary>
        public int StatusReportInterval { get; set; }
        /// <summary>
        /// 签名位置 0，在前，1 在后 
        /// </summary>
        public int SignaturePos { get; set; }
        /// <summary>
        /// 是否添加扩展子号0 否，1 是
        /// </summary>
        public int ExtendNo { get; set; }
    }
}
