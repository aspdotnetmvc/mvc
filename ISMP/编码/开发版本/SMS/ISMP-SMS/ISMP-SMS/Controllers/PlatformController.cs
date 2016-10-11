using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISMPModel;
using System.Data;
using Newtonsoft.Json;
using ISMPInterface;
using BXM.Utils;
using BXM.Logger;
using Newtonsoft.Json.Converters;

namespace ISMP_SMS.Controllers
{
    /// <summary>
    /// 系统平台配置
    /// </summary>
    public class PlatformController : BaseController
    {

        #region 网关配置
        /// <summary>
        /// 网关配置
        /// </summary>
        /// <returns></returns>
        public ActionResult GatewayConfig()
        {
            return View();
        }

        public ActionResult AddGateway()
        {
            try
            {
                var gateway = new SMS.Model.GatewayConfiguration();
                gateway.Gateway = Request["Gateway"];
                gateway.MinPackageSize = int.Parse(Request["MinPackageSize"]);
                gateway.MaxPackageSize = int.Parse(Request["MaxPackageSize"]);

                if (gateway.MaxPackageSize <= 0 || gateway.MaxPackageSize > 5000)
                {
                    return Error("单包最大大小必须大于0 小于 5000");
                }
                string Operators = Request["Operators"];
                if (!string.IsNullOrWhiteSpace(Operators))
                {
                    gateway.Operators = Operators.Split(',').ToList();
                }
                var r = Util.SMSProxy.AddGatewayConfig(gateway);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "网关配置", "添加网关", "网关名字【" + gateway.Gateway + "】，运营商【" + Operators + "】", "添加网关", CurrentUser);
                }

                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new ISMPModel.RPC_Result(false, "操作异常！"));
            }
        }
        public ActionResult UpdateGateway()
        {
            try
            {
                var gateway = new SMS.Model.GatewayConfiguration();
                gateway.Gateway = Request["Gateway"];
                gateway.MinPackageSize = int.Parse(Request["MinPackageSize"]);
                gateway.MaxPackageSize = int.Parse(Request["MaxPackageSize"]);

                if (gateway.MaxPackageSize <= 0 || gateway.MaxPackageSize > 5000)
                {
                    return Error("单包最大大小必须大于0 小于 5000");
                }
                string Operators = Request["Operators"];
                if (!string.IsNullOrWhiteSpace(Operators))
                {
                    gateway.Operators = Operators.Split(',').ToList();
                }
                var r = Util.SMSProxy.UpdateGatewayConfig(gateway);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "网关配置", "编辑网关", "修改为：网关名字【" + gateway.Gateway + "】，运营商【" + Operators + "】", "编辑网关", CurrentUser);
                }

                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new ISMPModel.RPC_Result(false, "操作异常！"));
            }
        }
        public ActionResult DeleteGateway(string Gateway)
        {
            try
            {
                var r = Util.SMSProxy.DelGatewayConfig(Gateway);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "网关配置", "删除网关", "网关【" + Gateway + "】", "删除网关", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new ISMPModel.RPC_Result(false, "操作异常！"));
            }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        public ActionResult GatewayList()
        {
            var r = Util.SMSProxy.GetGatewayConfigs();

            int rows = int.Parse(Request["rows"]);
            int page = int.Parse(Request["page"]);

            var rs = PageQuery(r.Value, page, rows);

            return GetActionResult_Grid(rs);
        }
        public ActionResult GetAllGateways()
        {
            var r = Util.SMSProxy.GetGatewayConfigs();
            return Json(r.Value);
        }

        public ActionResult GetGatewayKeywordsGroup(string Gateway)
        {

            SMS.Model.RPCResult<string> ra = Util.SMSProxy.GetKeyGroupGatewayBinds(Gateway);
            return GetActionResult(ra);
        }


        public ActionResult AllocateKeywordsGroupForGateway()
        {
            string Gateway = Request["Gateway"];
            string KeywordsGroup = Request["KeywordsGroup"];
            var r = Util.SMSProxy.AddkeyGroupGatewayBind(KeywordsGroup, Gateway);
            if (r.Success)
            {
                Util.SendSystemLogToISMP(Util.SMSProductName + "网关配置", "分配敏感词组", "网关【" + Gateway + "】，敏感词组【" + KeywordsGroup + "】", "分配敏感词组", CurrentUser);
            }
            return GetActionResult(r);
        }
        #endregion

        #region 通道配置
        /// <summary>
        /// 通道配置
        /// </summary>
        /// <returns></returns>
        public ActionResult ChannelManage()
        {
            return View();
        }

        public ActionResult AddChannel()
        {
            try
            {
                SMS.Model.Channel ch = new SMS.Model.Channel();
                ch.ChannelID = Request["ChannelID"];
                ch.ChannelName = Request["ChannelName"];
                ch.SMSType = (SMS.Model.SMSType)int.Parse(Request["SMSType"]);
                ch.Remark = Request["Remark"];
                var r = Util.SMSProxy.AddChannel(ch);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "通道配置", "添加通道", "通道(编码)【" + ch.ChannelID + "】，通道名称【" + ch.ChannelName + "】，通道说明【" + ch.Remark + "】", "添加通道", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new ISMPModel.RPC_Result(false, "操作异常！"));
            }
        }
        public ActionResult UpdateChannel()
        {
            try
            {
                var ch = new SMS.Model.Channel();
                ch.ChannelID = Request["ChannelID"];
                ch.ChannelName = Request["ChannelName"];
                ch.SMSType = (SMS.Model.SMSType)int.Parse(Request["SMSType"]);
                ch.Remark = Request["Remark"];
                var r = Util.SMSProxy.UpdateChannel(ch);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "通道配置", "编辑通道", "修改为：通道(编码)【" + ch.ChannelID + "】，通道名称【" + ch.ChannelName + "】，通道说明【" + ch.Remark + "】", "编辑通道", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new ISMPModel.RPC_Result(false, "操作异常！"));
            }
        }
        public ActionResult DeleteChannel(string ChannelID)
        {
            try
            {
                var r = Util.SMSProxy.DelChannel(ChannelID);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "通道配置", "删除通道", "通道【" + ChannelID + "】", "删除通道", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        public ActionResult ChannelList()
        {
            var r = Util.SMSProxy.GetChannels();
            #region 测试
            //   var r = new RPC_Result<List<Channel>>(true);
            //r.Value = new List<Channel>();
            //Channel ch = new Channel();
            //ch.ChannelID = "11";
            //ch.ChannelName = "11";
            //ch.Remark = "备注";
            //r.Value.Add(ch);

            #endregion

            int rows = int.Parse(Request["rows"]);
            int page = int.Parse(Request["page"]);

            var rs = PageQuery(r.Value, page, rows);

            ResultSet rs2 = new ResultSet();
            rs2.Value = new List<IDictionary<string, object>>();
            rs2.Total = rs.Total;
            foreach (var c in rs.Value)
            {
                IDictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("ChannelID", c.ChannelID);
                dic.Add("ChannelName", c.ChannelName);
                dic.Add("SMSType", (int)c.SMSType);
                dic.Add("Remark", c.Remark);
                try
                {
                    var l = Util.SMSProxy.GetGatewaysByChannel(c.ChannelID);
                    dic.Add("Gateways", l.Value);
                }
                catch (Exception ex)
                {
                    Log4Logger.Error(ex);
                }
                rs2.Value.Add(dic);
            }
            return GetActionResult_Grid(rs2);
        }
        public ActionResult AllocateGatewayForChannel()
        {
            string Gateways = Request["Gateways"];
            List<string> gatewaylist = new List<string>();
            if (!string.IsNullOrWhiteSpace(Gateways))
            {
                gatewaylist.AddRange(Gateways.Split(','));
            }
            string Channel = Request["ChannelID"];
            var r = Util.SMSProxy.AddChannelGatewayBind(Channel, gatewaylist);
            if (r.Success)
            {
                Util.SendSystemLogToISMP(Util.SMSProductName + "通道配置", "分配网关", "通道【" + Channel + "】，网关【" + Gateways + "】", "分配网关", CurrentUser);
            }
            return GetActionResult(r);
        }

        #endregion

        #region 短信模版审核记录

        public ActionResult TemplateAuditRecord()
        {
            return View();
        }

        public ActionResult GetTemplateAuditRecord()
        {
            var r = new ResultSet<SMS.Model.SMSTemplet>();
            r.Value = new List<SMS.Model.SMSTemplet>();
            try
            {
                WebParamList wpl = new WebParamList(Request);
                DateTime temp = DateTime.MinValue;
                DateTime startTime = DateTime.TryParse(wpl.str("StartTime"), out temp) ? temp : DateTime.MinValue;
                temp = DateTime.Now;
                DateTime endTime = DateTime.TryParse(wpl.str("EndTime"), out temp) ? temp : DateTime.Now;

                SMS.Model.RPCResult<List<SMS.Model.SMSTemplet>> result = null;
                if (wpl.str("AuditResult").Trim() == "审核失败")
                {
                    //失败
                    result = Util.SMSProxy.GetFailureSMSTemplet(null, startTime, endTime);
                }
                else if (wpl.str("AuditResult").Trim() == "审核通过")
                {
                    //审核通过
                    result = Util.SMSProxy.GetSuccessSMSTemplet(null, startTime, endTime);
                }
                else
                {
                    //全部
                    result = Util.SMSProxy.GetAllSMSTemplet(startTime, endTime);
                }

                if (result != null && result.Success && result.Value != null)
                {
                    r.Total = result.Value.Count;
                    r.Value = result.Value;
                    //分页
                    if (r.Value != null && r.Value.Count > 0)
                    {
                        var index = (wpl.page - 1) * wpl.rows;
                        var range = (r.Value.Count > index) ? (r.Value.Count - index) : 0;
                        var count = (wpl.rows > range) ? range : wpl.rows;
                        r.Value = r.Value.GetRange(index, count);
                    }
                }

                return Content(r.ToString());
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return Content(r.ToString());
            }
        }

        #endregion

        #region 短信审核记录

        public ActionResult SendSMSAuditRecord()
        {
            return View();
        }

        public ActionResult GetSendSMSAuditRecord()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                DateTime temp = DateTime.MinValue;
                DateTime startTime = DateTime.TryParse(wpl.str("StartTime"), out temp) ? temp : DateTime.MinValue;
                temp = DateTime.Now;
                DateTime endTime = DateTime.TryParse(wpl.str("EndTime"), out temp) ? temp : DateTime.Now;

                SMS.Model.QueryParams qp = new SMS.Model.QueryParams();
                qp.page = wpl.page;
                qp.rows = wpl.rows;
                qp.add("StartTime", wpl.get("StartTime"));
                qp.add("StartTime", wpl.get("EndTime"));
                if (wpl.str("AuditResult").Trim() == "审核失败")
                {
                    qp.add("AuditResult", "2");
                }
                else if (wpl.str("AuditResult").Trim() == "审核通过")
                {
                    qp.add("AuditResult", "1");
                }
                var r = Util.SMSProxy.GetSMSByAudit(qp);
                if (!r.Success)
                {
                    return GetActionResult(r);
                }
                else
                {
                    return Content(r.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new ISMPModel.RPC_Result(false, "异常！"));
            }
        }

        #endregion

        #region 敏感词

        #region 敏感词组
        public ActionResult KeywordsGroup()
        {
            return View();
        }
        public ActionResult KeywordsGroupData()
        {
            SMS.Model.RPCResult<Dictionary<string, string>> r = Util.SMSProxy.GetKeyGroups();
            if (r.Success)
            {
                int rows = int.Parse(Request["rows"]);
                int page = int.Parse(Request["page"]);
                var list = from k in r.Value select new { KeywordsGroup = k.Key, Remark = k.Value };
                var rs = PageQuery(list, page, rows);

                return GetActionResult_Grid(rs);
            }
            else
            {
                return GetActionResult(r);
            }
        }
        public ActionResult AddKeyWordsGroup(string KeywordsGroup, string Remark)
        {
            try
            {
                SMS.Model.RPCResult r = Util.SMSProxy.AddKeywordsGroup(KeywordsGroup, Remark);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "敏感词组", "添加", "敏感词组【" + KeywordsGroup + "】，备注【" + Remark + "】", "添加", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new SMS.Model.RPCResult(false, "操作异常！"));
            }
        }
        /// <summary>
        /// 删除敏感词组
        /// </summary>
        /// <param name="KeywordsGroup"></param>
        /// <returns></returns>
        public ActionResult DeleteKeywordsGroup(string KeywordsGroup)
        {
            try
            {
                var r = Util.SMSProxy.DelKeywordsGroup(KeywordsGroup);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "敏感词组", "删除", "敏感词组【" + KeywordsGroup + "】", "删除", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new SMS.Model.RPCResult(false, "操作异常！"));
            }
        }
        public ActionResult KeywordsGroupComboboxData()
        {
            SMS.Model.RPCResult<Dictionary<string, string>> r = Util.SMSProxy.GetKeyGroups();
            var q = from kg in r.Value.Keys select new { id = kg, text = kg };
            var row = q.ToList();
            row.Insert(0, new { id = "", text = " " });
            return Content(JsonConvert.SerializeObject(row));
        }


        #endregion

        #region 敏感词
        public ActionResult Keywords()
        {
            return View();
        }
        public ActionResult KeywordsData(string KeywordsGroup, string KeywordsType, string Keywords)
        {
            string KeywordsGroups = Request["KeywordsGroup"];
            var r = Util.SMSProxy.GetAllKeywords();

            if (r.Success)
            {
                var list = r.Value;
                if (!string.IsNullOrWhiteSpace(KeywordsGroup))
                {
                    list = list.Where(k => k.KeyGroup == KeywordsGroup).ToList();
                }
                if (!string.IsNullOrWhiteSpace(KeywordsType))
                {
                    list = list.Where(k => k.KeywordsType == KeywordsType).ToList();
                }
                if (!string.IsNullOrWhiteSpace(Keywords))
                {
                    list = list.Where(k => k.Words.IndexOf(Keywords) >= 0).ToList();
                }
                int rows = int.Parse(Request["rows"]);
                int page = int.Parse(Request["page"]);
                var rs = PageQuery(list, page, rows);

                return GetActionResult_Grid(rs);
            }
            else
            {
                return GetActionResult(r);
            }
        }




        public ActionResult AddKeywords(string KeyGroup, string KeywordsType, string Words, bool Enable, string ReplaceKeywords)
        {
            try
            {
                List<SMS.Model.Keywords> list = new List<SMS.Model.Keywords>();

                foreach (string s in Words.Split(','))
                {
                    SMS.Model.Keywords keyword = new SMS.Model.Keywords();
                    keyword.Enable = Enable;
                    keyword.KeyGroup = KeyGroup;
                    keyword.KeywordsType = KeywordsType;
                    keyword.ReplaceKeywords = ReplaceKeywords;
                    keyword.Words = s;
                    if (!string.IsNullOrEmpty(keyword.Words))
                    {
                        list.Add(keyword);
                    }
                }
                if (list.Count == 0)
                {

                    return GetActionResult(new RPC_Result(false, "请输入敏感词"));
                }
                var r = Util.SMSProxy.AddKeywords(KeyGroup, list);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "敏感词", "添加", "敏感词组【" + KeyGroup + "】，敏感词【" + Words + "】，敏感词类型【" + KeywordsType + "】，是否启用【" + (Enable ? "是" : "否") + "】，替换为【" + ReplaceKeywords + "】", "添加", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new SMS.Model.RPCResult(false, "操作异常！"));
            }
        }

        public ActionResult DeleteKeywords(String KeywordsGroup, String Keywords)
        {
            try
            {
                var r = Util.SMSProxy.DelKeywords(KeywordsGroup, new List<string> { Keywords });
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "敏感词", "删除", "敏感词组【" + KeywordsGroup + "】，敏感词【" + Keywords + "】", "删除", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new SMS.Model.RPCResult(false, "操作异常！"));
            }
        }

        public ActionResult KeywordsEnable(String KeyGroup, String Words)
        {
            try
            {
                var r = Util.SMSProxy.KeywordsEnabled(KeyGroup, Words, true);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "敏感词", "启用", "敏感词组【" + KeyGroup + "】，敏感词【" + Words + "】", "启用", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new SMS.Model.RPCResult(false, "操作异常！"));
            }
        }
        public ActionResult KeywordsDisable(String KeyGroup, String Words)
        {
            try
            {
                var r = Util.SMSProxy.KeywordsEnabled(KeyGroup, Words, false);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "敏感词", "停用", "敏感词组【" + KeyGroup + "】，敏感词【" + Words + "】", "停用", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new SMS.Model.RPCResult(false, "操作异常！"));
            }
        }

        public ActionResult KeywordsTypeComboboxData()
        {
            SMS.Model.RPCResult<Dictionary<string, string>> r = Util.SMSProxy.GetKeywordsTypes();
            var q = from kg in r.Value select new { id = kg.Key, text = kg.Value };
            var row = q.ToList();
            return Content(JsonConvert.SerializeObject(row));
        }
        public ActionResult KeywordsTypeComboboxWithNullData()
        {
            SMS.Model.RPCResult<Dictionary<string, string>> r = Util.SMSProxy.GetKeywordsTypes();
            var q = from kg in r.Value select new { id = kg.Key, text = kg.Value };
            var row = q.ToList();
            row.Insert(0, new { id = "", text = " " });
            return Content(JsonConvert.SerializeObject(row));
        }
        #endregion

        #endregion

        #region 企业设置

        public ActionResult EnterpriseSetting()
        {
            if (string.IsNullOrWhiteSpace(Request["Parameter"]))
            {
                return Content("接收参数失败，请关闭页面重试！");
            }
            var param = Util.DeserializeParameter<FunctionParameter>(Request["Parameter"]);
            if (param == null)
            {
                return Content("解析参数失败，请关闭页面重试！");
            }
            var paramObject = new { AccountCode = param.EnterpriseLoginName, AccountName = param.EnterpriseName };
            ViewData["ParamObject"] = JsonConvert.SerializeObject(paramObject);
            return View();
        }

        #region 企业短信设置

        public ActionResult EnterpriseSMSSetting()
        {
            if (string.IsNullOrWhiteSpace(Request["ParamObject"]))
            {
                return Content("接收参数失败，请关闭页面重试！");
            }
            ViewData["ParamObject"] = Request["ParamObject"];
            return View();
        }

        public ActionResult GetAllChannel()
        {
            var r = new ResultSet<SMS.Model.Channel>();
            r.Value = new List<SMS.Model.Channel>();
            try
            {
                WebParamList wpl = new WebParamList(Request);
                var result = Util.SMSProxy.GetChannels();
                if (result != null && result.Success && result.Value != null)
                {
                    r.Total = result.Value.Count;
                    r.Value = result.Value;
                }

                return Content(r.ToString());
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return Content(r.ToString());
            }
        }
        public ActionResult GetChannelBySMSType()
        {
            var r = new ResultSet<SMS.Model.Channel>();
            r.Value = new List<SMS.Model.Channel>();
            try
            {
                WebParamList wpl = new WebParamList(Request);
                var result = Util.SMSProxy.GetChannels();
                if (result != null && result.Success && result.Value != null)
                {
                    r.Value = result.Value.Where(c => c.SMSType == Util.SMSType).ToList();
                    r.Total = r.Value.Count;
                }

                return Content(r.ToString());
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return Content(r.ToString());
            }
        }
        public ActionResult GetEnterpriseSMSSetting()
        {
            ResultSet<SMS.Model.EnterpriseUser> r = new ResultSet<SMS.Model.EnterpriseUser>();
            r.Value = new List<SMS.Model.EnterpriseUser>();
            try
            {
                WebParamList wpl = new WebParamList(Request);
                var result = Util.SMSProxy.GetEnterprise(wpl.str("AccountCode"));
                if (result != null && result.Success && result.Value != null)
                {
                    r.Value.Add(result.Value);
                    r.Total = r.Value.Count;
                }

                return Content(r.ToString());
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return Content(r.ToString());
            }
        }

        public ActionResult UpdateSMSSetting()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("SPNumber")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【扩展码号】！"));
                }

                var r = Util.SMSProxy.GetEnterprise(wpl.str("AccountCode"));
                if (r.Success)
                {
                    SMS.Model.EnterpriseUser user = r.Value;
                    if (user != null)
                    {

                        user.Channel = (wpl.str("Channel") == "-1-") ? "" : wpl.str("Channel");
                        user.FilterType = ushort.Parse(wpl.str("Filter"));
                        user.Signature = wpl.str("Signature");

                        user.Audit = (SMS.Model.AccountAuditType)(int.Parse(wpl.str("Audit")));
                        user.Priority = int.Parse(wpl.str("Priority"));
                        user.SPNumber = wpl.str("SPNumber");
                        user.IsOpen = (wpl.str("IsOpen") == "1") ? true : false;

                        user.StatusReport = (SMS.Model.StatusReportType)ushort.Parse(wpl.str("StatusReport"));
                       
                        SMS.Model.RPCResult rs = Util.SMSProxy.UpdateEnterpriseSMS(user);
                        if (rs.Success)
                        {
                            Util.SendSystemLogToISMP(Util.SMSProductName + "设置", "修改企业【" + wpl.str("AccountCode") + "】的短信设置", "", "修改短信设置", CurrentUser);
                        }
                        var ok = Util.SMSProxy.UpdateAccountSetting(user);
                        if (ok.Success)
                        {
                            Util.SendSystemLogToISMP(Util.SMSProductName + "设置", "修改企业【" + wpl.str("AccountCode") + "】的企业设置", "", "修改企业设置", CurrentUser);
                        }
                        return GetActionResult(ok);
                    }
                }

                return Error(r.Message);

            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        #endregion

        #region 密码重置

        public ActionResult EnterprisePasswordReset()
        {
            if (string.IsNullOrWhiteSpace(Request["ParamObject"]))
            {
                return Content("接收参数失败，请关闭页面重试！");
            }
            ViewData["ParamObject"] = Request["ParamObject"];
            return View();
        }

        public ActionResult ResetEnterprisePassword()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("Password")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【短信发送密码】！"));
                }

                var ok = Util.SMSProxy.ResetEnterprisePass(wpl.str("AccountCode"), wpl.str("Password").Trim());
                if (ok.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "发送密码设置", "修改企业【" + wpl.str("AccountCode") + "】的短信发送密码", "", "修改短信发送密码", CurrentUser);

                    return GetActionResult(new RPC_Result(true, "操作成功！"));
                }
                else
                {
                    return GetActionResult(ok);
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        #endregion

        #endregion

        #region 企业通道设置  批量

        public ActionResult EnterpriseChannelSettingList()
        {
            try
            {
                var result = Util.SMSProxy.GetChannels();
                if (result != null && result.Success && result.Value != null)
                {
                    var r = result.Value.Where(c => c.SMSType == Util.SMSType).Select(p => new { Id = p.ChannelID, Name = p.ChannelName }).ToArray();
                    ViewData["Channel"] = JsonSerialize.Instance.Serialize(r);
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                //不需返回错误
            }
            return View();
        }
        public ActionResult EnterpriseChannelSettingData(int page, int rows)
        {
            try
            {
                List<Dictionary<string, object>> enterList = GetSMSEnterpriseList();
                var eulist = Util.SMSProxy.ISMPGetEnterpriseBySMSType(Util.SMSType).Value;

                var resultlist = from ent in enterList
                                 join eu in eulist
                                 on ent["LoginName"].ToString().Split(',')[0] equals eu.AccountCode into tmp
                                 from t in tmp.DefaultIfEmpty()
                                 select new
                                 {
                                     LoginName = ent["LoginName"].ToString().Split(',')[0],
                                     Name = Convert.ToString(ent["Name"]),//.ToString(),
                                     Channel = t == null ? "" : t.Channel
                                 };

                //  var resultlist = new []{ new { LoginName = "", Name = "", Channel = "" } ,new { LoginName = "", Name = "", Channel = "" } }.AsEnumerable();

                string Channel = Request["Channel"];
                string Keywords = Request["Keywords"];

                if (!string.IsNullOrWhiteSpace(Channel))
                {
                    resultlist = resultlist.Where(e => e.Channel == Channel);
                }

                if (!string.IsNullOrWhiteSpace(Keywords))
                {
                    resultlist = resultlist.Where(e => e.Name.Contains(Keywords) || e.LoginName.Contains(Keywords));
                }

                return GetActionResult_Grid(PageQuery(resultlist, page, rows));

            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }


        public ActionResult UpdateEnterpriseChannel(string entloginnamelist, string channel)
        {
            try
            {
                var elist = entloginnamelist.Split(',');
                foreach (var e in elist)
                {
                    Util.SMSProxy.UpdateEnterpriseChannel(e, channel);
                }
                return GetActionResult(new RPC_Result(true, "操作完成！"));
            }
            catch (Exception ex)
            {
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }

        }
        #endregion

        #region 短信系统黑名单

        public ActionResult SMSSystemBlackList()
        {
            return View();
        }

        public ActionResult GetBlackList()
        {
            ResultSet<string> r = new ResultSet<string>();
            r.Value = new List<string>();
            try
            {
                WebParamList wpl = new WebParamList(Request);

                var smsTemplet = Util.SMSProxy.GetBlacklist();
                if (!smsTemplet.Success)
                {
                    return GetActionResult(smsTemplet);
                }

                if (smsTemplet.Value == null || smsTemplet.Value.Count <= 0) return Content(r.ToString());
                if (!string.IsNullOrWhiteSpace(wpl.str("Number")))
                {
                    smsTemplet.Value = smsTemplet.Value.Where(n => n.IndexOf(wpl.str("Number").Trim()) > -1).ToList();
                }

                r.Total = smsTemplet.Value.Count;
                r.Value = smsTemplet.Value;
                //分页
                if (r.Value != null && r.Value.Count > 0)
                {
                    r.Value = (from item in r.Value orderby item select item).ToList();
                    var index = (wpl.page - 1) * wpl.rows;
                    var range = (r.Value.Count > index) ? (r.Value.Count - index) : 0;
                    var count = (wpl.rows > range) ? range : wpl.rows;
                    r.Value = r.Value.GetRange(index, count);
                }

                return Content(r.ToString());
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return Content(r.ToString());
            }
        }

        public ActionResult AddBlackNumber()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (wpl.str("Number").Trim() == "")
                {
                    return GetActionResult(new RPC_Result(false, "无有效号码！"));
                }
                List<string> list = new List<string>(wpl.str("Number").Trim().Split(','));
                var r = Util.SMSProxy.AddBlacklist(list);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "系统黑名单", "添加系统黑名单号码", "添加黑名单号码为【" + wpl.str("Number") + "】", "添加系统黑名单", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult DelBlackNumber()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (wpl.str("Number").Trim() == "")
                {
                    return GetActionResult(new RPC_Result(false, "无有效号码！"));
                }
                List<string> list = new List<string>(wpl.str("Number").Trim().Split(','));
                var r = Util.SMSProxy.DelBlacklist(list);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "系统黑名单", "删除系统黑名单号码", "删除黑名单号码为【" + wpl.str("Number") + "】", "删除系统黑名单", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        #endregion

        #region 短信审核

        public ActionResult SendSMSAudit()
        {
            try
            {
                var result = Util.SMSProxy.GetChannels();
                if (result != null && result.Success && result.Value != null)
                {
                    var r = result.Value.Select(p => new { Id = p.ChannelID, Name = p.ChannelName }).ToArray();
                    ViewData["Channel"] = JsonSerialize.Instance.Serialize(r);
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                //不需返回错误
            }
            return View();
        }
        /// <summary>
        /// 获取审核失败原因列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAuditFailureReasonList()
        {
            try
            {
                var list = Util.SMSProxy.GetAuditFailureReasonList();
                if (!list.Success)
                {
                    return GetActionResult(list);
                }

                return Content(JsonConvert.SerializeObject(list.Value));
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "获取审核失败原因发生异常！"));
            }
        }

        public ActionResult GetSendSMSAuditList()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                wpl.add("SMSType", (int)Util.SMSType);
                var smslist = Util.SMSProxy.GetSMSForAudit(new SMS.Model.QueryParams() { ispage = true, page = wpl.page, rows = wpl.rows });
                if (!smslist.Success)
                {
                    return GetActionResult(smslist);
                }


                return Content(smslist.Value.ToString());
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "获取待审核短信发生异常！"));
            }
        }

        public ActionResult AuditSendSMSSuccess()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("SerialNumber")))//|| wpl.str("AccountID").Trim() == "" || wpl.str("Channel").Trim() == "")
                {
                    return GetActionResult(new RPC_Result(false, "无有效操作项！"));
                }

                List<string> list = wpl.str("SerialNumber").Trim().Split(';').ToList();
                var seriallist = (from s in list select s.Split(',')[1]).ToList();
                //审核短信
                var r = Util.SMSProxy.AuditSMSSuccess(CurrentUser.LoginName, seriallist, wpl.str("Channel"));
                if (r.Success)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        Util.SendSystemLogToISMP(Util.SMSProductName + "审核", "审核短信【通过】", "短信序列号【" + wpl.str("SerialNumber") + "】", "短信审核", CurrentUser);

                        ISMP_UpdateSendSMSAuditToDoStatus(list[i].Split(',')[1]);
                        ISMP_SendSendSMSAuditSuccessMsg(list[i].Split(',')[0]);
                    }

                    return GetActionResult(new RPC_Result(true, "操作成功！"));
                }
                else
                {

                    return GetActionResult(r);
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult AuditSendSMSFail()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("SerialNumber")))
                {
                    return GetActionResult(new RPC_Result(false, "无有效操作项！"));
                }
                List<string> list = new List<string>(wpl.str("SerialNumber").Trim().Split(';'));
                var seriallist = (from s in list select s.Split(',')[1]).ToList();

                var r = Util.SMSProxy.AuditSMSFailure(CurrentUser.LoginName, seriallist, wpl.str("AuditMsg"));
                if (r.Success)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        Util.SendSystemLogToISMP(Util.SMSProductName + "审核", "审核短信【不通过】", "企业账号【" + wpl.str("AccountID") + "】，短信序列号【" + wpl.str("SerialNumber") + "】", "短信审核", CurrentUser);
                        ISMP_UpdateSendSMSAuditToDoStatus(list[i].Split(',')[1]);

                        ISMP_SendSendSMSAuditFailToDoList(list[i].Split(',')[0], list[i].Split(',')[1]);

                    }
                    return GetActionResult(new RPC_Result(true, "操作成功！"));
                }
                else
                {
                    return GetActionResult(r);
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        private void ISMP_UpdateSendSMSAuditToDoStatus(string guid)
        {
            try
            {
                //通知ISMP,变更待处理任务状态
                string url = Util.ISMPHost + "/CallBack/SetToDoDealedByRowId?";
                url += "ProjectId=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductId)
                    + "&TableName=" + string.Empty
                    + "&RowId=" + System.Web.HttpUtility.UrlEncode(guid)
                    + "&ToDoType=" + "SMSSendSMSAudit" + Util.ProductSuffix;

                string resultISMP = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(resultISMP, new { success = true, message = string.Empty });
                if (!o.success)
                {
                    //不成功
                    Log4Logger.Error(resultISMP);
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
            }
        }

        private void ISMP_SendSendSMSAuditSuccessMsg(string accountId)
        {
            try
            {
                //通知ISMP，向短信发送人发送审核成功的消息
                SystemMessage model = new SystemMessage()
                {
                    RecieveAccountId = accountId,
                    RecieveName = "",
                    Title = "您发送的" + Util.SMSProductName + "已审核完成",
                    Message = "审核结果【审核通过】",
                    SenderAccountId = "System",
                    SenderName = "系统消息",
                    IsImportmant = true
                };

                string paramJson = JsonSerialize.Instance.Serialize<SystemMessage>(model);
                string url = Util.ISMPHost + "/CallBack/SendMsgToOneUser?";
                url += "Param=" + System.Web.HttpUtility.UrlEncode(paramJson);

                string resultISMP = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(resultISMP, new { success = true, message = string.Empty });
                if (!o.success)
                {
                    Log4Logger.Error(resultISMP);
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
            }
        }

        private void ISMP_SendSendSMSAuditFailToDoList(string accountId, string guid)
        {
            try
            {
                //通知ISMP，向短信发送人发送待处理任务
                string urlSMSEdit = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port + "/Enterprise/SMSAuditFailure";

                SystemToDoList std = new SystemToDoList();
                std.AccountId = accountId;
                std.CreateTime = DateTime.Now;
                std.Id = System.Guid.NewGuid().ToString();
                std.IsDealed = false;
                std.PageId = "ISMP_SendSendSMSAuditFailToDoList";
                std.PageTitle = "审核失败短信";
                std.Title = "您发送的" + Util.SMSProductName + "【审核不通过】。请重新编辑提交！";
                std.Url = "/Home/Transfer?url=" + urlSMSEdit + "&urlParam=/Common/GetBaseParam";
                std.ProjectId = Util.SMSProductId;
                std.TableName = "";
                std.RowId = guid;
                std.ToDoType = "SMSAuditFailure";

                string paramJson = JsonSerialize.Instance.Serialize<SystemToDoList>(std);
                string url = Util.ISMPHost + "/CallBack/SendToDoToOneUser?";
                url += "Param=" + System.Web.HttpUtility.UrlEncode(paramJson);

                string resultISMP = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(resultISMP, new { success = true, message = string.Empty });
                if (!o.success)
                {
                    //不成功
                    Log4Logger.Error(resultISMP);
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
            }
        }

        #endregion

        #region 短信质检（查询）

        public ActionResult SMSSendSMSList()
        {
            return View();
        }
        public ActionResult SMSSendSMSListData()
        {
            try
            {
                SMS.Model.QueryParams query = new SMS.Model.QueryParams();
                query.ispage = true;
                query.page = int.Parse(Request["page"]);
                query.rows = int.Parse(Request["rows"]);
                query.add("StartTime", Request["StartTime"]);
                query.add("EndTime", Request["EndTime"]);
                query.add("keywords", Request["keywords"]);
                query.add("Signature", Request["Signature"]);
                query.add("Channel", Request["Channel"]);
                query.add("SMSType", Util.SMSType);
                var smslist = Util.SMSProxy.GetSMSList(query);



                if (!smslist.Success)
                {
                    return GetActionResult(smslist);
                }
                var entlist = GetSMSEnterpriseList();

                var result = from sms in smslist.Value.Value
                             join e in entlist on sms.AccountID equals e["AccountId"] into entsmslist
                             from esms in entsmslist

                             select new
                             {
                                 EnterpriseName = esms["Name"],
                                 EnterpriseLoginName = esms["LoginName"],
                                 Content = sms.Content,
                                 Signature = sms.Signature,
                                 AccountID = sms.AccountID,
                                 ID = sms.ID,
                                 Status = sms.Status == "待审核" ? "待审核" : "已发送",
                                 NumberCount = sms.NumberCount,
                                 FailureCount = sms.FailureCount,
                                 FeeBack = sms.FeeBack,
                                 SendTime = sms.SendTime.Value.ToString(),
                                 SplitNumber = sms.SplitNumber,
                                 FeeTotalCount = sms.FeeTotalCount,
                                 AuditType = (sms.AuditType == SMS.Model.AuditType.Template) ? "模板匹配" : (sms.AuditType == SMS.Model.AuditType.Auto) ? "自动审核" : "人工审核",
                                 Channel = sms.Channel
                             };
                return Json(new { total = smslist.Value.Total, rows = result });
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult GetSMSNumbersBySMSID(string SMSID)
        {
            var list = Util.SMSProxy.GetSMSNumbersBySMSID(SMSID);
            if (list.Success)
            {
                return Content(list.Value);
            }
            else
            {
                return Content(list.Message);
            }
        }

        #endregion

        #region ISMP审核接口

        public ActionResult Audit()
        {
            if (string.IsNullOrWhiteSpace(Request["Parameter"]))
            {
                return Content("接收参数失败，请关闭页面重试！");
            }
            var param = Util.DeserializeParameter<AuditProductParameter>(Request["Parameter"]);
            if (param == null)
            {
                return Content("解析参数失败，请关闭页面重试！");
            }

            if (param.AuditType == Util.AuditType_TempletAudit)
            {
                Response.Redirect("/Platform/SMSTempletAudit?Identifier=" + param.Identifier + "&IsDisplayDetail=" + param.IsDisplayDetail);
            }
            else
            {
                return Content("解析参数失败，请关闭页面重试！");
            }

            return View();
        }

        #region 短信模版审核-ISMP审核中心

        public ActionResult SMSTempletAudit()
        {
            var isDisplayDetail = bool.Parse(Request["IsDisplayDetail"]);
            var result = GetAuditTempletByIdentifier(Request["Identifier"]);
            if (result == null)
            {
                return Content("获取审核项失败或审核项已被审核！");
            }
            if (isDisplayDetail)
            {
                ViewData["IsDisplayDetail"] = 1;
            }
            else
            {
                if (result.AuditState != SMS.Model.TempletAuditType.NoAudit)
                {
                    return Content("该审核项已审核完成，请关闭当前页面！");
                }
            }

            ViewData["Identifier"] = Request["Identifier"];
            ViewData["AccountCode"] = result.AccountCode;
            ViewData["TempletContent"] = result.TempletContent;
            ViewData["Signature"] = result.Signature;
            ViewData["SubmitTime"] = result.SubmitTime;

            return View();
        }

        public SMS.Model.SMSTemplet GetAuditTempletByIdentifier(string identifier)
        {
            try
            {
                DateTime startTime = DateTime.MinValue;
                DateTime endTime = DateTime.Now;

                var smsTemplet = Util.SMSProxy.GetAllSMSTemplet(startTime, endTime);//全部
                if (!smsTemplet.Success)
                {
                    return null;
                }

                if (smsTemplet.Value == null || smsTemplet.Value.Count <= 0) return null;
                foreach (var st in smsTemplet.Value)
                {
                    if (st.TempletID == identifier)
                        return st;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return null;
            }
        }

        public ActionResult AuditTempletResult()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("Status")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【审核结果】！"));
                }
                bool success = (wpl.str("Status") == "1") ? true : false;
                var result = GetAuditTempletByIdentifier(wpl.str("TempletID"));
                if (result == null)
                {
                    return GetActionResult(new RPC_Result(false, "获取审核项失败或审核项已被审核！"));
                }
                if (result.AuditState != SMS.Model.TempletAuditType.NoAudit)
                {
                    return GetActionResult(new RPC_Result(false, "该审核项已审核完成，请关闭当前页面！"));
                }
                var r = Util.SMSProxy.AuditSMSTemplet(wpl.str("TempletID"), CurrentUser.LoginName, success, wpl.str("AuditMsg"));
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "模版审核", "审核短信模版【" + (success ? "通过" : "不通过") + "】", "模版【" + wpl.str("TempletID") + "】", "短信模版审核", CurrentUser);
                    ISMP_SendSMSTempletAuditResultToDoList(wpl.str("TempletID"), success, wpl.str("AuditMsg"));
                }

                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        private void ISMP_SendSMSTempletAuditResultToDoList(string templetID, bool success, string auditMsg)
        {
            try
            {
                //通知ISMP
                string url = Util.ISMPHost + "/CallBack/AuditProductAuditRecord?";
                url += "Id=" + System.Web.HttpUtility.UrlEncode(templetID)
                    + "&ProductId=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductId)
                    + "&Type=" + System.Web.HttpUtility.UrlEncode(Util.AuditType_TempletAudit)
                    + "&Status=" + System.Web.HttpUtility.UrlEncode(((int)(success ? ProductAuditStatus.AuditSuccess : ProductAuditStatus.AuditFail)).ToString())
                    + "&AuditorAccountId=" + System.Web.HttpUtility.UrlEncode(CurrentUser.OperatorAccountId)
                    + "&Auditor=" + System.Web.HttpUtility.UrlEncode(CurrentUser.OperatorName)
                    + "&AuditMessage=" + System.Web.HttpUtility.UrlEncode(auditMsg);

                string resultISMP = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(resultISMP, new { success = true, message = string.Empty });
                if (!o.success)
                {
                    //通知不成功
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
            }
        }

        #endregion

        #endregion

        #region ISMP审核失败编辑重提接口

        public ActionResult AuditEdit()
        {
            if (string.IsNullOrWhiteSpace(Request["Parameter"]))
            {
                return Content("接收参数失败，请关闭页面重试！");
            }
            var param = Util.DeserializeParameter<AuditProductParameter>(Request["Parameter"]);
            if (param == null)
            {
                return Content("解析参数失败，请关闭页面重试！");
            }
            //switch (param.AuditType)
            //{
            //    case "短信-短信模版":
            //        Response.Redirect("/Enterprise/SMSTempletAuditFailEdit?Identifier=" + param.Identifier + "&AuditTargetAccountId="+param.AuditTargetAccountId);
            //        break;
            //    default:
            //        return Content("解析参数失败，请关闭页面重试！");
            //}
            if (param.AuditType == Util.AuditType_TempletAudit)
            {
                Response.Redirect("/Enterprise/SMSTempletAuditFailEdit?Identifier=" + param.Identifier + "&AuditTargetAccountId=" + param.AuditTargetAccountId);
            }
            else
            {
                return Content("解析参数失败，请关闭页面重试！");
            }
            return View();
        }

        #endregion


        #region 短信营收，发送，余额 统计

        public ActionResult SMSStatistics()
        {
            return View();
        }

        public ActionResult SMSStatisticsData(DateTime StartDate, DateTime EndDate, String keyword, int page, int rows)
        {
            if (StartDate == null)
            {
                return GetActionResult(new RPC_Result(false, "请选择开始日期"));
            }
            if (EndDate == null)
            {
                return GetActionResult(new RPC_Result(false, "请选择结束日期"));
            }
            var dd = Session["SMSStatisticsData"];
            dynamic resultdata = null;
            if (dd != null)
            {
                var ddStartDate = (DateTime)((dynamic)dd).StartDate;
                var ddEndDate = (DateTime)((dynamic)dd).EndDate;
                if (StartDate.Date.Equals(ddStartDate.Date) && EndDate.Date.Equals(ddEndDate.Date))
                {
                    resultdata = ((dynamic)dd).result;
                }
            }
            if (resultdata != null)
            {
                //根据name 过滤
                var result = (IEnumerable<dynamic>)resultdata;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    result = result.Where(s => s.Name.ToString().Contains(keyword) || s.EnterpriseCode.ToString().Contains(keyword));
                }
                var resultset = PageQuery(result, page, rows);
                var total = new
                {

                    TotalChargeCount = result.Sum(s => s.ChargeCount),
                    TotalChargeMoney = result.Sum(s => (decimal)s.ChargeMoney),
                    TotalSendCount = result.Sum(s => s.SendCount),
                    TotalSucceed = result.Sum(s => s.Succeed),
                    TotalBalance = result.Sum(s => s.Balance),
                    total = resultset.Total,
                    rows = resultset.Value
                };

                return Json(total);
            }
            else
            {
                var r = Util.SMSProxy.GetSMSStatisticsAll(StartDate, EndDate);
                var ao = new
                {
                    EnterpriseCode = string.Empty,
                    AccountId = string.Empty,
                    ChargeCount = 0,
                    ChargeMoney = Convert.ToDecimal(0),
                    SendCount = 0,
                    Succeed = 0,
                    Balance = 0
                };
                var statistics = (from ostr in r.Value select JsonConvert.DeserializeAnonymousType(ostr, ao)).ToList();

                List<Dictionary<string, object>> enterList = GetSMSEnterpriseList();

                var result = from e in enterList
                             join s in
                                 (from s0 in statistics
                                  group s0 by s0.AccountId into g
                                  select new
                                  {
                                      AccountId = g.Key,
                                      ChargeCount = g.Sum(t => t.ChargeCount),
                                      ChargeMoney = g.Sum(t => t.ChargeMoney),
                                      SendCount = g.Sum(t => t.SendCount),
                                      Succeed = g.Sum(t => t.Succeed),
                                      Balance = g.Sum(t => t.Balance)
                                  })
                             on e["AccountId"] equals s.AccountId into tmp
                             from t in tmp.DefaultIfEmpty()
                             select new
                             {
                                 EnterpriseCode = e["LoginName"],
                                 Name = e["Name"],
                                 AccountId = e["AccountId"],
                                 ChargeCount = t == null ? 0 : t.ChargeCount,
                                 ChargeMoney = t == null ? 0 : t.ChargeMoney,
                                 SendCount = t == null ? 0 : t.SendCount,
                                 Succeed = t == null ? 0 : t.Succeed,
                                 Balance = t == null ? 0 : t.Balance
                             };
                var data = new { StartDate = StartDate, EndDate = EndDate, result = result };

                Session["SMSStatisticsData"] = data;


                //根据name 过滤

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    result = result.Where(s => s.Name.ToString().Contains(keyword) || s.EnterpriseCode.ToString().Contains(keyword));
                }
                var resultset = PageQuery(result, page, rows);
                var total = new
                {

                    TotalChargeCount = result.Sum(s => s.ChargeCount),
                    TotalChargeMoney = result.Sum(s => s.ChargeMoney),
                    TotalSendCount = result.Sum(s => s.SendCount),
                    TotalSucceed = result.Sum(s => s.Succeed),
                    TotalBalance = result.Sum(s => s.Balance),
                    total = resultset.Total,
                    rows = resultset.Value
                };

                return Json(total);
            }

        }
        /// <summary>
        /// 查询所有短信企业
        /// </summary>
        /// <returns></returns>
        private static List<Dictionary<string, object>> GetSMSEnterpriseList()
        {
            //获取企业 
            string url = Util.ISMPHost + "/CallBack/GetEnterpriseByProduct?ProductId=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductId);

            string resonse = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
            List<Dictionary<string, object>> enterList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(resonse);
            return enterList;
        }
        #endregion

    }
}
