
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSPlatform
{
    public partial interface ISMSPlatformService 
    {
        #region 外部使用接口

        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="smsContent"></param>
        /// <param name="numbers"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<SMS.DTO.SMSDTO> SendSMS(string account, string password, string smsContent, List<string> numbers,string Source);
        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="smsContent"></param>
        /// <param name="numbers"></param>
        /// <param name="spNumber"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<SMS.DTO.SMSDTO> SendSMS(string account, string password, string smsContent, List<string> numbers, string spNumber,string Source);
        /// <summary>
        /// 获取用户上行短信
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<List<SMS.Model.MOSMS>> GetSMS(string account, string pass);
        /// <summary>
        /// 根据业务号获取短信状态明细
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="msgID"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<List<SMS.Model.StatusReport>> GetReport(string account, string password, string msgID="");
      
        #endregion

        #region 直客端接口
        /// <summary>
        /// 修改企业信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        SMS.Model.RPCResult UpdateEnterprise(SMS.Model.EnterpriseUser user);
        /// <summary>
        /// 修改企业短信设置
        /// </summary>
        /// <param name="sms"></param>
        /// <returns></returns>
        SMS.Model.RPCResult UpdateEnterpriseSMS(EnterpriseUser user);
        /// <summary>
        /// 企业修改密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="oldPass"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        SMS.Model.RPCResult ChangeEnterprisePass(string account, string oldPass, string newPass);
     
      
        /// <summary>
        /// 获取帐号获取状态报告
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        RPCResult<List<ReportStatistics>> GetStatisticsReportByAccount(string accountID, DateTime beginTime, DateTime endTime);


        /// <summary>
        /// 获取帐号获取状态报告
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        RPCResult<QueryResult<StatusReport>> GetStatusReportBySMSID(QueryParams qp);
        #endregion

        #region 管理平台
        /// <summary>
        /// 添加企业
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        SMS.Model.RPCResult AddEnterprise(SMS.Model.EnterpriseUser user);
       
        /// <summary>
        /// 删除企业
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        SMS.Model.RPCResult DelEnterprise(string account);
        
        SMS.Model.RPCResult UpdateAccountSetting(SMS.Model.EnterpriseUser user);
        /// <summary>
        /// 更新企业资料设置
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        SMS.Model.RPCResult UpdateAccontInfo(SMS.Model.EnterpriseUser user);
        /// <summary>
        /// 统计短信总营收(代理商下面的终端用户不在统计之列)
        /// </summary>
        /// <returns></returns>
        SMS.Model.RPCResult<List<SMS.Model.ChargeStatics>> GetChargeStatics(string account);
        /// <summary>
        /// 企业帐号充值
        /// </summary>
        /// <param name="chargeRecord"></param>
        /// <returns></returns>
        SMS.Model.RPCResult AccountPrepaid(SMS.Model.ChargeRecord chargeRecord);
        /// <summary>
        /// 根据企业获取短信统计报告
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        RPCResult<List<ReportStatistics>> GetStatisticsReportAllByAccount(string account, DateTime beginTime, DateTime endTime);

     
        /// <summary>
        /// 根据企业获取MO短信
        /// </summary>
        /// <param name="spNumber"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        RPCResult<List<MOSMS>> GetMOSMS(string account, DateTime beginTime, DateTime endTime);

        #endregion

        #region 公共接口
             /// <summary>
        /// 获取用户余额
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pass">明文密码</param>
        /// <returns></returns>
        SMS.Model.RPCResult<SMS.Model.UserBalance> GetBalanceByPlainPass(string account, string pass);

        /// <summary>
        /// 获取用户余额
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pass">密文密码</param>
        /// <returns></returns>
        SMS.Model.RPCResult<SMS.Model.UserBalance> GetBalance(string account, string pass);
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        SMS.Model.RPCResult ResetEnterprisePass(string accountCode, string password);
        /// <summary>
        /// 获取企业的充值记录
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<List<SMS.Model.ChargeRecord>> GetEnterpriseChargeRecord(string accountCode, DateTime beginTime, DateTime endTime);
        ///// <summary>
        ///// 获取代理商终端企业
        ///// </summary>
        ///// <param name="account"></param>
        ///// <returns></returns>
        //RPCResult<List<Model.EnterpriseUser>> GetLowerEnterprises(string account);
        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        RPCResult<SMS.Model.EnterpriseUser> GetEnterprise(string accountCode);
        ///// <summary>
        ///// 确认删除审核失败记录
        ///// </summary>
        ///// <param name="accountID"></param>
        ///// <returns></returns>
        //RPCResult AffirmAuditFailureSMS(Guid serialNumber);
        #endregion

        #region 短信平台接口


        #region 黑名单
        /// <summary>
        /// 添加黑名单
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        RPCResult AddBlacklist(List<string> numbers);
        /// <summary>
        /// 删除黑名单
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        RPCResult DelBlacklist(List<string> numbers);
        /// <summary>
        /// 获取黑名单
        /// </summary>
        /// <returns></returns>
        RPCResult<List<string>> GetBlacklist();
        #endregion

        #region 关键词
        /// <summary>
        /// 添加敏感词组
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        RPCResult AddKeywordsGroup(string keyGroup, string remark);
        /// <summary>
        /// 添加敏感词类别
        /// </summary>
        /// <param name="type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        RPCResult AddKeywordsType(string type, string remark);

        /// <summary>
        /// 获取敏感词类别
        /// </summary>
        /// <returns></returns>
        RPCResult<Dictionary<string,string>> GetKeywordsTypes();
        /// <summary>
        /// 获取敏感词组
        /// </summary>
        /// <returns></returns>
        RPCResult<Dictionary<string, string>> GetKeyGroups();

        /// <summary>
        /// 添加关键词
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        RPCResult AddKeywords(string keyGroup, List<Keywords> keywords);
        /// <summary>
        /// 删除关键词
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        RPCResult DelKeywords(string keyGroup, List<string> keywords);
        /// <summary>
        /// 删除敏感词组
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        RPCResult DelKeywordsGroup(string keyGroup);
        /// <summary>
        /// 根据词组获取关键词
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        RPCResult<List<Keywords>> GetKeywords(string keyGroup);
        /// <summary>
        /// 根据敏感词类型获取敏感词
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        RPCResult<List<Keywords>> GetKeywordsByType(string type);
        /// <summary>
        /// 根据敏感词获取敏感词（模糊查询）
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        RPCResult<List<Keywords>> GetKeywordsByKeyword(string keyword);
        /// <summary>
        /// 敏感词启用与否
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        RPCResult KeywordsEnabled(string keyGroup, string keywords, bool enabled);

        /// <summary>
        /// 添加关键词组与网关绑定信息
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="gateways"></param>
        /// <returns></returns>
        RPCResult AddkeyGroupGatewayBind(string keyGroup, string gateway);
        /// <summary>
        /// 获取网关指定的关键词组
        /// </summary>
        /// <param name="keywordGroup"></param>
        /// <returns></returns>
        RPCResult<string> GetKeyGroupGatewayBinds(string gateway);
        #endregion

        #region 网关配置
        /// <summary>
        /// 添加网关
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        RPCResult AddGatewayConfig(GatewayConfiguration config);
        /// <summary>
        /// 修改网关
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        RPCResult UpdateGatewayConfig(GatewayConfiguration config);
        /// <summary>
        /// 获取网关配置
        /// </summary>
        /// <returns></returns>
        RPCResult<GatewayConfiguration> GetGatewayConfig(string gateway);
        /// <summary>
        /// 获取网关配置
        /// </summary>
        /// <returns></returns>
        RPCResult<List<GatewayConfiguration>> GetGatewayConfigs();
        /// <summary>
        /// 删除网关
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        RPCResult DelGatewayConfig(string gateway);
        #endregion

        #region 短信审核调整记录
        ///// <summary>
        ///// 短信审核的记录
        ///// </summary>
        ///// <param name="beginTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //RPCResult<List<AuditRecord>> GetAuditRecords(DateTime beginTime, DateTime endTime);
        ///// <summary>
        ///// 短信级别调整记录
        ///// </summary>
        ///// <param name="beginTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //RPCResult<List<LevelModifyRecord>> GetLevelModifyRecords(DateTime beginTime, DateTime endTime);
        #endregion

        #region 通道配置
        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        RPCResult AddChannel(Channel channel);
        /// <summary>
        /// 更新通道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        RPCResult UpdateChannel(Channel channel);
        /// <summary>
        /// 获取所有通道
        /// </summary>
        /// <returns></returns>
        RPCResult<List<Channel>> GetChannels();
        /// <summary>
        /// 获取通道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        RPCResult<Channel> GetSMSChannel(string channel);
        /// <summary>
        /// 删除通道
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        RPCResult DelChannel(string channel);
        /// <summary>
        /// 添加通道绑定的网关
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="gateways"></param>
        /// <returns></returns>
        RPCResult AddChannelGatewayBind(string channel, List<string> gateways);
        /// <summary>
        /// 根据通道获取绑定的网关
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        RPCResult<List<string>> GetGatewaysByChannel(string channel);
        #endregion

        #endregion

        #region 短信模板

        /// <summary>
        /// 提交短信模板内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<Guid> AddSMSTemplet(string enterpiseCode, string templetContent);
        /// <summary>
        /// 审核短信模板
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="auditStatus"></param>
        /// <param name="cause"></param>
        /// <returns></returns>
        SMS.Model.RPCResult AuditSMSTemplet(string templetID,string user,bool auditStatus,string cause);
        /// <summary>
        /// 获取需要审核短信模板列表
        /// </summary>
        /// <returns></returns>
        SMS.Model.RPCResult<List<SMSTemplet>> GetAuditSMSTemplet(DateTime beginTime,DateTime endTime);
        /// <summary>
        /// 获取所有短信模板列表
        /// </summary>
        /// <returns></returns>
        SMS.Model.RPCResult<List<SMSTemplet>> GetAllSMSTemplet(DateTime beginTime, DateTime endTime);
        /// <summary>
        /// 获取备案的短信模板列表
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<List<SMSTemplet>> GetSuccessSMSTemplet(string enterpise,DateTime beginTime,DateTime endTime);
        /// <summary>
        /// 获取失败的短信模板列表
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<List<SMSTemplet>> GetFailureSMSTemplet(string enterpise, DateTime beginTime, DateTime endTime);
        /// <summary>
        /// 直客端获取短信模板列表
        /// </summary>
        /// <param name="enterpiseCode"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<List<SMSTemplet>> GetZKDSMSTempletStauts(string enterpiseCode);

        /// <summary>
        /// 直客端删除短信模板
        /// </summary>
        /// <param name="templetID"></param>
        /// <returns></returns>
        SMS.Model.RPCResult ZKDDelSMSTemplet(string templetID);
        /// <summary>
        /// 直客端修改短信模板
        /// </summary>
        /// <param name="templetID"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        SMS.Model.RPCResult ZKDUpdateSMSTemplet(string templetID, string content);
        /// <summary>
        /// 获取短信模板内容
        /// </summary>
        /// <param name="templetID"></param>
        /// <returns></returns>
        SMS.Model.RPCResult<SMSTemplet> GetSMSTemplet(string templetID);
        #endregion
    }
}
