using SMS.DB;
using LogClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using SMSInterface;

namespace SMSService
{
    public partial class SMSService : MarshalByRefObject, ISMSService
    {
       
        #region 账号
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public RPCResult<Guid> CreateAccount(Account account)
        {
            Guid guid = System.Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(account.AccountID))
            {
                account.AccountID = guid.ToString();
            }
            else
            {
                guid = Guid.Parse(account.AccountID);
            }
            account.SMSNumber = 0;
            try
            {
                if (AccountDB.CreateAccount(account))
                {
                    return new RPCResult<Guid>(true, guid, "创建用户成功！");
                }
                LogHelper.LogWarn("SMSService", "SMSService.CreateAccount", "创建账号数据库失败");
                return new RPCResult<Guid>(false, guid, "创建账号失败！");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.CreateAccount", ex.ToString());
                return new RPCResult<Guid>(false, guid, "创建账号异常");
            }
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public RPCResult DelAccount(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
            {
                LogHelper.LogWarn("SMSService", "SMSService.DelAccount", "账号为空");
                return new RPCResult(false, "删除的账号不能为空");
            }
            try
            {
                if (AccountDB.DelAccount(accountID))
                {
                    return new RPCResult(true, "删除用户成功！");
                }
                LogHelper.LogWarn("SMSService", "SMSService.DelAccount", "删除用户数据库操作失败");
                return new RPCResult(false, "删除用户失败！");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.DelAccount", ex.ToString());
                return new RPCResult(false, "删除账号出现异常");
            }
        }
        /// <summary>
        /// 获取账号
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public RPCResult<Account> GetAccount(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
            {
                LogHelper.LogWarn("SMSService", "SMSService.GetAccount", "账号为空");
                return new RPCResult<Account>(false, null, "账号不能为空！");
            }
            try
            {
                Account account = AccountDB.GetAccount(accountID); //AccountServer.Instance.GetAccount(accountID);
                if (account == null) return new RPCResult<Account>(false, null, "不存在用户");
                //account.Password = "";
                return new RPCResult<Account>(true, account, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetAccount", ex.ToString());
                return new RPCResult<Account>(false, null, "获取账号出现异常！");
            }
        }
        public RPCResult<List<Account>> GetAccounts()
        {
            try
            {
                List<Account> accounts = AccountDB.GetAccounts();
                return new RPCResult<List<Account>>(true, accounts, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetAccounts", ex.ToString());
                return new RPCResult<List<Account>>(false, null, "获取账号出现异常！");
            }
        }

        /// <summary>
        /// 充值记录
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public RPCResult<List<PrepaidRecord>> GetPrepaidRecord(DateTime beginTime, DateTime endTime)
        {
            try
            {
                if (DateTime.Compare(beginTime, endTime) > 0)
                {
                    DateTime dt = beginTime;
                    beginTime = endTime;
                    endTime = dt;
                }
                if (DateTime.Compare(endTime, DateTime.Now) > 0)
                {
                    endTime = DateTime.Now;
                }
                List<PrepaidRecord> list = PrepaidRecordDB.Get(beginTime, endTime);
                return new RPCResult<List<PrepaidRecord>>(true, list, "");
            }
            catch
            {
                return new RPCResult<List<PrepaidRecord>>(false, null, "获取充值记录失败");
            }
        }
        /// <summary>
        /// 账号充值
        /// </summary>
        /// <param name="accountID">充值账号</param>
        /// <param name="quantity">充值数</param>
        /// <param name="operatorAccount">操作人</param>
        /// <returns></returns>
        public RPCResult AccountPrepaid(string accountID, uint quantity, string operatorAccount)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID))
                {
                    LogHelper.LogWarn("SMSService", "SMSService.AccountPrepaid", "充值账号为空");
                    return new RPCResult(false, "充值账号不能为空！");
                }
                Account account = AccountDB.GetAccount(accountID);//AccountServer.Instance.GetAccount(accountID);
                if (account == null)
                {
                    LogHelper.LogWarn("SMSService", "SMSService.AccountPrepaid", "充值账号不存在");
                    return new RPCResult(false, "账号不存在");
                }
                if (account.SMSNumber + (int)quantity < 0 && quantity > 10000000)
                {
                    return new RPCResult(false, "充值金额太大");
                }
                if (AccountDB.AccountPrepaid(account.AccountID, (int)quantity))
                {
                    PrepaidRecordDB.Add(operatorAccount, accountID, quantity);
                    return new RPCResult(true, "充值成功");
                }
                LogHelper.LogWarn("SMSService", "SMSService.AccountPrepaid", "充值数据库操作失败");
                return new RPCResult(false, "充值失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AccountPrepaid", ex.ToString());
                return new RPCResult(false, "充值失败");
            }
        }
        /// <summary>
        /// 账号扣费,返费
        /// </summary>
        /// <param name="accountID">扣费账号</param>
        /// <param name="quantity">扣费数 可以为负</param>
        /// <returns></returns>
        public RPCResult AccountDeductSMSCount(string accountID, int quantity)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID))
                {
                    LogHelper.LogWarn("SMSService", "SMSService.AccountDeductSMSCharge", "账号为空");
                    return new RPCResult(false, "扣费账号不能为空！");
                }
                Account account =AccountDB.GetAccount(accountID);// AccountServer.Instance.GetAccount(accountID);
                if (account == null)
                {
                    LogHelper.LogWarn("SMSService", "SMSService.AccountDeductSMSCharge", "账号不存在");
                    return new RPCResult(false, "账号不存在");
                }
                if (AccountDB.DeductAccountSMSCharge(account.AccountID, (int)quantity))
                {
                    return new RPCResult(true, "");
                }
                LogHelper.LogWarn("SMSService", "SMSService.AccountDeductSMSCharge", "扣费数据库操作失败");
                return new RPCResult(false, "扣费失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AccountDeductSMSCharge", ex.ToString());
                return new RPCResult(false, "扣费失败");
            }
        }
        #endregion

        #region 黑名单
        public RPCResult AddBlacklist(List<string> numbers)
        {
            try
            {
                if (numbers.Count == 0)
                {
                    return new RPCResult(false, "黑名单不能为空");
                }
                if (BlacklistDB.Add(numbers))
                {
                    string message = "Add" + (char)2;
                    foreach (var number in numbers)
                    {
                        message += number + (char)2;
                    }
                    BlacklistSend.Instance.Send(message.Substring(0, message.Length - 1));
                    return new RPCResult(true, "");
                }
                LogHelper.LogWarn("SMSService", "SMSService.AddBlacklist", "黑名单添加数据库失败");
                return new RPCResult(false, "添加黑名单失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AddBlacklist", ex.ToString());
                return new RPCResult(false, "添加黑名单出现错误");
            }
        }

        public RPCResult DelBlacklist(List<string> numbers)
        {
            try
            {
                if (numbers.Count == 0)
                {
                    return new RPCResult(false, "黑名单不能为空");
                }
                if (BlacklistDB.Del(numbers))
                {
                    string message = "Dec" + (char)2;
                    foreach (var number in numbers)
                    {
                        message += number + (char)2;
                    }
                    BlacklistSend.Instance.Send(message.Substring(0, message.Length - 1));
                    return new RPCResult(true, "");
                }
                LogHelper.LogWarn("SMSService", "SMSService.DelBlacklist", "黑名单数据库删除失败");
                return new RPCResult(false, "删除黑名单失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.DelBlacklist", ex.ToString());
                return new RPCResult(false, "删除黑名单出现错误");
            }
        }

        public RPCResult<List<string>> GetBlacklist()
        {
            try
            {
                List<string> list = BlacklistDB.GetNumbers();
                return new RPCResult<List<string>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetBlacklist", ex.ToString());
                return new RPCResult<List<string>>(false, null, "获取黑名单失败");
            }
        }
        #endregion

        #region 敏感词
        /// <summary>
        /// 添加敏感词组
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public RPCResult AddKeywordsGroup(string keyGroup, string remark)
        {
            try
            {
                if (keyGroup == "-1") return new RPCResult(false, "已存在此敏感词组");
                if (string.IsNullOrEmpty(keyGroup)) return new RPCResult(false, "敏感词组不能为空");
                if (WordfilteDB.ExistGroup(keyGroup)) return new RPCResult(false, "已存在此敏感词组");
                bool ok = WordfilteDB.AddKeyGroup(keyGroup, remark);
                if (ok) return new RPCResult(true, "");
                return new RPCResult(false, "添加敏感词组失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AddKeywordsGroup", ex.ToString());
                return new RPCResult(false, "添加敏感词组失败");
            }
        }
        /// <summary>
        /// 添加敏感词类别
        /// </summary>
        /// <param name="type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public RPCResult AddKeywordsType(string type, string remark)
        {
            try
            {
                if (type == "-1") return new RPCResult(false, "已存在此敏感词类型");
                if (string.IsNullOrEmpty(type)) return new RPCResult(false, "敏感词类型不能为空");
                if (WordfilteDB.ExistType(type)) return new RPCResult(false, "已存在此敏感词类型");
                bool ok = WordfilteDB.AddKeyType(type, remark);
                if (ok) return new RPCResult(true, "");
                return new RPCResult(false, "添加敏感词类型失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AddKeywordsType", ex.ToString());
                return new RPCResult(false, "添加敏感词类型失败");
            }
        }
        /// <summary>
        /// 添加词组与类别绑定
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keyTypes"></param>
        /// <returns></returns>
        public RPCResult AddKeywordsGroupTypeBind(string keyGroup, List<string> keyTypes)
        {
            try
            {
                if (string.IsNullOrEmpty(keyGroup)) return new RPCResult(false, "敏感词组不能为空");
                if (keyTypes == null || keyTypes.Count == 0) return new RPCResult(false, "敏感词类型不能为空");
                if (WordfilteDB.DelKeyTypesByGroup(keyGroup))
                {
                    bool ok = WordfilteDB.AddGroupTypeBind(keyGroup, keyTypes);
                    if (ok) return new RPCResult(true, "");
                }
                return new RPCResult(false, "添加敏感词组与类别绑定失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AddKeywordsGroupTypeBind", ex.ToString());
                return new RPCResult(false, "添加敏感词组与类别绑定失败");
            }
        }
        /// <summary>
        /// 获取敏感词类别
        /// </summary>
        /// <returns></returns>
        public RPCResult<Dictionary<string, string>> GetKeywordsTypes()
        {
            try
            {
                Dictionary<string, string> list = WordfilteDB.GetKeyTypes();
                return new RPCResult<Dictionary<string, string>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetKeywordsTypes", ex.ToString());
                return new RPCResult<Dictionary<string, string>>(false, null, "获取敏感词类别失败");
            }
        }
        /// <summary>
        /// 根据词组获取敏感词类别
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public RPCResult<List<string>> GetKeywordsTypesByGroup(string keyGroup)
        {
            try
            {
                if (string.IsNullOrEmpty(keyGroup)) return new RPCResult<List<string>>(false, null, "敏感词组不能为空");
                List<string> list = WordfilteDB.GetKeyTypesByGroup(keyGroup);
                return new RPCResult<List<string>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetKeywordsTypesByGroup", ex.ToString());
                return new RPCResult<List<string>>(false, null, "获取敏感词组类别失败");
            }
        }

        /// <summary>
        /// 敏感词添加
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public RPCResult AddKeywords(string keyGroup, List<Keywords> keywords)
        {
            try
            {
                if (string.IsNullOrEmpty(keyGroup))
                {
                    return new RPCResult(false, "关键词组名不能为空！");
                }
                if (keywords.Count == 0)
                {
                    return new RPCResult(false, "关键词不能为空");
                }
                if (WordfilteDB.Add(keyGroup, keywords))
                {
                    //WordfilteDB.Del(keyGroup, new List<string> { ""});
                    string message = "ADD" + (char)5;
                    foreach (var key in keywords)
                    {
                        if (key.Enable)
                        {
                            message += key.Words + (char)5;
                        }
                    }
                    KeywordsSend.KeyGroup = keyGroup;
                    KeywordsSend.Instance.Send(keyGroup, message.Substring(0, message.Length - 1));
                    return new RPCResult(true, "");
                }
                LogHelper.LogWarn("SMSService", "SMSService.AddKeywords", "关键字添加数据库失败");
                return new RPCResult(false, "添加关键字失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AddKeywords", ex.ToString());
                return new RPCResult(false, "添加关键字出现错误");
            }
        }
        /// <summary>
        /// 删除敏感词
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public RPCResult DelKeywords(string keyGroup, List<string> keywords)
        {
            try
            {
                if (string.IsNullOrEmpty(keyGroup))
                {
                    return new RPCResult(false, "关键词组名不能为空！");
                }
                if (keywords.Count == 0)
                {
                    return new RPCResult(false, "关键词不能为空");
                }

                if (WordfilteDB.Del(keyGroup, keywords))
                {
                    string message = "DEL" + (char)5;
                    foreach (var key in keywords)
                    {
                        message += key + (char)5;
                    }
                    KeywordsSend.KeyGroup = keyGroup;
                    KeywordsSend.Instance.Send(keyGroup, message.Substring(0, message.Length - 1));
                    return new RPCResult(true, "");
                }
                LogHelper.LogWarn("SMSService", "SMSService.DelKeywords", "关键字删除数据库失败");
                return new RPCResult(false, "删除关键字失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.DelKeywords", ex.ToString());
                return new RPCResult(false, "删除关键字出现错误");
            }
        }

        /// <summary>
        /// 删除敏感词组
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public RPCResult DelKeywordGroup(string keyGroup)
        {
            try
            {
                if (string.IsNullOrEmpty(keyGroup))
                {
                    return new RPCResult(false, "敏感词组名不能为空！");
                }
                if (KeywordsGatewayBindDB.GetGateways(keyGroup).Count > 0)
                {
                    return new RPCResult(false, "敏感词组正在使用，不允许删除");
                }
                if (WordfilteDB.GetCountKeywords(keyGroup) > 0)
                {
                    return new RPCResult(false, "敏感词组内还包含敏感词，不允许删除");
                }
                if (WordfilteDB.DelKeyTypesByGroup(keyGroup))
                {
                    if (WordfilteDB.DelKeyGroup(keyGroup)) return new RPCResult(true, "");
                }
                return new RPCResult(false, "删除词组失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.DelKeywordGroup", ex.ToString());
                return new RPCResult(false, "删除词组失败出现错误");
            }
        }

        /// <summary>
        /// 根据词组获取敏感词
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public RPCResult<List<Keywords>> GetKeywords(string keyGroup)
        {
            try
            {
                if (string.IsNullOrEmpty(keyGroup))
                {
                    return new RPCResult<List<Keywords>>(false, null, "关键词组名不能为空！");
                }
                List<Keywords> list = new List<Keywords>();
                list = WordfilteDB.Gets(keyGroup);
                return new RPCResult<List<Keywords>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetKeywords", ex.ToString());
                return new RPCResult<List<Keywords>>(false, null, "获取失败");
            }
        }
        /// <summary>
        /// 根据类型获取敏感词
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public RPCResult<List<Keywords>> GetKeywordsByType(string type)
        {
            try
            {
                List<Keywords> list = new List<Keywords>();
                list = WordfilteDB.GetKeywordsByType(type);
                return new RPCResult<List<Keywords>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetKeywordsByType", ex.ToString());
                return new RPCResult<List<Keywords>>(false, null, "获取失败");
            }
        }
        /// <summary>
        /// 敏感词状态启用与否
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public RPCResult KeywordsEnabled(string keyGroup, string keywords, bool enabled)
        {
            try
            {
                if (string.IsNullOrEmpty(keyGroup))
                {
                    return new RPCResult(false, "关键词组名不能为空！");
                }
                if (string.IsNullOrEmpty(keywords))
                {
                    return new RPCResult(false, "关键词不能为空");
                }
                bool ok = WordfilteDB.KeywordsEnabled(keyGroup, keywords, enabled);
                if (ok)
                {
                    if (enabled)
                    {
                        //敏感词启用
                        string message = "ADD" + (char)5 + keywords + (char)5;
                        KeywordsSend.KeyGroup = keyGroup;
                        KeywordsSend.Instance.Send(keyGroup, message.Substring(0, message.Length - 1));
                    }
                    else
                    {
                        //敏感词禁用
                        string message = "DEL" + (char)5 + keywords + (char)5;
                        KeywordsSend.KeyGroup = keyGroup;
                        KeywordsSend.Instance.Send(keyGroup, message.Substring(0, message.Length - 1));
                    }
                    return new RPCResult(true, "");
                }
                return new RPCResult(false, "操作失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.KeywordsEnabled", ex.ToString());
                return new RPCResult(false, "操作失败");
            }
        }
        /// <summary>
        /// 更新敏感词（更新类型和替换成其他词语）
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public RPCResult UpdateKeywords(Keywords keyword)
        {
            try
            {
                bool ok = WordfilteDB.Update(keyword);
                if (ok)
                {
                    return new RPCResult(true, "");
                }
                return new RPCResult(false, "更新失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.UpdateKeywords", ex.ToString());
                return new RPCResult(false, "更新失败");
            }
        }
        /// <summary>
        /// 获取敏感词组
        /// </summary>
        /// <returns></returns>
        public RPCResult<Dictionary<string, string>> GetKeyGroups()
        {
            try
            {
                Dictionary<string, string> dic = WordfilteDB.GetKeyGroups();
                return new RPCResult<Dictionary<string, string>>(true, dic, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetKeyGroups", ex.ToString());
                return new RPCResult<Dictionary<string, string>>(false, null, "获取失败");
            }
        }
        /// <summary>
        /// 添加关键词组与网关绑定信息
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="gateways"></param>
        /// <returns></returns>
        public RPCResult AddkeyGroupGatewayBind(string keyGroup, string gateway)
        {
            try
            {
                if (string.IsNullOrEmpty(keyGroup))
                {
                    return new RPCResult(false, "关键词组不能为空");
                }
                if (string.IsNullOrEmpty(gateway))
                {
                    return new RPCResult(false, "网关名不能为空");
                }
                if (KeywordsGatewayBindDB.Del(gateway))
                {
                    if (KeywordsGatewayBindDB.Add(keyGroup, gateway))
                    {
                        return new RPCResult(true, "");
                    }
                }
                LogHelper.LogWarn("SMSService", "SMSService.AddkeyGroupGatewayBind", "关键词组绑定网关数据库操作失败");
                return new RPCResult(false, "关键词组绑定网关失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AddkeyGroupGatewayBind", ex.ToString());
                return new RPCResult(false, "关键词组绑定网关失败");
            }
        }
        /// <summary>
        /// 获取网关指定的关键词组
        /// </summary>
        /// <param name="keywordGroup"></param>
        /// <returns></returns>
        public RPCResult<string> GetKeyGroupGatewayBinds(string gateway)
        {
            try
            {
                if (string.IsNullOrEmpty(gateway))
                {
                    return new RPCResult<string>(false, "", "网关名不能为空");
                }
                string str = KeywordsGatewayBindDB.GetkeyGroup(gateway);
                return new RPCResult<string>(true, str, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetKeyGroupGatewayBinds", ex.ToString());
                return new RPCResult<string>(false, "", "获取网关指定的关键词组失败");
            }
        }
        #endregion

        #region 网关配置

        public RPCResult AddGatewayConfig(GatewayConfiguration config)
        {
            try
            {
                if (GatewayConfigDB.Exist(config.Gateway))
                {
                    return new RPCResult(false, "已存在此网关");
                }
                if (GatewayConfigDB.Add(config))
                {
                    return new RPCResult(true, "");
                }
                LogHelper.LogWarn("SMSService", "SMSService.AddGatewayConfig", "网关添加数据库失败");
                return new RPCResult(false, "添加网关失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AddGatewayConfig", ex.ToString());
                return new RPCResult(false, "添加网关出现错误");
            }
        }

        public RPCResult UpdateGatewayConfig(GatewayConfiguration config)
        {
            try
            {
                if (GatewayConfigDB.Update(config))
                {
                    return new RPCResult(true, "");
                }
                LogHelper.LogWarn("SMSService", "SMSService.UpdateGatewayConfig", "网关更新数据库失败");
                return new RPCResult(false, "更新网关失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.UpdateGatewayConfig", ex.ToString());
                return new RPCResult(false, "更新网关出现错误");
            }
        }

        public RPCResult<GatewayConfiguration> GetGatewayConfig(string gateway)
        {
            try
            {
                GatewayConfiguration config = GatewayConfigDB.GetConfig(gateway);
                return new RPCResult<GatewayConfiguration>(true, config, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetGatewayConfig", ex.ToString());
                return new RPCResult<GatewayConfiguration>(false, null, "获取网关出现错误");
            }
        }

        public RPCResult<List<GatewayConfiguration>> GetGatewayConfigs()
        {
            try
            {
                List<GatewayConfiguration> list = GatewayConfigDB.GetConfigs();
                return new RPCResult<List<GatewayConfiguration>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetGatewayConfigs", ex.ToString());
                return new RPCResult<List<GatewayConfiguration>>(false, null, "获取网关出现错误");
            }
        }

        public RPCResult DelGatewayConfig(string gateway)
        {
            try
            {
                if (GatewayConfigDB.Del(gateway))
                {
                    KeywordsGatewayBindDB.Del(gateway);
                    ChannelDB.DelGatewayByGateway(gateway);
                    return new RPCResult(true, "");
                }
                LogHelper.LogWarn("SMSService", "SMSService.DelGatewayConfig", "网关删除数据库失败");
                return new RPCResult(false, "删除网关失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.DelGatewayConfig", ex.ToString());
                return new RPCResult(false, "删除网关出现错误");
            }
        }
        #endregion

        #region 通道配置
        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public RPCResult AddChannel(Channel channel)
        {
            try
            {
                if (channel.ChannelID == "-1-") return new RPCResult(false, "已存在此短信通道");
                if (ChannelDB.GetChannel(channel.ChannelID) != null) return new RPCResult(false, "已存在此短信通道");
                if (ChannelDB.AddChannle(channel))
                {
                    return new RPCResult(true, "");
                }
                LogHelper.LogWarn("SMSService", "SMSService.AddChannel", "通道添加数据库失败");
                return new RPCResult(false, "添加通道失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AddChannel", ex.ToString());
                return new RPCResult(false, "添加通道出现错误");
            }
        }
        /// <summary>
        /// 通道更新
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public RPCResult UpdateChannel(Channel channel)
        {
            try
            {
                if (ChannelDB.UpdateChannel(channel))
                {
                    return new RPCResult(true, "");
                }
                LogHelper.LogWarn("SMSService", "SMSService.UpdateChannel", "通道更新数据库失败");
                return new RPCResult(false, "更新通道失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.UpdateChannel", ex.ToString());
                return new RPCResult(false, "更新通道出现错误");
            }
        }

        public RPCResult<List<Channel>> GetChannels()
        {
            try
            {
                List<Channel> list = ChannelDB.GetChannels();
                if (list == null || list.Count == 0)
                {
                    return new RPCResult<List<Channel>>(false, null, "无通道信息");
                }
                return new RPCResult<List<Channel>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetChannels", ex.ToString());
                return new RPCResult<List<Channel>>(false, null, "获取通道信息失败");
            }
        }

        public RPCResult<Channel> GetChannel(string channel)
        {
            try
            {
                if (string.IsNullOrEmpty(channel)) return new RPCResult<Channel>(false, null, "通道不能为空");
                Channel ch = ChannelDB.GetChannel(channel);
                if (ch == null) return new RPCResult<Channel>(false, null, "不存在此通道");
                return new RPCResult<Channel>(true, ch, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetChannel", ex.ToString());
                return new RPCResult<Channel>(false, null, "获取通道失败");
            }
        }
        /// <summary>
        /// 通道删除
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public RPCResult DelChannel(string channel)
        {
            try
            {
                if (string.IsNullOrEmpty(channel)) return new RPCResult(false, "通道不能为空");
                if (ChannelDB.GetGatewaysByChannel(channel).Count > 0) return new RPCResult(false, "通道已绑定其他网关");
                if (ChannelDB.DelChannel(channel))
                {
                    return new RPCResult(true, "");
                }

                LogHelper.LogError("SMSService", "SMSService.DelChannel", "删除通道数据库操作失败");
                return new RPCResult(false, "删除通道失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.DelChannel", ex.ToString());
                return new RPCResult(false, "通道删除失败");
            }
        }
        /// <summary>
        /// 通道绑定网关
        /// </summary>
        /// <param name="channel">通道编号</param>
        /// <param name="gateways">网关</param>
        /// <returns></returns>
        public RPCResult AddChannelGatewayBind(string channel, List<string> gateways)
        {
            try
            {
                if (string.IsNullOrEmpty(channel))
                {
                    return new RPCResult(false, "通道不能为空");
                }
                if (gateways == null || gateways.Count == 0)
                {
                    ChannelDB.DelGatewayByChannel(channel);
                    return new RPCResult(true, "通道没有绑定网关。");
                    // return new RPCResult(false, "绑定的网关不能为空");
                }
                if (ChannelDB.DelGatewayByChannel(channel))
                {
                    if (ChannelDB.AddChannelGatewayBind(channel, gateways))
                    {
                        return new RPCResult(true, "");
                    }
                }
                LogHelper.LogWarn("SMSService", "SMSService.AddChannelGatewayBind", "通道绑定网关数据库操作失败");
                return new RPCResult(false, "通道绑定网关失败");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.AddChannelGatewayBind", ex.ToString());
                return new RPCResult(false, "通道绑定网关失败");
            }
        }
        /// <summary>
        /// 获取通道绑定的网关
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public RPCResult<List<string>> GetGatewaysByChannel(string channel)
        {
            try
            {
                if (string.IsNullOrEmpty(channel))
                {
                    return new RPCResult<List<string>>(false, null, "网关名不能为空");
                }
                List<string> str = ChannelDB.GetGatewaysByChannel(channel);
                return new RPCResult<List<string>>(true, str, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetGatewaysByChannel", ex.ToString());
                return new RPCResult<List<string>>(false, null, "获取网关指定的关键词组失败");
            }
        }
        #endregion


        #region 敏感词
        /// <summary>
        /// 根据敏感词类型获取敏感词组
        /// </summary>
        /// <param name="keyType"></param>
        /// <returns></returns>
        public RPCResult<List<string>> GetKeywordsGroupByType(string keyType)
        {
            try
            {
                if (string.IsNullOrEmpty(keyType)) return new RPCResult<List<string>>(false, null, "敏感词类型是空的");
                List<string> list = WordfilteDB.GetKeywordsGroupByType(keyType);
                return new RPCResult<List<string>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetKeywordsGroupByType", ex.ToString());
                return new RPCResult<List<string>>(false, null, "获取敏感词类型失败");
            }
        }
        /// <summary>
        /// 根据词获取敏感词（模糊查询）
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public RPCResult<List<Keywords>> GetKeywordsByKeyword(string keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword)) return new RPCResult<List<Keywords>>(false, null, "查询的敏感词是空的");
                List<Keywords> list = WordfilteDB.GetKeywordsByKeyword(keyword);
                return new RPCResult<List<Keywords>>(true, list, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetKeywordsByKeyword", ex.ToString());
                return new RPCResult<List<Keywords>>(false, null, "获取敏感词失败");
            }
        }

        /// <summary>
        /// 获取所有敏感词
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public RPCResult<Dictionary<int, List<Keywords>>> GetAllKeywords(int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex < 0) pageIndex = 0;
                if (pageSize <= 0) new RPCResult<List<Keywords>>(false, null, "每页的显示的条数应大于0");
                List<Keywords> list = WordfilteDB.GetAllKeywords(pageIndex, pageSize);
                int count = WordfilteDB.GetAllKeywordCount();
                Dictionary<int, List<Keywords>> dic = new Dictionary<int, List<Keywords>>();
                dic.Add(count, list);
                return new RPCResult<Dictionary<int, List<Keywords>>>(true, dic, "");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("SMSService", "SMSService.GetAllKeywords", ex.ToString());
                return new RPCResult<Dictionary<int, List<Keywords>>>(false, null, "获取敏感词失败");
            }
        }
        #endregion
    }
}
