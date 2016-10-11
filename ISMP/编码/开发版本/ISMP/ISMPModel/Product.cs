using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    [Serializable]
    public class Product
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 产品接口地址
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// 产品描述
        /// </summary>
        public String Description { get; set; }
    }
}
