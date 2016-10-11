using ISMPInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    [Serializable]
    public class ProductConfiguration: IProductConfiguration
    {
        public string ProductId { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        /// <returns></returns>
        public List<string> PaymentsType { get; set; }
        /// <summary>
        /// 功能项
        /// </summary>
        /// <returns></returns>
        public List<IProductModule> Modules { get; set; }
        /// <summary>
        /// 功能接口
        /// </summary>
        public List<IProductFunction> Functions { get; set; }
        /// <summary>
        /// 获取指定类型接口列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<IProductFunction> GetFunction(FunctionType type)
        {
            List<IProductFunction> fs = new List<IProductFunction>();
            foreach (IProductFunction f in Functions)
            {
                if (f.FunctionType == type)
                {
                    fs.Add(f);
                }
            }
            return fs;
        }
    }
}
