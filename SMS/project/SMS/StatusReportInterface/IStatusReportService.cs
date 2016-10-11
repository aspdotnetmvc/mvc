using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatusReportService
{
   public interface IStatusReportService
    {
    
        /// <summary>
        /// 获取用户上行短信(缓存获取，每次最多100条，获取后缓存删除)
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        RPCResult<List<MOSMS>> GetMOSmsByAccountID(string accountID);
        /// <summary>
        /// 客户状态报告明细(缓存获取，获取后缓存删除)
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        RPCResult<List<StatusReport>> GetCustomStatusReportBySMSID(string accountID, string SMSID);
        /// <summary>
        /// 客户状态报告明细（缓存获取，每次最多100条，获取后缓存删除）
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        RPCResult<List<StatusReport>> GetCustomStatusReportByAccount(string accountID);
  
    }
}
