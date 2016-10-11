using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class GatewayConfiguration
    {
        string _gateway;
        List<string> _operators;
    

        /// <summary>
        /// 网关的处理能力
        /// </summary>
        public int MinPackageSize
        {
            get;
            set;
        }

        public string Gateway
        {
            get { return _gateway; }
            set { _gateway = value; }
        }
        /// <summary>
        /// 单次提交最大号码数
        /// </summary>
        public int MaxPackageSize { get; set; }
        public List<string> Operators
        {
            get { return _operators; }
            set { _operators = value; }
        }
        public GatewayConfiguration()
        {
        }
    }
}
