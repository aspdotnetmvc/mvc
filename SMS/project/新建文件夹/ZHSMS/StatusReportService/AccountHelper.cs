using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatusReportService
{
    /// <summary>
    /// 用于短信返费
    /// </summary>
    public class AccountHelper
    {
        /// <summary>
        /// 短信返费
        /// </summary>
        /// <param name="smsid"></param>
        /// <param name="number"></param>
        public static void ReturnFee(string smsid, string number)
        {
            try
            {
                SMS.DB.SMSDAL.ReturnBalanceBySMS(smsid, number);
            }
            catch (Exception ex)
            {
                LogClient.LogHelper.LogError("StatusReport", "StatusReportHost.ReturnFee", ex.ToString());
                LogClient.LogHelper.LogError("StatusReport", "StatusReportHost.ReturnFee", ex.Message + "|" + smsid + "|" + number);
            }
        }
    }
}
