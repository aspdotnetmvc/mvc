using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 产品接口
    /// </summary>
    public interface ProductInterface
    {
        /// <summary>
        /// 得到产品的配置信息
        /// </summary>
        /// <returns></returns>
        IProductConfiguration GetProductConfiguration();
        /// <summary>
        /// 功能调用
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>HTML页面</returns>
        string CallProductFunction(FunctionParameter parameter);
        /// <summary>
        /// 调用产品功能
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        string CallProductFunction(string Url, FunctionParameter parameter);
        /// <summary>
        /// 销售产品
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        string SaleProduct(SaleParameter parameter);
        /// <summary>
        /// 得到融合套餐包
        /// </summary>
        /// <returns></returns>
        List<IntegrationPackages> GetIntegrationPackages();
        /// <summary>
        /// 销售产品（验证提单、提交订单）
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="isSubmit">true提交false验证</param>
        /// <returns></returns>
        string SaleCheckSubmit(string orderId, bool isSubmit);
    }
}
