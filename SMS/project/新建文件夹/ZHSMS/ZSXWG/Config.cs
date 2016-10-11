using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZSXWG
{
    public class Config
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string ServerUrl { get; set; }
        public string UserId { get; set; }
        public string SrcID { get; set; }
        /// <summary>
        /// 签名位置 0 前 1 后
        /// </summary>
        public string SignaturePos { get; set; }
    }
}
