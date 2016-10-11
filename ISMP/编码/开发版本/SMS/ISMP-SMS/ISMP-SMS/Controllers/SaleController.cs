using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISMPInterface;
using Newtonsoft.Json;
using System.Configuration;
using Newtonsoft.Json.Linq;
using BXM.Logger;
using SMSPlatform.DAL;
using SMS.Model;
namespace ISMP_SMS.Controllers
{
    /// <summary>
    /// 代理商端使用
    /// </summary>
    public class SaleController : BaseController
    {

        #region 销售
        /// <summary>
        /// 短信销售接口
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            FunctionParameter fp = FunctionParameter;

            var eu = Util.SMSProxy.GetEnterprise(fp.EnterpriseLoginName);

            if (eu.Success)
            {
                return Content("一个企业只能开通一个短信产品，该企业"+fp.EnterpriseName + "已开通"+eu.Value.SMSType.ToString()+"短信产品，如需开通其他类短信产品，请建立新企业账号！");
            }
 
            ViewData["EnterpriseCode"] = fp.EnterpriseLoginName;
            ViewData["EnterpriseAccountID"] = fp.EnterpriseAccountId;
            ViewData["AgentAccountID"] = fp.AgentAccountId;

            return View();
        }

        public ActionResult SaleSubmit(SMS.Model.EnterpriseUser eu)
        {
            try
            {
                var sr = new SMS.Model.RPCResult(false, "");

                string enterpriseCode = Request["EnterpriseCode"];
                string enterpriseAccountID = Request["EnterpriseAccountID"];
                string agentAccountID = Request["AgentAccountID"];
                string Description = "企业新开充值";


                eu.AccountCode = enterpriseCode;
                eu.AccountID = enterpriseAccountID;
                eu.Name = "";
                eu.IsAgent = false;
                eu.IsOpen = false;
                eu.FilterType = (ushort)FilterType.Replace;
                eu.Audit = AccountAuditType.Audit;
                eu.SMSType = Util.SMSType;
                eu.StatusReport = StatusReportType.Disable;
                eu.Enabled = true;
                eu.RegisterDate = DateTime.Now;
                eu.Password = Util.GeneratePassword(8);//随机生成8位密码。
                eu.Channel = Util.DefaultChannel;
                eu.ParentAccountCode = "-1";//无上级企业
                string spNumber = Util.GenSpNumber();//随机算法生成
                var entlist = Util.SMSProxy.ISMPGetAllEnterprise().Value;
                //检验号码是否可用
                while (true)
                {
                    if (entlist.Any(e => e.SPNumber == spNumber))
                    {
                        spNumber = Util.GenSpNumber();//重新生成
                    }
                    else
                    {
                        break;
                    }
                }
                eu.SPNumber = spNumber;

                //检查企业是否已存在
                if (!entlist.Any(e => e.AccountCode == eu.AccountCode))
                {
                    //不存在，注册企业，不审核
                    sr = Util.SMSProxy.ISMPAddEnterprise(eu);
                    if (sr.Success)
                    {
                        try
                        {
                            //添加默认通讯录分组
                            bool resultAddContactGroup = PhoneAndGroupDB.GroupAdd(enterpriseCode, "0", "未分组");
                        }
                        catch (Exception ex)
                        {
                            Log4Logger.Error(ex);
                        }
                        //ISMP 订单
                        string url = Util.ISMPHost + "/CallBack/OpenProduct_CallBack?";
                        url += "Id=" + System.Web.HttpUtility.UrlEncode(System.Guid.NewGuid().ToString())
                            + "&EnterpriseAccountId=" + System.Web.HttpUtility.UrlEncode(enterpriseAccountID)
                            + "&ProductId=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductId)
                            + "&Description=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductName + "订单");

                        string result = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                        var o = JsonConvert.DeserializeAnonymousType(result, new { success = true, message = string.Empty });
                        if (!o.success)
                        {
                            //需要通知运维进行处理或再次尝试
                            Util.SendSystemLogToISMP(Util.SMSProductName + "开通", "短信中开通企业成功，回调ISMP添加订单失败", "企业AccountID【" + eu.AccountID + "】，企业登录名【" + eu.AccountCode + "】，添加订单失败原因【" + o.message + "】", "开通失败", CurrentUser);
                            return GetActionResult(new RPC_Result(false, "添加短信订单失败，请联系客服"));
                        }
                        else
                        {
                            Util.SendSystemLogToISMP(Util.SMSProductName + "开通", "短信中开通企业成功", "企业AccountID【" + eu.AccountID + "】，企业登录名【" + eu.AccountCode + "】", "开通产品", CurrentUser);
                        }
                    }
                    else
                    {
                        return GetActionResult(sr);
                    }
                }
                else
                {
                    return GetActionResult(new RPC_Result(false, "该企业已开通短信产品，不能重复开通！"));
                }

                var smsNumber = int.Parse(string.IsNullOrWhiteSpace(Request["smsNumber"]) ? "0" : Request["smsNumber"]);
                //开通同时给企业充值
                if (smsNumber > 0)
                {
                    ChargeRecord cr = new ChargeRecord();
                    cr.ChargeFlag = 0;
                    cr.Money = smsNumber * Util.SMSRate;
                    cr.SMSCount = smsNumber;
                    cr.ThenRate = Convert.ToDecimal(Util.SMSRate);
                    cr.OperatorAccount = CurrentUser.LoginName;
                    cr.PrepaidAccount = enterpriseCode;
                    cr.PrepaidTime = DateTime.Now;
                    cr.PrepaidType = 1;

                    //ISMP 扣费 
                    string url = Util.ISMPHost + "/CallBack/DeductForProduct?";
                    url += "DeductAccountId=" + System.Web.HttpUtility.UrlEncode(agentAccountID)
                        + "&RechargeAccountId=" + System.Web.HttpUtility.UrlEncode(enterpriseAccountID)
                        + "&Money=" + System.Web.HttpUtility.UrlEncode(Convert.ToString(cr.Money))
                        + "&Description=" + System.Web.HttpUtility.UrlEncode(Description)
                        + "&ProductPayType=" + System.Web.HttpUtility.UrlEncode("短信充值")
                        + "&ApplyAccountId=" + System.Web.HttpUtility.UrlEncode(CurrentUser.OperatorAccountId)
                        + "&ApplyName=" + System.Web.HttpUtility.UrlEncode(CurrentUser.OperatorName)
                        + "&Type=" + System.Web.HttpUtility.UrlEncode("21")
                        + "&ProductId=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductId);

                    string result = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                    var o = JsonConvert.DeserializeAnonymousType(result, new { success = true, message = string.Empty });
                    if (o.success)
                    {
                        var r = Util.SMSProxy.AccountPrepaid(cr);
                        if (r.Success)
                        {
                            Util.SendSystemLogToISMP(Util.SMSProductName + "充值", "开通完成，充值短信【" + smsNumber + "】条。", "企业AccountID【" + eu.AccountID + "】，企业登录名【" + eu.AccountCode + "】", "短信充值", CurrentUser);
                            return GetActionResult(new RPC_Result(true, "开通完成，充值短信【" + smsNumber + "】条。"));
                        }
                        else
                        {
                            //此处应记录日志和错误，并及时通知
                            //此处扣费成功但充值失败。

                            Util.SendSystemLogToISMP(Util.SMSProductName + "充值", "开通且扣费成功，充值失败", "企业AccountID【" + eu.AccountID + "】，企业登录名【" + eu.AccountCode + "】,充值失败原因【" + r.Message + "】", "短信充值失败", CurrentUser);
                            return GetActionResult(new RPC_Result(false, "开通且扣费成功，充值失败，请联系客服"));
                        }
                    }
                    else
                    {
                        Util.SendSystemLogToISMP(Util.SMSProductName + "开通", "注册完成，扣费失败", "企业AccountID【" + eu.AccountID + "】，企业登录名【" + eu.AccountCode + "】，金额【" + cr.Money + "】，扣费失败原因【" + o.message + "】", "开通", CurrentUser);
                        return GetActionResult(new RPC_Result(false, "注册完成，扣费失败，失败原因【" + o.message + "】"));
                    }

                }
                else
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "开通", "开通完成，充值短信【" + smsNumber + "】条", "企业AccountID【" + eu.AccountID + "】，企业登录名【" + eu.AccountCode + "】", "开通", CurrentUser);
                    return GetActionResult(new RPC_Result(true, "开通完成，充值短信【" + smsNumber + "】条。"));
                }

