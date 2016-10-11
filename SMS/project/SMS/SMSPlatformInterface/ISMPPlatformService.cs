using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSPlatform
{
    public partial interface ISMSPlatformService
    {
        #region ISMP 专用 接口

        /// <summary>
        /// 添加一个企业
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        SMS.Model.RPCResult ISMPAddEnterprise(SMS.Model.EnterpriseUser user);
        /// <summary>
        /// 获取所有企业，包括待审核的 
        /// 用于检查spNumber 是否已被使用等
        /// </summary>
        /// <returns></returns>
        SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>> ISMPGetAllEnterprise();

        RPCResult<List<EnterpriseUser>> ISMPGetEnterpriseBySMSType(SMSType smstype);
        SMS.Model.RPCResult<List<SMS.Model.Keywords>> GetAllKeywords();

        /// <summary>
        /// 获取短信统计
        /// </summary>
        /// <param name="Account">按账号模糊查询</param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<List<string>> GetSMSStatisticsAll(DateTime BeginDate, DateTime EndDate);
        #endregion


        #region 企业黑名单  2016-1-31 新增功能

        SMS.Model.RPCResult AddEnterpriseBlackList(string EnterpriseCode, List<string> Numbers);
        SMS.Model.RPCResult DeleteEnterpriseBlackList(string EnterpriseCode, List<string> Numbers);

        SMS.Model.RPCResult<List<string>> GetEnterpriseBlackList(string EnterpriseCode);

        #endregion
 
        #region 上行短信处理
        /// <summary>
        /// 上行短信处理（自动把内容为T/N 的号码加入企业黑名单
        /// </summary>
        /// <param name="sms"></param>
        /// <returns></returns>
        SMS.Model.RPCResult ProcessMOSMS(string message);
        #endregion
    }
}
