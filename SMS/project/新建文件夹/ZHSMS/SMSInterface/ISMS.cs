using SMS.Model;
using Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    public partial interface ISMS : IMonitoring
    {
        #region 帐号
        /// <summary>
        /// 创建账号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        RPCResult<Guid> CreateAccount(Account account);
        /// <summary>
        /// 删除账号
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        RPCResult DelAccount(string accountID);
        /// <summary>
        /// 获取账号
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        RPCResult<Account> GetAccount(string accountID);
        /// <summary>
        /// 获取所有账号
        /// </summary>
        /// <returns></returns>
        RPCResult<List<Account>> GetAccounts();
        /// <summary>
        /// 账号充值
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="quantity"></param>
        /// <param name="operatorAccount"></param>
        /// <returns></returns>
        RPCResult AccountPrepaid(string accountID, uint quantity,string operatorAccount);
        /// <summary>
        /// 账号充值记录
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        RPCResult<List<PrepaidRecord>> GetPrepaidRecord(DateTime beginTime,DateTime endTime);
        /// <summary>
        /// 账号扣费
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        RPCResult AccountDeductSMSCount(string accountID, int quantity);
        #endregion

        #region 黑名单
        RPCResult AddBlacklist(List<string> numbers);
        RPCResult DelBlacklist(List<string> numbers);
        RPCResult<List<string>> GetBlacklist();
        #endregion

        #region 敏感词
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
        /// 根据词获取敏感词（模糊查询）
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        RPCResult<List<Keywords>> GetKeywordsByKeyword(string keyword);
        /// <summary>
        /// 添加敏感词
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        RPCResult AddKeywords(string keyGroup, List<Keywords> keywords);
        /// <summary>
        /// 删除敏感词
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
        RPCResult DelKeywordGroup(string keyGroup);
        /// <summary>
        /// 根据词组获取敏感词
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
        RPCResult AddkeyGroupGatewayBind(string keyGroup,string gateway);
        /// <summary>
        /// 获取网关指定的关键词组
        /// </summary>
        /// <param name="keywordGroup"></param>
        /// <returns></returns>
        RPCResult<string> GetKeyGroupGatewayBinds(string gateway);

        /// <summary>
        /// 获取所有敏感词
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        RPCResult<Dictionary<int,List<Keywords>>> GetAllKeywords(int pageIndex,int pageSize);

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
        RPCResult<Channel> GetChannel(string channel);
        /// <summary>
        /// 删除通道
        /// </summary>
        /// <param name="channel"></param>
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

    }
}