                //return GetActionResult(sr);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(true, "操作异常"));
            }
        }
        public ActionResult GetChannels()
        {
            var r = Util.SMSProxy.GetChannels();
            ISMPModel.ResultSet<Channel> rs = new ISMPModel.ResultSet<Channel>();
            rs.Value = r.Value;
            return Content(rs.ListToJson());
        }
        #endregion


        #region 充值

        public ActionResult Recharge()
        {

            EnterpriseProductRechargeParameter fp = BXM.Utils.JsonSerialize.Instance.Deserialize<EnterpriseProductRechargeParameter>(Request["Parameter"]);
            ViewData["EnterpriseCode"] = fp.EnterpriseLoginName;
            ViewData["EnterpriseAccountID"] = fp.EnterpriseAccountId;
            ViewData["AgentAccountID"] = fp.AgentAccountId;

            if (fp.IsGrant)
            {
                ViewData["IsGrant"] = "1";
                //加上key 用于防护前端hack
                string rand = System.Guid.NewGuid().ToString();
                ViewData["key"] = rand;
                Session["key"] = rand;
                Session["GrantReasonList"] = BXM.Utils.JsonSerialize.Instance.Serialize(fp.GrantReason); ;
            }
            return View();
        }

        public ActionResult GetGrantReasonList()
        {
            return Content(Convert.ToString(Session["GrantReasonList"]));
        }
        public ActionResult DoRecharge()
        {
            try
            {
                string enterpriseCode = Request["EnterpriseCode"];
                string enterpriseAccountID = Request["EnterpriseAccountID"];
                string agentAccountID = Request["AgentAccountID"];
                string Description = Request["Description"];

                string IsGrant = Request["IsGrant"];
                string Type = "22";
                string GrantType = "";
                if (IsGrant == "1")
                {
                    //验证key
                    string key = Request["key"];
                    var skey = Session["key"];
                    if (skey != null && skey.ToString() == key)
                    {
                        GrantType = Request["GrantType"];
                        Type = "2";
                    }
                    else
                    {
                        return GetActionResult(new RPC_Result(false, "充值失败，请重新登录后操作"));
                    }
                }

                if (string.IsNullOrWhiteSpace("enterpriseCode") || string.IsNullOrWhiteSpace("enterpriseAccountID") || string.IsNullOrWhiteSpace("agentAccountID"))
                {
                    return GetActionResult(new RPC_Result(false, "充值失败，缺少参数"));
                }


                var smsNumber = int.Parse(string.IsNullOrWhiteSpace(Request["smsNumber"]) ? "0" : Request["smsNumber"]);
                //给企业充值
                if (smsNumber > 0)
                {
                    ChargeRecord cr = new ChargeRecord();
                    cr.ChargeFlag = 0;
                    cr.Money = smsNumber * Util.SMSRate;
                    cr.SMSCount = smsNumber;
                    cr.ThenRate = Convert.ToDecimal(Util.SMSRate);
                    cr.OperatorAccount = CurrentUser.LoginName;
                    cr.PrepaidAccount = enterpriseCode;
                    cr.PrepaidTime = DateTime.Now;
                    cr.PrepaidType = 1;

                    //ISMP 扣费 
                    string url = Util.ISMPHost + "/CallBack/DeductForProduct?"
                        + "DeductAccountId=" + System.Web.HttpUtility.UrlEncode(agentAccountID)
                        + "&RechargeAccountId=" + System.Web.HttpUtility.UrlEncode(enterpriseAccountID)
                        + "&Money=" + System.Web.HttpUtility.UrlEncode(Convert.ToString(cr.Money))
                        + "&Description=" + System.Web.HttpUtility.UrlEncode(Description)
                        + "&ProductPayType=" + System.Web.HttpUtility.UrlEncode("短信充值")
                        + "&ApplyAccountId=" + System.Web.HttpUtility.UrlEncode(CurrentUser.OperatorAccountId)
                        + "&ApplyName=" + System.Web.HttpUtility.UrlEncode(CurrentUser.OperatorName)
                        + "&Type=" + System.Web.HttpUtility.UrlEncode(Type)
                        + "&ProductId=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductId);
                    if (IsGrant == "1")
                    {
                        url += "&GrantType=" + System.Web.HttpUtility.UrlEncode(GrantType);
                    }
                    string result = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                    var o = JsonConvert.DeserializeAnonymousType(result, new { success = true, message = string.Empty });
                    if (o.success)
                    {
                        var r = Util.SMSProxy.AccountPrepaid(cr);
                        if (r.Success)
                        {
                            Util.SendSystemLogToISMP(Util.SMSProductName + "充值", "充值短信【" + smsNumber + "】条。", "企业AccountID【" + enterpriseAccountID + "】，代理商AccountID【" + agentAccountID + "】，金额【" + cr.Money + "】", "充值", CurrentUser);
                            return GetActionResult(new RPC_Result(true, "充值成功,充值短信【" + smsNumber + "】条。"));
                        }
                        else
                        {
                            //此处应记录日志和错误，并及时通知
                            Util.SendAlertMessageByEmail(Util.SMSProductName + "产品充值扣费成功，充值失败：企业AccountID【" + enterpriseAccountID + "】，代理商AccountID【" + agentAccountID + "】，金额【" + cr.Money + "】，充值失败原因【" + r.Message + "】");
                            //此处扣费成功但充值失败。
                            Util.SendSystemLogToISMP(Util.SMSProductName + "充值", "扣费成功，充值失败", "企业AccountID【" + enterpriseAccountID + "】，代理商AccountID【" + agentAccountID + "】，金额【" + cr.Money + "】，充值失败原因【" + r.Message + "】", "充值", CurrentUser);
                            return GetActionResult(new RPC_Result(false, "给代理商扣费成功，充值失败，失败原因【" + r.Message + "】"));
                        }
                    }
                    else
                    {
                        Util.SendSystemLogToISMP(Util.SMSProductName + "充值", "扣费失败", "企业AccountID【" + enterpriseAccountID + "】，代理商AccountID【" + agentAccountID + "】，金额【" + cr.Money + "】，扣费失败原因【" + o.message + "】", "充值", CurrentUser);
                        return GetActionResult(new RPC_Result(false, "扣费失败，失败原因【" + o.message + "】"));
                    }
                }
                else
                {
                    return GetActionResult(new RPC_Result(false, "请输入正确的充值条数"));
                }

                //return View();
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常"));
            }
        }
        #endregion
    }
}
