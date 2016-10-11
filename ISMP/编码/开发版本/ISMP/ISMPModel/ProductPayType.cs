using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 支付类型（产品细化）
    /// </summary>
    [Serializable]
    public class ProductPayType
    {
        /// <summary>
        /// 类型ID
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        public String ProductId { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public String Name { get; set; }

    }
}