using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMS.Model;

namespace SMS.DB
{
    public partial class SMSDAL
    {
        /// <summary>
        /// 获取审核失败原因列表
        /// </summary>
        /// <returns></returns>
        public static List<AuditFailureReason> GetAuditFailureReasonList()
        {
            string sql = "select * from AuditFailureReason";
            return DBHelper.Instance.Query<AuditFailureReason>(sql);
        }
    }
}
