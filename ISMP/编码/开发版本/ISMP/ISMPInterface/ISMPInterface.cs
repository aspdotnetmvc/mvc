using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    public interface ISMPInterface
    {
        /// <summary>
        /// 账户支付
        /// </summary>
        /// <param name="account"></param>
        /// <param name="product"></param>
        /// <param name="paymentType"></param>
        /// <param name="money"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        string PaymentAccountDeduct(string account, string product, string paymentType, float money, string description);
        //产品开通接口
        bool CreateProductOrder(IProductOrder order);
        //审核接口
        //审核申请
        bool SubmitAudit(IAudit audit);
        //审核结果
        bool SubmitAuditResult(IAuditResult result);
    }
}
