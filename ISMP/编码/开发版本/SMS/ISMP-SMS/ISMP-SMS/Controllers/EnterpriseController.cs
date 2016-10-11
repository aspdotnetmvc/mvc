using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ISMPModel;
using System.Text;
using BXM.Utils;
using System.Data;
using System.IO;
using ISMPInterface;
using Newtonsoft.Json;
using BXM.Logger;
using SMSPlatform.DAL;

namespace ISMP_SMS.Controllers
{
    public class EnterpriseController : BaseController
    {
        #region 发送短信
        public ActionResult SendSMS()
        {
            string Numbers = Request["Numbers"];
            ViewData["Numbers"] = Numbers;

            var eu = Util.SMSProxy.GetEnterprise(FunctionParameter.EnterpriseLoginName);
            if (eu != null && eu.Success == true && !string.IsNullOrWhiteSpace(eu.Value.Signature))
            {
                ViewData["Signature"] = eu.Value.Signature;
            }
            else
            {
                ViewData["Signature"] = "";
            }


            return View();
        }
        public ActionResult DoSendSMS(string MobileNumbers, string UnicomNumbers, string TelecomNumbers, string Signature, DateTime? SMSTimer, string WapUrl)
        {
            try
            {
                //因为Content 是MVC里的基础方法
                string _Content = Request["Content"];

                //计算短信条数
                int smsSize = 0;

                //拆分号码
                var mobilelist = GetNumberList(MobileNumbers);
                var unicomlist = GetNumberList(UnicomNumbers);
                var telecomlist = GetNumberList(TelecomNumbers);

                SMS.Model.SMSMessage sms = new SMS.Model.SMSMessage();
                sms.ID = System.Guid.NewGuid().ToString();
                sms.Content = _Content;
                sms.Signature = '【' + Signature.TrimStart('【').TrimEnd('】') + '】';
                sms.SMSType = (int)Util.SMSType;
                sms.SplitNumber = SMS.Util.SMSSplit.GetSplitNumber(_Content, sms.Signature, out smsSize);
                sms.WapURL = WapUrl;
                sms.SMSTimer = SMSTimer;
                sms.NumberCount = mobilelist.Count + unicomlist.Count + telecomlist.Count;
                if (sms.NumberCount == 0)
                {
                    return GetActionResult(new RPC_Result(false, "待发送的号码数为0！"));
                }

                sms.FeeTotalCount = sms.NumberCount * sms.SplitNumber;
                sms.SendTime = DateTime.Now;

                SMS.DTO.SMSDTO dto = new SMS.DTO.SMSDTO();
                dto.Message = sms;

                dto.SMSNumbers = new List<SMS.Model.SMSNumber>();

                AddSMSNumber(dto, mobilelist, SMS.Model.OperatorType.mobile);
                AddSMSNumber(dto, unicomlist, SMS.Model.OperatorType.unicom);
                AddSMSNumber(dto, telecomlist, SMS.Model.OperatorType.telecom);

                var rs = Util.SMSProxy.SendSMS(CurrentUser.LoginName, CurrentUser.SMSPassword, dto, "平台");

                if (rs.Success)
                {
                    Util.SendSystemLogToISMP("发送" + Util.SMSProductName, "发送一条短信", "短信内容【" + _Content + "】号码数【" + sms.NumberCount + "个】，WapUrl【" + WapUrl + "】，定时发送【" + ((SMSTimer != null) ? "是" : "否") + "】，发送时间【" + (SMSTimer == null ? DateTime.Now : SMSTimer.Value) + "】", "发送短信", CurrentUser);
                    if (rs.Value.Message.AuditType == SMS.Model.AuditType.Manual)
                    {
                        //添加审核提醒
                        SendToDoToSMSAuditor(rs.Value.Message);
                    }
                }
                return GetActionResult(rs);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex.Message + "\r\n" + ex.StackTrace);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }
        /// <summary>
        /// 生成SMSNumber
        /// </summary>
        /// <param name="sms"></param>
        /// <param name="numbers"></param>
        /// <param name="operatorType"></param>
        private void AddSMSNumber(SMS.DTO.SMSDTO sms, List<string> numbers, SMS.Model.OperatorType operatorType)
        {
            if (numbers.Count > 0)
            {
                //按照大小限制拆分
                for (int i = 0; i * Util.MaxNumbersPerPackage < numbers.Count; i++)
                {
                    SMS.Model.SMSNumber sn = new SMS.Model.SMSNumber();
                    sn.SMSID = sms.Message.ID;
                    sn.ID = System.Guid.NewGuid().ToString();
                    sn.Operator = operatorType;
                    var t = numbers.Skip(i * Util.MaxNumbersPerPackage).Take(Util.MaxNumbersPerPackage).ToList();
                    sn.Numbers = string.Join(",", t);
                    sn.NumberCount = t.Count;
                    sms.SMSNumbers.Add(sn);
                }
            }
        }
        /// <summary>
        /// 预提交号码
        /// </summary>
        /// <param name="Numbers"></param>
        /// <param name="Signature"></param>
        /// <returns></returns>
        public ActionResult PreSubmitSMS(string Numbers, string Signature)
        {
            try
            {
                //因为Content 是MVC里的基础方法
                string _Content = Request["Content"];
                //判断商业短信中有无退订俩字，没有则不让提交
                if (Util.SMSType == SMS.Model.SMSType.商业)
                {

                    if (string.IsNullOrWhiteSpace(_Content))
                    {
                        return GetActionResult(new RPC_Result(false, "短信内容不能为空！"));
                    }

                    if (!_Content.Contains("退订"))
                    {
                        return GetActionResult(new RPC_Result(false, "商业短信内容结尾必须包含 <退订回T>"));
                    }
                }
                string _Signature = "【" + Signature + "】";
                //计算短信条数
                int SmsSplitCount = 0;
                int smsSize = 0;

                SmsSplitCount = SMS.Util.SMSSplit.GetSplitNumber(_Content, _Signature, out smsSize);

                //拆分号码
                var numlist = GetNumberList(Numbers);
                //错误号码
                var errnum = numlist.Where(n => !Util.ValidMobile(n)).ToList();
                var validNumbers = numlist.Where(n => Util.ValidMobile(n)).ToList();
                var dic = SMS.Util.OperatorHelper.GroupNumbersByOperator(validNumbers.ToList(), Util.SMSProxy.GetNumSect().Value);
                var SendContent = _Content;
                if (Util.SMSType == SMS.Model.SMSType.行业)
                {
                    SendContent = _Content + _Signature;
                }
                else
                {
                    SendContent = _Signature + _Content;
                }

                bool haskeywords = false;
                string keywordslist = "";
                var r = Util.SMSProxy.GetAllKeywords();
                if (r.Success)
                {
                    var list = from k in r.Value where SendContent.IndexOf(k.Words) >= 0 select k.Words;
                    if (list.Any())
                    {
                        haskeywords = true;
                        keywordslist = string.Join(",", list);
                    }
                }
                var result = new
                {
                    success = true,
                    TotalCount = numlist.Count,
                    ErrorCount = errnum.Count,
                    ErrorNumbers = errnum.ToList(),
                    MobileCount = dic.ContainsKey(SMS.Model.OperatorType.mobile) ? dic[SMS.Model.OperatorType.mobile].Count : 0,
                    MobileNumbers = dic.ContainsKey(SMS.Model.OperatorType.mobile) ? dic[SMS.Model.OperatorType.mobile] : new List<string>(),
                    TelecomCount = dic.ContainsKey(SMS.Model.OperatorType.telecom) ? dic[SMS.Model.OperatorType.telecom].Count : 0,
                    TelecomNumbers = dic.ContainsKey(SMS.Model.OperatorType.telecom) ? dic[SMS.Model.OperatorType.telecom] : new List<string>(),
                    UnicomCount = dic.ContainsKey(SMS.Model.OperatorType.unicom) ? dic[SMS.Model.OperatorType.unicom].Count : 0,
                    UnicomNumbers = dic.ContainsKey(SMS.Model.OperatorType.unicom) ? dic[SMS.Model.OperatorType.unicom] : new List<string>(),
                    NotDefinedCount = dic.ContainsKey(SMS.Model.OperatorType.notdefined) ? dic[SMS.Model.OperatorType.notdefined].Count : 0,
                    NotDefinedNumbers = dic.ContainsKey(SMS.Model.OperatorType.notdefined) ? dic[SMS.Model.OperatorType.notdefined] : new List<string>(),
                    SmsSplitCount = SmsSplitCount, //拆分条数
                    SendContent = SendContent,  //实际发送内容
                    HasKeywords = haskeywords,  //是否有敏感词
                    KeywordsList = keywordslist  //敏感词列表
                };
                return Content(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "提交短信时发生异常"));
            }
        }

        private List<string> GetNumberList(string Numbers)
        {
            if (string.IsNullOrWhiteSpace(Numbers))
            {
                return new List<string>();
            }
            var templist = Numbers.Replace("\r\n", ",").Replace("\n", ",").Split(',').Where(n => !string.IsNullOrWhiteSpace(n));
            var numlist = from n in templist select n.Trim();
            return numlist.ToList();
        }

        private void CheckErrorNumber(List<string> numlist, ISMPModel.RPC_Result r)
        {
            string ErrorNumbers = "";
            //判断手机号是否有效
            if (!numlist.All(n => Util.ValidMobile(n)))
            {

                var errnum = numlist.Where(n => !Util.ValidMobile(n)).ToList();
                //把出现错误的号码写到Session里，用于获取错误号码,暂未做
                ErrorNumbers = string.Join(",", errnum);
                Session["ErrorNumbers"] = ErrorNumbers;
                r.Success = false;

                if (errnum.Count > 5)//有超过5 个错误号码 
                {
                    r.Message = "存在 " + string.Join(",", errnum.GetRange(0, 5)) + "等 共 " + errnum.Count + " 个错误号码！";
                    r.ErrorCode = 10;
                }
                else
                {
                    r.Message = "存在 " + ErrorNumbers + "错误号码！";
                    r.ErrorCode = 1;
                }
            }
        }
        /// <summary>
        /// 给审核人发送一条消息
        /// </summary>
        /// <param name="audit"></param>
        public void SendToDoToSMSAuditor(SMS.Model.SMSMessage sms)
        {
            try
            {

                string urlSMSEdit = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port + "/Platform/SendSMSAudit";

                SystemToDoList std = new SystemToDoList();
                std.CreateTime = DateTime.Now;
                std.Id = System.Guid.NewGuid().ToString();
                std.IsDealed = false;
                std.PageId = "SMSSendSMSAudit" + Util.ProductSuffix;
                std.PageTitle = Util.SMSProductName + "审核";
                std.Title = "您的" + Util.SMSProductName + "审核中有新的【" + Util.SMSProductName + "审核】申请，请及时审核！";
                std.Url = "/Home/Transfer?url=" + urlSMSEdit + "&urlParam=/Common/GetBaseParam";
                std.ProjectId = Util.SMSProductId;
                std.TableName = "";
                std.RowId = sms.ID;
                std.ToDoType = "SMSSendSMSAudit" + Util.ProductSuffix;

                string Param = JsonSerialize.Instance.Serialize<SystemToDoList>(std);

                string url = Util.ISMPHost + "/CallBack/SendToDoToOneGroupByPermission?";
                url += "Param=" + System.Web.HttpUtility.UrlEncode(Param)
                    + "&Identifier=" + System.Web.HttpUtility.UrlEncode(std.ToDoType);


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
        public ActionResult CheckKeywords(string Content)
        {
            try
            {
                var r = Util.SMSProxy.GetAllKeywords();
                if (r.Success)
                {
                    var list = from k in r.Value where Content.IndexOf(k.Words) >= 0 select k.Words;
                    if (list.Any())
                    {
                        string keywordslist = string.Join(",", list);
                        return GetActionResult(new RPC_Result(false, keywordslist));
                    }
                }
                return GetActionResult(new RPC_Result(true, ""));
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult CheckErrorNumbers(string Numbers)
        {

            try
            {
                var r = new RPC_Result(true, "没有错误号码");
                var numlist = GetNumberList(Numbers);
                CheckErrorNumber(numlist, r);
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "检查错误号码时发生了异常！"));
            }
        }



        public FileResult GetErrorNumbers()
        {
            string ErrorNumber = Convert.ToString(Session["ErrorNumbers"]);

            return File(Encoding.Default.GetBytes(ErrorNumber), "text/plain");
        }
        public FileResult GetFailNumbers()
        {
            string FailNumbers = Convert.ToString(Session["FailNumbers"]);

            return File(Encoding.Default.GetBytes(FailNumbers), "text/plain");
        }
        public FileResult GetNumbersFile()
        {
            string Numbers = Request["Numbers"];
            return File(Encoding.Default.GetBytes(Numbers), "text/plain");
        }
        #endregion

        #region 短信发送记录查询

        public ActionResult SMSView()
        {
            ViewData["EnterpriseAccountId"] = FunctionParameter.EnterpriseAccountId;
            ViewData["EnterpriseCode"] = FunctionParameter.EnterpriseLoginName;
            ViewData["EnterpriseName"] = FunctionParameter.EnterpriseName;
            return View();
        }
        public ActionResult SMSViewData()
        {
            try
            {
                string AccountID = Request["EnterpriseAccountId"];
                if (string.IsNullOrWhiteSpace(AccountID))
                {
                    return GetActionResult(new RPC_Result(false, "查询账号为空，权限不足！"));
                }
                SMS.Model.QueryParams query = new SMS.Model.QueryParams();
                query.ispage = true;
                query.page = int.Parse(Request["page"]);
                query.rows = int.Parse(Request["rows"]);
                query.add("StartTime", Request["StartTime"]);
                query.add("EndTime", Request["EndTime"]);
                query.add("keywords", Request["keywords"]);
                var r = Util.SMSProxy.GetSMSListByAccountID(AccountID, query);

                if (r.Success)
                {
                    var result = from sms in r.Value.Value
                                 select new
                                   {
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
                                       AuditType = (sms.AuditType == SMS.Model.AuditType.Template) ? "模板匹配" : (sms.AuditType == SMS.Model.AuditType.Auto) ? "自动审核" : "人工审核"
                                   };
                    return Json(new { total = r.Value.Total, rows = result });
                }
                else
                {
                    return GetActionResult(r);
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "查询时发生了异常"));
            }
        }


        #endregion

        #region 状态报告
        public ActionResult SMSStatus()
        {
            ViewData["EnterpriseCode"] = FunctionParameter.EnterpriseLoginName;
            ViewData["EnterpriseName"] = FunctionParameter.EnterpriseName;
            return View();
        }
        /// <summary>
        /// 查询3天内状态报告
        /// </summary>
        /// <returns></returns>
        public ActionResult SMSStatusData()
        {
            string EnterpriseCode = Request["EnterpriseCode"];

            var begindate = DateTime.Now.AddDays(-2).Date;
            var enddtate = DateTime.Now.Date.AddMilliseconds(24 * 3600 * 1000 - 1);

            SMS.Model.RPCResult<List<SMS.Model.ReportStatistics>> r = Util.SMSProxy.GetStatisticsReportByAccount(EnterpriseCode, begindate, enddtate);
            var list = r.Value;
            if (r.Success)
            {
                string number = Request["Number"];

                int rows = int.Parse(Request["rows"]);
                int page = int.Parse(Request["page"]);

                var rs = PageQuery(list.OrderByDescending(p => p.SendTime), page, rows);

                return GetActionResult_Grid(rs);
            }
            else
            {
                return GetActionResult(r);
            }

        }

        public ActionResult SMSStatusMoreData(String SerialNumber, DateTime SendTime)
        {


            string Status = Request["Status"];
            string Number = Request["Number"];
            SMS.Model.QueryParams qp = new SMS.Model.QueryParams();
            qp.rows = int.Parse(Request["rows"]);
            qp.page = int.Parse(Request["page"]);
            qp.add("SMSID", SerialNumber);
            qp.add("SendTime", SendTime.ToString("yyyyMMdd"));
            if (Status == "success")
            {
                qp.add("Succeed", "1");
            }
            else if (Status == "fail")
            {
                qp.add("Succeed", "0");
            }

            if (!string.IsNullOrWhiteSpace(Number))
            {
                qp.add("Number", Number);
            }
            var rs = Util.SMSProxy.GetStatusReportBySMSID(qp);
            if (rs.Success)
            {
                var rs2 = new
                {
                    total=rs.Value.Total,
                    rows = (from sr in rs.Value.Value
                            select  new
                            {
                                AccountID=sr.AccountID,
                                Channel=sr.Channel,
                                Description=sr.Description,
                                Gateway=sr.Gateway,
                                Number=sr.Number,
                                ResponseTime=sr.ResponseTime.ToString(),
                                SendTime=sr.SendTime.ToString(),
                                SerialNumber=sr.SerialNumber,
                                SMSID=sr.SMSID,
                                Status = getStatus(sr.StatusCode)
                            }).ToArray()
                };

                return Content(JsonConvert.SerializeObject(rs2));
            }
            else
            {
                return GetActionResult(rs);
            }

        }
        /// <summary>
        /// 对状态进行加工
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string getStatus(int code)
        {
            if (code % 1000 == 0)
                return "提交成功";
            if (code % 1000 == 100)
                return "发送成功";
            return "发送失败";
        }

        public ActionResult SMSStatusHistory()
        {
            ViewData["EnterpriseCode"] = FunctionParameter.EnterpriseLoginName;
            ViewData["EnterpriseName"] = FunctionParameter.EnterpriseName;
            return View();
        }
        public ActionResult SMSStatusHistoryData(DateTime StartTime, DateTime EndTime)
        {
            string EnterpriseCode = Request["EnterpriseCode"];
            try
            {
                if (DateTime.Compare(StartTime, EndTime) >= 0)
                {
                    return GetActionResult(new RPC_Result(false, "开始时间应小于结束时间"));
                }
                SMS.Model.RPCResult<List<SMS.Model.ReportStatistics>> r = Util.SMSProxy.GetStatisticsReportByAccount(EnterpriseCode, StartTime, EndTime);
                if (r.Success)
                {
                    string number = Request["Number"];
                    int rows = int.Parse(Request["rows"]);
                    int page = int.Parse(Request["page"]);
                    var list = r.Value;

                    var rs = PageQuery(list, page, rows);
                    return GetActionResult_Grid(rs);
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
        #endregion

        #region 接收短信
        /// <summary>
        /// 接收短信
        /// </summary>
        /// <returns></returns>
        public ActionResult SMSMoList()
        {
            ViewData["EnterpriseCode"] = FunctionParameter.EnterpriseLoginName;
            ViewData["EnterpriseName"] = FunctionParameter.EnterpriseName;
            return View();
        }
        public ActionResult SMSMoListData(DateTime StartTime, DateTime? EndTime)
        {

            string EnterpriseCode = Request["EnterpriseCode"];
            //  var Eu = Util.SMSProxy.GetEnterprise(EnterpriseCode);
            #region ZKD 方式读取上行短信
            //SMS.Model.RPCResult<List<SMS.Model.MOSMS>> mo = Util.SMSProxy.GetDirectMOSmsBySPNumber(EnterpriseCode, Eu.Value.Password);

            //if (mo.Success)
            //{
            //    List<SMS.Model.MOSMS> mm = mo.Value;
            //    if (mm.Count > 0)
            //    {
            //        foreach (var s in mm)
            //        {
            //            BLL.MO.Add(s);
            //        }
            //    }
            //}
            //else
            //{
            //    return GetActionResult(mo);
            //}

            //var r = BLL.MO.Gets(Eu.Value.SPNumber, StartTime, EndTime == null ? DateTime.Now : EndTime.Value);
            #endregion

            #region 中呼端方式读取上行短信

            var r = Util.SMSProxy.GetMOSMS(EnterpriseCode, StartTime, EndTime == null ? DateTime.Now : EndTime.Value);
            if (!r.Success)
            {
                return GetActionResult(r);
            }

            #endregion
            int rows = int.Parse(Request["rows"]);
            int page = int.Parse(Request["page"]);

            var rs = PageQuery(r.Value.OrderByDescending(p => p.ReceiveTime), page, rows);
            return GetActionResult_Grid(rs);
        }

        #endregion

        #region 审核失败短信

        public ActionResult SMSAuditFailure()
        {
            ViewData["EnterpriseAccountId"] = FunctionParameter.EnterpriseAccountId;
            ViewData["EnterpriseCode"] = FunctionParameter.EnterpriseLoginName;
            ViewData["EnterpriseName"] = FunctionParameter.EnterpriseName;
            return View();
        }
        public ActionResult SMSAuditFailureData()
        {
            try
            {
                string AccountID = Request["EnterpriseAccountID"];
                if (string.IsNullOrWhiteSpace(AccountID))
                {
                    return GetActionResult(new RPC_Result(false, "账号空"));
                }
                SMS.Model.QueryParams qp = new SMS.Model.QueryParams() { ispage = true };
                qp.rows = int.Parse(Request["rows"]);
                qp.page = int.Parse(Request["page"]);
                qp.add("AccountID", AccountID);
                qp.add("AuditResult", "2");
                var r = Util.SMSProxy.GetSMSByAudit(qp);

                if (r.Success)
                {
                    return Content(r.Value.ToString());
                }
                else
                {
                    return GetActionResult(r);
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "查询时发生了异常"));
            }
        }

        public ActionResult ReSendSMS(string SerialNumber, string Numbers, string Content, DateTime? SMSTimer, string WapUrl)
        {
            return GetActionResult(new RPC_Result(false, "系统调整后，该功能尚未实现"));
        }

        private void ISMP_UpdateSendSMSAuditFailToDoStatus(string guid)
        {
            try
            {
                //通知ISMP,变更待处理任务状态为已处理
                string url = Util.ISMPHost + "/CallBack/SetToDoDealedByRowId?";
                url += "ProjectId=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductId)
                    + "&TableName=" + string.Empty
                    + "&RowId=" + System.Web.HttpUtility.UrlEncode(guid)
                    + "&ToDoType=" + "SMSAuditFailure";

                string resultISMP = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(resultISMP, new { success = true, message = string.Empty });
                if (!o.success)
                {
                    //不成功
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
            }
        }

        #endregion

        #region 短信模版管理

        public ActionResult SMSTemplet()
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
            var paramObject = new { AccountCode = param.EnterpriseLoginName, AccountName = param.EnterpriseName, AccountId = param.EnterpriseAccountId };
            ViewData["ParamObject"] = JsonConvert.SerializeObject(paramObject);
            if (Util.SMSType == SMS.Model.SMSType.商业 || Util.SMSType == SMS.Model.SMSType.四大类)
            {
                ViewData["Example"] = "模版举例：【云径增值测试】感谢您试用云径增值测试，我们致力于为所有用户提供优质、高效的全网销售工具。详情咨询400-9955-888.退订回T";
            }
            else
            {
                ViewData["Example"] = "模板举例：亲爱的会员******，您好。您本次登录的密码是******，*分钟内有效。";
            }

            return View();
        }

        public ActionResult AddTemplet()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                var r = Util.SMSProxy.AddSMSTemplet(wpl.str("AccountCode"), wpl.str("TempletContent"));
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "模版管理", "添加一个短信模版", "企业【" + wpl.str("AccountCode") + "】，模版【" + r.Value.ToString() + "】", "添加短信模版", CurrentUser);
                    if (String.IsNullOrWhiteSpace(wpl.str("Description")))
                    {
                        wpl.add("Description", "审核企业【" + wpl.str("AccountCode") + "】的" + Util.SMSProductName + "模版");
                    }
                    ISMP_SendSMSTempletAuditToDoList(r.Value.ToString("N"), wpl.str("Description"), wpl.str("AccountId"));
                }

                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        private void ISMP_SendSMSTempletAuditToDoList(string guid, string description, string auditTargetAccountId)
        {
            try
            {
                //通知ISMP
                string url = Util.ISMPHost + "/CallBack/AddProductAuditRecord?";
                url += "Id=" + System.Web.HttpUtility.UrlEncode(guid)
                    + "&ProductId=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductId)
                    + "&Type=" + System.Web.HttpUtility.UrlEncode(Util.AuditType_TempletAudit)
                    + "&Description=" + System.Web.HttpUtility.UrlEncode(description)
                    + "&ApplyAccountId=" + System.Web.HttpUtility.UrlEncode(CurrentUser.OperatorAccountId)
                    + "&ApplyName=" + System.Web.HttpUtility.UrlEncode(CurrentUser.OperatorName)
                    + "&AuditTargetAccountId=" + System.Web.HttpUtility.UrlEncode(auditTargetAccountId);

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

        public ActionResult GetTempletList()
        {
            var r = new ResultSet<SMS.Model.SMSTemplet>();
            r.Value = new List<SMS.Model.SMSTemplet>();
            try
            {
                WebParamList wpl = new WebParamList(Request);
                var smsTemplet = Util.SMSProxy.GetZKDSMSTempletStauts(wpl.str("AccountCode"));
                if (!smsTemplet.Success)
                {
                    return GetActionResult(smsTemplet);
                }

                if (smsTemplet.Value == null) smsTemplet.Value = new List<SMS.Model.SMSTemplet>();
                r.Total = smsTemplet.Value.Count;
                r.Value = smsTemplet.Value;
                //分页
                if (r.Value != null && r.Value.Count > 0)
                {
                    r.Value = (from item in r.Value orderby item.SubmitTime descending select item).ToList();
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
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult DelTemplet()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                var r = Util.SMSProxy.ZKDDelSMSTemplet(wpl.str("TempletID"));
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "模版管理", "删除一个短信模版", "企业【" + wpl.str("AccountCode") + "】，模版【" + wpl.str("TempletID") + "】", "删除短信模版", CurrentUser);
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

        #region 短信模版审核失败编辑重提

        public ActionResult SMSTempletAuditFailEdit()
        {
            if (string.IsNullOrWhiteSpace(Request["Identifier"]) || string.IsNullOrWhiteSpace(Request["AuditTargetAccountId"]))
            {
                return Content("接收参数失败，请关闭页面重试！");
            }
            var result = GetAuditTempletByIdentifier(Request["Identifier"]);
            if (result == null)
            {
                return Content("获取审核项失败！");
            }
            ISMP_UpdateSMSTempletAuditFailToDoStatus(result);
            if (result.AuditState == SMS.Model.TempletAuditType.NoAudit)
            {
                return Content("该审核项尚未审核，请关闭当前页面！");
            }
            var paramObject = new { AccountCode = result.AccountCode, TempletContent = result.TempletContent, AuditMessage = result.Remark, AccountId = Request["AuditTargetAccountId"] };
            ViewData["ParamObject"] = JsonConvert.SerializeObject(paramObject);
            return View();
        }

        private void ISMP_UpdateSMSTempletAuditFailToDoStatus(SMS.Model.SMSTemplet st)
        {
            try
            {
                //通知ISMP置当前待处理项为已处理
                string url = Util.ISMPHost + "/CallBack/SetToDoDealedByRowId?";
                url += "ProjectId=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductId)
                    + "&TableName=" + System.Web.HttpUtility.UrlEncode("")
                    + "&RowId=" + System.Web.HttpUtility.UrlEncode(st.TempletID)
                    + "&ToDoType=" + System.Web.HttpUtility.UrlEncode(Util.AuditType_TempletAudit);

                string result = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(result, new { success = true, message = string.Empty });
                if (!o.success)
                {
                    //不成功
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
            }
        }

        private SMS.Model.SMSTemplet GetAuditTempletByIdentifier(string identifier)
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

        #endregion

        #region 终端企业短信使用状态、统计分析

        /// <summary>
        /// 短信ISMP 终端企业短信使用状态
        /// </summary>
        /// <returns></returns>
        public ActionResult EnterpriseSMSStatus()
        {
            if (string.IsNullOrWhiteSpace(Request["Parameter"]))
            {
                return GetActionResult(new RPC_Result(false, "接收参数失败"));
            }
            var param = Util.DeserializeParameter<FunctionParameter>(Request["Parameter"]);
            if (param == null)
            {
                return GetActionResult(new RPC_Result(false, "解析参数失败"));
            }

            try
            {
                return GetEnterpriseProductOrderDetail(param);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "异常"));
            }
        }

        /// <summary>
        /// 短信ISMP 产品详单
        /// </summary>
        /// <returns></returns>
        private ActionResult GetEnterpriseProductOrderDetail(FunctionParameter param)
        {
            try
            {
                var r = Util.SMSProxy.GetChargeStatics(param.EnterpriseLoginName);
                if (!r.Success)
                {
                    return GetActionResult(r);
                }

                if (r.Value == null || r.Value.Count <= 0) return GetActionResult(new RPC_Result(false, "不存在数据！"));

                //var list = smsTemplet.Value;
                //过滤
                var list = r.Value.Where(p => p.Enterprese == param.EnterpriseLoginName).ToList();
                if (list == null || list.Count <= 0) return GetActionResult(new RPC_Result(false, "不存在数据！"));
                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("充值总额", list[0].TotalMoney.ToString() + "元");
                d.Add("短信总量", list[0].SMSCount.ToString() + "条");
                d.Add("短信用量", list[0].SendCount.ToString() + "条");
                d.Add("短信余量", list[0].RemainSMSNumber.ToString() + "条");

                return Content(JsonSerialize.Instance.Serialize(d));
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "异常"));
            }
        }

        /// <summary>
        /// 短信ISMP 终端企业短信统计分析
        /// </summary>
        /// <returns></returns>
        public ActionResult EnterpriseSMSHomePageStatistics()
        {
            if (string.IsNullOrWhiteSpace(Request["Parameter"]))
            {
                return GetActionResult(new RPC_Result(false, "接收参数失败"));
            }
            var param = Util.DeserializeParameter<FunctionParameter>(Request["Parameter"]);
            if (param == null)
            {
                return GetActionResult(new RPC_Result(false, "解析参数失败"));
            }

            try
            {
                switch (FunctionParameter.UserType)
                {
                    case UserType.Enterprise:
                        return GetEnterpriseCurrentPeriodStatistic(param);
                    case UserType.Agent:
                    case UserType.AgentEmployee:
                        return GetAgentCurrentMonthStatistic(param);
                    default:
                        return GetActionResult(new RPC_Result(false, "无对应当前用户类型统计"));
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "异常"));
            }
        }

        /// <summary>
        /// 短信ISMP 本期统计（最后一个周期统计：上期结余、本期总量、本期用量）
        /// </summary>
        /// <returns></returns>
        private ActionResult GetEnterpriseCurrentPeriodStatistic(FunctionParameter param)
        {
            try
            {
                var smsTemplet = Util.SMSProxy.GetChargeStatics(param.EnterpriseLoginName);
                if (!smsTemplet.Success)
                {
                    return GetActionResult(smsTemplet);
                }

                var rechargeRecord = Util.SMSProxy.GetEnterpriseChargeRecord(param.EnterpriseLoginName, DateTime.MinValue, DateTime.MaxValue);
                if (!rechargeRecord.Success)
                {
                    return GetActionResult(rechargeRecord);
                }

                //var sendSMSReport = Util.SMSProxy.GetDirectStatisticReportByAccount(param.EnterpriseLoginName);
                //if (!sendSMSReport.Success)
                //{
                //    return GetActionResult(sendSMSReport);
                //}

                if (smsTemplet.Value == null || smsTemplet.Value.Count <= 0 || rechargeRecord.Value == null || rechargeRecord.Value.Count <= 0)
                {
                    return GetActionResult(new RPC_Result(false, "不存在数据！"));
                }

                //最近一条记录
                var currentRecharge = (from record in rechargeRecord.Value orderby record.PrepaidTime descending select record).FirstOrDefault();
                if (null == currentRecharge)
                {
                    return GetActionResult(new RPC_Result(false, "不存在充值记录！"));
                }

                //最后一次充值前发送失败，返充的条数
                //int sendSMSFailCount = (sendSMSReport.Value == null || sendSMSReport.Value.Count <= 0) ? 0 :
                //    (from reportItem in sendSMSReport.Value where (reportItem.SendTime < currentRecharge.PrepaidTime && reportItem.LastResponseTime >= currentRecharge.PrepaidTime) select reportItem.FailureCount).Sum();

                //var list = smsTemplet.Value;
                //过滤
                var list = smsTemplet.Value.Where(p => p.Enterprese == param.EnterpriseLoginName).ToList();
                if (list == null || list.Count <= 0) return GetActionResult(new RPC_Result(false, "不存在数据！"));
                var LastTotal = ((currentRecharge.RemainSMSCount.HasValue ? currentRecharge.RemainSMSCount.Value : 0));//上期结余(充值时剩余的条数+充值前发送充值后返回失败结果并返充的条数)
                var CurrentPeriodRecharge = currentRecharge.SMSCount.Value;//本期新充
                var TotalRemain = list[0].RemainSMSNumber;//总剩余量

                if ((LastTotal + CurrentPeriodRecharge) < TotalRemain)
                {
                    //return GetActionResult(new RPC_Result(false, "统计数据不符合逻辑"));
                    LastTotal += TotalRemain - (LastTotal + CurrentPeriodRecharge);
                }

                //Dictionary<string, double> d = new Dictionary<string, double>();
                //d.Add("LastTotal", LastTotal);//上期结余(充值时剩余的条数+充值前发送充值后返回失败结果并返充的条数)
                //d.Add("CurrentPeriodRecharge", CurrentPeriodRecharge);//本期新充
                //d.Add("TotalRemain", TotalRemain);//总剩余量

                var usage = LastTotal + CurrentPeriodRecharge - TotalRemain;
                var lastUsage = (LastTotal >= usage) ? usage : LastTotal;
                var currentUsage = usage - lastUsage;
                var model = new HomePageStatistics
                {
                    unit = "条",
                    TotalRemain = TotalRemain,                              //总剩余量
                    LastTotal = LastTotal,                                  //结转总量
                    LastUsage = lastUsage,                                  //结转用量
                    LastRemain = LastTotal - lastUsage,                     //结转余量
                    CurrentTotal = CurrentPeriodRecharge,                   //本期总量
                    CurrentUsage = currentUsage,                            //本期用量
                    CurrentRemain = CurrentPeriodRecharge - currentUsage    //本期余量
                };

                return Content(JsonSerialize.Instance.Serialize(model));
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "异常"));
            }
        }

        /// <summary>
        /// 短信ISMP 本期统计（应续费数和新开数）
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAgentCurrentMonthStatistic(FunctionParameter param)
        {
            try
            {
                //ISMP 请求代理商或代理商员工分管的企业
                string url = "";
                if (param.UserType == UserType.Agent)
                {
                    url = Util.ISMPHost + "/CallBack/GetEnterpriseAccountIdLoginNameByAgentAccountId?";
                    url += "AgentAccountId=" + System.Web.HttpUtility.UrlEncode(param.AccountId);
                }
                else
                {
                    url = Util.ISMPHost + "/CallBack/GetEnterpriseAccountIdLoginNameByAgentEmployee?";
                    url += "AgentAccountId=" + System.Web.HttpUtility.UrlEncode(param.AccountId);
                    url += "&AgentEmployeeAccountId=" + System.Web.HttpUtility.UrlEncode(param.OperatorAccountId);
                }

                string result = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                List<Dictionary<string, object>> enterList = JsonSerialize.Instance.Deserialize<List<Dictionary<string, object>>>(result);

                int needRechargeCount = 0;  //应充值
                int newOpenCount = 0;       //本月新开
                foreach (var item in enterList)
                {
                    string enterLoginName = item["LoginName"].ToString();

                    var smsTemplet = Util.SMSProxy.GetChargeStatics(enterLoginName);
                    if (!smsTemplet.Success)
                    {
                        continue;
                    }

                    var rechargeRecord = Util.SMSProxy.GetEnterpriseChargeRecord(enterLoginName, DateTime.MinValue, DateTime.MaxValue);
                    if (!rechargeRecord.Success)
                    {
                        continue;
                    }

                    if (smsTemplet.Value == null || smsTemplet.Value.Count <= 0 || rechargeRecord.Value == null || rechargeRecord.Value.Count <= 0)
                    {
                        continue;
                    }

                    //最近一条记录
                    var currentRecharge = (from record in rechargeRecord.Value orderby record.PrepaidTime descending select record).FirstOrDefault();
                    //最早一条记录
                    var oldestRecharge = (from record in rechargeRecord.Value orderby record.PrepaidTime ascending select record).FirstOrDefault();
                    if (null == currentRecharge || null == oldestRecharge)
                    {
                        continue;
                    }

                    //过滤
                    var list = smsTemplet.Value.Where(p => p.Enterprese == enterLoginName).ToList();
                    if (list == null || list.Count <= 0) continue;
                    int remainTotal = list[0].RemainSMSNumber;  //总剩余量
                    int currentTotal = currentRecharge.SMSCount.Value;  //本期总量
                    needRechargeCount += ((currentTotal != 0) ? ((1.0 * remainTotal / currentTotal <= 0.1) ? 1 : 0) : 0);

                    DateTime dt = DateTime.Now;
                    newOpenCount += (oldestRecharge.PrepaidTime.Value > dt.AddDays(1 - dt.Day)) ? 1 : 0;
                }

                var model = new HomePageStatistics_Agent
                {
                    NeedRechargeCount = needRechargeCount,  //应续费数
                    NewOpenCount = newOpenCount             //新开数
                };

                return Content(JsonSerialize.Instance.Serialize(model));
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "异常"));
            }
        }

        #endregion

        #region 通讯录

        public ActionResult SMSContacts()
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

        public ActionResult GetSMSContactGroupList()
        {
            ResultSet<Dictionary<string, object>> r = new ResultSet<Dictionary<string, object>>();
            r.Value = new List<Dictionary<string, object>>();
            try
            {
                WebParamList wpl = new WebParamList(Request);
                DataTable dt = PhoneAndGroupDB.GetGroupForTreeByAccountCode(wpl.str("AccountCode"));
                if (dt != null && dt.Rows.Count > 0)
                {
                    r.Total = dt.Rows.Count;
                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("id", dr["GID"]);
                        dic.Add("text", dr["TelPhoneGroupName"]);
                        if (dr["TelPhoneGroupName"] != null && dr["TelPhoneGroupName"].ToString() == "0")
                        {
                            dic["text"] = "未分组";
                        }
                        list.Add(dic);
                    }
                    r.Value = list;
                }
                return Content(r.ToString());
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return Content(r.ToString());
            }
        }

        public ActionResult GetSMSContactList()
        {
            ResultSet<Dictionary<string, object>> r = new ResultSet<Dictionary<string, object>>();
            r.Value = new List<Dictionary<string, object>>();
            try
            {
                WebParamList wpl = new WebParamList(Request);
                DataTable dt = null;
                if (string.IsNullOrWhiteSpace(wpl.str("GroupID")) || wpl.str("GroupID") == "-1")
                {
                    dt = PhoneAndGroupDB.GetPhoneByAccountCodes(wpl.str("AccountCode"));
                }
                else
                {
                    dt = PhoneAndGroupDB.GetPhoneByAccountCodeAndGroup(wpl.str("AccountCode"), wpl.str("GroupID"));
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(wpl.str("Number")))
                    {
                        DataRow[] drArray = dt.Select("TelPhoneNum like '%" + wpl.str("Number") + "%'");
                        DataTable dtTemp = dt.Clone();
                        dtTemp.Clear();
                        foreach (DataRow dr in drArray)
                        {
                            dtTemp.Rows.Add(dr.ItemArray);
                        }
                        dt = dtTemp;
                    }
                    r.Total = dt.Rows.Count;
                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        foreach (DataColumn dc in dr.Table.Columns)
                        {
                            dic.Add(dc.Caption, dr[dc.Caption]);
                        }
                        list.Add(dic);
                    }
                    r.Value = list;
                    //分页
                    if (r.Value != null && r.Value.Count > 0)
                    {
                        r.Value = (from item in r.Value orderby item["UserName"] select item).ToList();
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

        public ActionResult GetGroupSMSContact()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("GroupSelectSendSMS")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【目标组别】！"));
                }

                DataTable dt = PhoneAndGroupDB.GetPhoneByAccountCodeAndGroup(wpl.str("AccountCode"), wpl.str("GroupSelectSendSMS"));
                if (dt == null || dt.Rows.Count <= 0)
                {
                    return GetActionResult(new RPC_Result(false, "获取分组联系人失败或分组中无联系人！"));
                }

                List<string> valueList = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    valueList.Add(dr["TelPhoneNum"].ToString());
                }

                return GetActionResult(new RPC_Result<string>(true, string.Join(",", valueList.ToArray()), "操作成功！"));
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult AddSMSContact()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("GroupID")))
                {
                    DataTable dt = PhoneAndGroupDB.GetGroupByTelPhoneGroupNameAndAccountCode("0", wpl.str("AccountCode"));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        wpl.add("GroupID", dt.Rows[0][0].ToString());
                    }
                }
                bool result = PhoneAndGroupDB.PhoneUpload(wpl.str("AccountCode"), wpl.str("UserName"), wpl.str("UserBrithday"), wpl.str("UserSex"),
                    wpl.str("CompanyName"), wpl.str("TelPhoneNum"), wpl.str("CompanyEmail"), wpl.str("QQ"), wpl.str("ComPostion"), wpl.str("WebChat"), wpl.str("CompanyWeb"), wpl.str("GroupID"));
                if (result)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "企业通讯录", "添加一个号码", "企业【" + wpl.str("AccountCode") + "】，分组【" + wpl.str("GroupID") + "】，号码【" + wpl.str("TelPhoneNum") + "】", "添加通讯录项", CurrentUser);
                    return GetActionResult(new RPC_Result<string>(true, wpl.str("GroupID"), "操作成功！"));
                }
                else
                {
                    return GetActionResult(new RPC_Result(false, "操作失败！"));
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult DeleteSMSContact()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("PID")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【ID】！"));
                }
                bool result = PhoneAndGroupDB.PhoneDelByID(wpl.str("PID"));
                if (result)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "企业通讯录", "删除一个号码", "企业【" + wpl.str("AccountCode") + "】，通讯录项【" + wpl.str("PID") + "】", "删除通讯录项", CurrentUser);
                    return GetActionResult(new RPC_Result(true, "操作成功！"));
                }
                else
                {
                    return GetActionResult(new RPC_Result(false, "操作失败！"));
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult AddSMSContactGroup()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("GroupName")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【组名称】！"));
                }

                DataTable dt = PhoneAndGroupDB.GetGroupByTelPhoneGroupNameAndAccountCode(wpl.str("GroupName"), wpl.str("AccountCode"));
                if (dt != null && dt.Rows.Count > 0)
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，已存在同名通讯录组！"));
                }

                bool result = PhoneAndGroupDB.GroupAdd(wpl.str("AccountCode"), wpl.str("GroupName"), "");//描述为空
                if (result)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "企业通讯录", "添加一个分组", "企业【" + wpl.str("AccountCode") + "】，通讯录组名称【" + wpl.str("GroupName") + "】", "添加分组", CurrentUser);
                    return GetActionResult(new RPC_Result(true, "操作成功！"));
                }
                else
                {
                    return GetActionResult(new RPC_Result(false, "操作失败！"));
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult IsSMSContactGroupSame()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("GroupName")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【组名称】！"));
                }

                DataTable dt = PhoneAndGroupDB.GetGroupByTelPhoneGroupNameAndAccountCode(wpl.str("GroupName"), wpl.str("AccountCode"));
                if (dt != null && dt.Rows.Count > 0)
                {
                    return GetActionResult(new RPC_Result(false, "已存在同名通讯录组！"));
                }

                return GetActionResult(new RPC_Result(true, "可以使用！"));
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult ChangeSMSContactGroup()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("GroupSelect")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【目标组别】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("PIDSelect")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【待分组项】！"));
                }

                bool result = PhoneAndGroupDB.PhoneConnectGroup(wpl.str("GroupSelect"), wpl.str("AccountCode"), wpl.str("PIDSelect"));
                if (result)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "企业通讯录", "更换通讯录项分组", "企业【" + wpl.str("AccountCode") + "】，新通讯录组【" + wpl.str("GroupSelect") + "】，选择的通讯录【" + wpl.str("PIDSelect") + "】", "更换分组", CurrentUser);
                    return GetActionResult(new RPC_Result(true, "操作成功！"));
                }
                else
                {
                    return GetActionResult(new RPC_Result(false, "操作失败！"));
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult DeleteSMSContactGroup()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("GroupSelectDel")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【目标组别】！"));
                }
                //置当前分组联系人至未分组下
                DataTable dt = PhoneAndGroupDB.GetGroupByTelPhoneGroupNameAndAccountCode("0", wpl.str("AccountCode"));
                string GID = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    GID = dt.Rows[0][0].ToString();
                }
                DataTable ds = PhoneAndGroupDB.GetPhoneByAccountCodeAndGroup(wpl.str("AccountCode"), wpl.str("GroupSelectDel"));
                if (ds.Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Rows.Count; i++)
                    {
                        PhoneAndGroupDB.PhoneConnectGroup(GID, wpl.str("AccountCode"), ds.Rows[i]["PID"].ToString());
                    }
                }
                //删除
                bool result = PhoneAndGroupDB.GroupDelByID(wpl.str("GroupSelectDel"));
                if (result)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "企业通讯录", "删除分组", "企业【" + wpl.str("AccountCode") + "】，通讯录组【" + wpl.str("GroupSelectDel") + "】", "删除分组", CurrentUser);
                    return GetActionResult(new RPC_Result<string>(true, GID, "操作成功！"));
                }
                else
                {
                    return GetActionResult(new RPC_Result(false, "操作失败！"));
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult ImportSMSContactExcel()
        {
            try
            {
                var httpPostedFile = Request.Files["SMSContactExcelFile"];
                if (httpPostedFile == null)
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，获取上传文件失败！"));
                }
                DataTable dt = Util.ImportExcel(httpPostedFile.InputStream);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，获取文件有效失败或文件中不存在有效数据！"));
                }

                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("GroupSelectImportExcel")))
                {
                    DataTable dtGroup = PhoneAndGroupDB.GetGroupByTelPhoneGroupNameAndAccountCode("0", wpl.str("AccountCode"));
                    if (dtGroup != null && dtGroup.Rows.Count > 0)
                    {
                        wpl.add("GroupSelectImportExcel", dt.Rows[0][0].ToString());
                    }
                }

                string errorName = "";
                string errorNumber = "";
                string errorImport = "";
                string successNumber = "";
                //检测数据有效性及导入
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString() == "" || dt.Rows[i][0].ToString() == null)
                    {
                        errorName += i.ToString() + "、";
                        continue;
                    }
                    bool r = Util.ValidMobile(dt.Rows[i][4].ToString());
                    if (!r)
                    {
                        errorNumber += i.ToString() + "、";
                        continue;
                    }
                    bool rr = PhoneAndGroupDB.PhoneUpload(
                         wpl.str("AccountCode"), dt.Rows[i][0].ToString(),
                         dt.Rows[i][1].ToString(),
                         dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(),
                         dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString(),
                         dt.Rows[i][6].ToString(), dt.Rows[i][7].ToString(),
                         dt.Rows[i][8].ToString(), dt.Rows[i][9].ToString(), wpl.str("GroupSelectImportExcel"));
                    if (!rr)
                    {
                        errorImport += i.ToString() + "、";
                        continue;
                    }
                    successNumber += dt.Rows[i][4].ToString() + ",";
                }
                string message = "操作成功！但下列项导入失败：行号为：" + errorName + "联系人为空！行号为：" + errorNumber + "手机号格式不正确！行号为：" + errorImport + "导入时失败！";
                message = (string.IsNullOrWhiteSpace(errorName) && string.IsNullOrWhiteSpace(errorNumber) && string.IsNullOrWhiteSpace(errorImport)) ? "操作成功！" : message;
                Util.SendSystemLogToISMP(Util.SMSProductName + "企业通讯录", "批量导入通讯录项", "企业【" + wpl.str("AccountCode") + "】，分组【" + wpl.str("GroupSelectImportExcel") + "】，号码【" + successNumber + "】", "添加通讯录项", CurrentUser);
                return GetActionResult(new RPC_Result<string>(true, wpl.str("GroupSelectImportExcel"), message));
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult DownloadFile()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                string filePath = Server.MapPath(wpl.str("FilePath"));
                FileStream fs = new FileStream(filePath + "//" + wpl.str("FileName"), FileMode.Open);
                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                fs.Dispose();
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Response.ContentType = "application/octet-stream";

                Response.AddHeader("Content-Disposition", "attachment; filename=" + wpl.str("FileName"));
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                Response.Write("<script>alert('下载出现异常!')</script>");
                return new EmptyResult();
            }
        }

        public ActionResult ExportSMSContactGroup()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    Response.Write("<script>alert('操作失败，缺少参数【企业信息】！')</script>");
                    return new EmptyResult();
                }
                if (string.IsNullOrWhiteSpace(wpl.str("GroupSelectExport")))
                {
                    Response.Write("<script>alert('操作失败，缺少参数【目标组别】！')</script>");
                    return new EmptyResult();
                }
                //获取分组联系人
                DataTable ds = PhoneAndGroupDB.GetPhoneByAccountCodeAndGroup(wpl.str("AccountCode"), wpl.str("GroupSelectExport"));
                if (ds == null || ds.Rows.Count <= 0)
                {
                    Response.Write("<script>alert('操作失败，获取分组联系人失败或分组无联系人！')</script>");
                    return new EmptyResult();
                }

                string numberTemp = "";
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    numberTemp += ds.Rows[i]["TelPhoneNum"].ToString() + ",";
                }
                Util.SendSystemLogToISMP(Util.SMSProductName + "企业通讯录", "导出一组通讯录", "企业【" + wpl.str("AccountCode") + "】，分组【" + wpl.str("GroupSelectExport") + "】，号码【" + numberTemp + "】", "导出通讯录分组", CurrentUser);

                //处理导出DataTable
                string[] name = new string[] { "联系人", "生日", "性别", "电话号码", "联系人公司", "联系人Email", "联系人QQ", "联系人职位", "联系人微信", "联系人公司网站" };
                string[] text = new string[] { "UserName", "UserBrithday", "UserSex", "TelPhoneNum", "CompanyName", "CompanyEmail", "QQ", "ComPostion", "WebChat", "CompanyWeb" };
                for (int i = 0; i < name.Length; i++)
                {
                    ds.Columns[text[i]].SetOrdinal(i);
                    ds.Columns[text[i]].ColumnName = name[i];
                }
                for (int i = ds.Columns.Count - 1; i >= text.Length; i--)//移除多余列
                {
                    ds.Columns.RemoveAt(i);
                }

                MemoryStream ms = Util.DataTableToExcel(ds);
                byte[] bytes = new byte[(int)ms.Length];
                ms.Read(bytes, 0, bytes.Length);
                ms.Close();
                ms.Dispose();
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Response.ContentType = "application/octet-stream";

                Response.AddHeader("Content-Disposition", "attachment; filename=" + wpl.str("GroupSelectExportName") + "通讯录.xls");
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                Response.Write("<script>alert('导出出现异常!')</script>");
                return new EmptyResult();
            }
        }

        #endregion

        #region 修改密码

        public ActionResult ChangePassword()
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

        public ActionResult ChangeEnterprisePassword()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("OldPassword")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【平台登录密码】！"));
                }
                if (string.IsNullOrWhiteSpace(wpl.str("Password")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【短信发送密码】！"));
                }

                //ISMP 验证密码
                string url = Util.ISMPHost + "/CallBack/ValidatePassword?";
                url += "LoginName=" + System.Web.HttpUtility.UrlEncode(wpl.str("AccountCode"))
                    + "&Password=" + System.Web.HttpUtility.UrlEncode(EncryptTool.symmetry_Encode(wpl.str("OldPassword")));

                string result = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(result, new { success = true, message = string.Empty });
                if (!o.success)
                {
                    return GetActionResult(new RPC_Result(false, "平台登录密码验证失败！" + o.message));
                }

                var rr = Util.SMSProxy.ResetEnterprisePass(wpl.str("AccountCode"), wpl.str("Password").Trim());
                if (rr.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "发送密码设置", "修改短信发送密码", "企业【" + wpl.str("AccountCode") + "】", "修改短信发送密码", CurrentUser);
                }
                return GetActionResult(rr);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        #endregion

        #region 短信企业黑名单

        public ActionResult SMSEnterpriseBlackList()
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

        public ActionResult GetEnterpriseBlackList()
        {
            ResultSet<string> r = new ResultSet<string>();
            r.Value = new List<string>();
            try
            {
                WebParamList wpl = new WebParamList(Request);

                var smsTemplet = Util.SMSProxy.GetEnterpriseBlackList(wpl.str("AccountCode"));
                if (!smsTemplet.Success)
                {
                    return GetActionResult(smsTemplet);
                }

                if (smsTemplet.Value == null || smsTemplet.Value.Count <= 0) return Content(r.ToString()); ;
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

        public ActionResult AddEnterpriseBlackNumber()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (wpl.str("Number").Trim() == "")
                {
                    return GetActionResult(new RPC_Result(false, "无有效号码！"));
                }
                List<string> list = new List<string>(wpl.str("Number").Trim().Split(','));
                var r = Util.SMSProxy.AddEnterpriseBlackList(wpl.str("AccountCode"), list);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "企业黑名单", "添加企业黑名单号码", "企业【" + wpl.str("AccountCode") + "】，添加黑名单号码为【" + wpl.str("Number") + "】", "添加企业黑名单", CurrentUser);
                }
                return GetActionResult(r);
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "操作异常！"));
            }
        }

        public ActionResult DelEnterpriseBlackNumber()
        {
            try
            {
                WebParamList wpl = new WebParamList(Request);
                if (string.IsNullOrWhiteSpace(wpl.str("AccountCode")))
                {
                    return GetActionResult(new RPC_Result(false, "操作失败，缺少参数【企业信息】！"));
                }
                if (wpl.str("Number").Trim() == "")
                {
                    return GetActionResult(new RPC_Result(false, "无有效号码！"));
                }
                List<string> list = new List<string>(wpl.str("Number").Trim().Split(','));
                var r = Util.SMSProxy.DeleteEnterpriseBlackList(wpl.str("AccountCode"), list);
                if (r.Success)
                {
                    Util.SendSystemLogToISMP(Util.SMSProductName + "企业黑名单", "删除企业黑名单号码", "企业【" + wpl.str("AccountCode") + "】，删除黑名单号码为【" + wpl.str("Number") + "】", "删除企业黑名单", CurrentUser);
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

        #region 终端企业短信自充值

        /// <summary>
        /// 短信企业自充值可选金额或套餐查询
        /// </summary>
        /// <returns></returns>
        public ActionResult EnterpriseSMSAutoRechargeAsk()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Request["Parameter"]))
                {
                    return GetActionResult(new RPC_Result(false, "接收参数失败"));
                }
                var param = Util.DeserializeParameter<FunctionParameter>(Request["Parameter"]);
                if (param == null)
                {
                    return GetActionResult(new RPC_Result(false, "解析参数失败"));
                }

                AutoRechargeLimit arl = new AutoRechargeLimit();
                arl.AutoRechargeLimitType = AutoRechargeLimitType.UnLimit;
                arl.AgentDiscount = Util.SMSRate;
                arl.ProductPayType = "短信充值";

                return Content(JsonSerialize.Instance.Serialize(arl));
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return GetActionResult(new RPC_Result(false, "异常"));
            }
        }

        ///// <summary>
        ///// 短信企业自充值
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult EnterpriseSMSAutoRecharge()
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(Request["Parameter"]))
        //        {
        //            return GetActionResult(new RPC_Result(false, "接收参数失败"));
        //        }
        //        var param = Util.DeserializeParameter<EnterpriseAutoRechargeParameter>(Request["Parameter"]);
        //        if (param == null)
        //        {
        //            return GetActionResult(new RPC_Result(false, "解析参数失败"));
        //        }

        //        string enterpriseAccountID = param.EnterpriseAccountId;
        //        string agentAccountID = param.AgentAccountId;

        //        var smsNumber = (int)Math.Ceiling(param.Money / Util.SMSRate);
        //        //给企业充值
        //        if (smsNumber > 0)
        //        {
        //            ChargeRecord cr = new ChargeRecord();
        //            cr.ChargeFlag = 0;
        //            cr.Money = param.Money;
        //            cr.SMSCount = smsNumber;
        //            cr.ThenRate = Convert.ToDecimal(Util.SMSRate);
        //            cr.OperatorAccount = CurrentUser.LoginName;
        //            cr.PrepaidAccount = param.EnterpriseLoginName;
        //            cr.PrepaidTime = DateTime.Now;
        //            cr.PrepaidType = 1;

        //            //ISMP 扣费
        //            string url = Util.ISMPHost + "/CallBack/DeductForProduct?"
        //                + "DeductAccountId=" + System.Web.HttpUtility.UrlEncode(param.AgentAccountId)
        //                + "&RechargeAccountId=" + System.Web.HttpUtility.UrlEncode(param.EnterpriseAccountId)
        //                + "&Money=" + System.Web.HttpUtility.UrlEncode(Convert.ToString(cr.Money))
        //                + "&Description=" + System.Web.HttpUtility.UrlEncode(param.Description)
        //                + "&ProductPayType=" + System.Web.HttpUtility.UrlEncode("短信充值")
        //                + "&ApplyAccountId=" + System.Web.HttpUtility.UrlEncode(CurrentUser.OperatorAccountId)
        //                + "&ApplyName=" + System.Web.HttpUtility.UrlEncode(CurrentUser.OperatorName)
        //                + "&Type=" + System.Web.HttpUtility.UrlEncode("22")
        //                + "&ProductId=" + System.Web.HttpUtility.UrlEncode(Util.SMSProductId);

        //            string result = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
        //            var o = JsonConvert.DeserializeAnonymousType(result, new { success = true, message = string.Empty });
        //            if (o.success)
        //            {
        //                return GetActionResult(new RPC_Result(true, "充值成功,充值短信【" + smsNumber + "】条。"));
        //                var r = Util.SMSProxy.AccountPrepaid(cr);
        //                if (r.Success)
        //                {
        //                    Util.SendSystemLogToISMP(Util.SMSProductName + "充值", "自助充值，充值" + Util.SMSProductName + "【" + smsNumber + "】条。", "企业AccountID【" + enterpriseAccountID + "】，代理商AccountID【" + agentAccountID + "】，金额【" + cr.Money + "】", "充值", CurrentUser);
        //                    return GetActionResult(new RPC_Result(true, "充值成功,充值短信【" + smsNumber + "】条。"));
        //                }
        //                else
        //                {
        //                    //此处应记录日志和错误，并及时通知
        //                    Util.SendAlertMessageByEmail(Util.SMSProductName + "产品充值扣费成功，充值失败：企业AccountID【" + enterpriseAccountID + "】，代理商AccountID【" + agentAccountID + "】，金额【" + cr.Money + "】，充值失败原因【" + r.Message + "】");
        //                    //此处扣费成功但充值失败。
        //                    Util.SendSystemLogToISMP(Util.SMSProductName + "充值", "扣费成功，充值失败", "企业AccountID【" + enterpriseAccountID + "】，代理商AccountID【" + agentAccountID + "】，金额【" + cr.Money + "】，充值失败原因【" + r.Message + "】", "充值", CurrentUser);
        //                    return GetActionResult(new RPC_Result(false, "给代理商扣费成功，充值失败，失败原因【" + r.Message + "】"));
        //                }
        //            }
        //            else
        //            {
        //                Util.SendSystemLogToISMP(Util.SMSProductName + "充值", "扣费失败", "企业AccountID【" + enterpriseAccountID + "】，代理商AccountID【" + agentAccountID + "】，金额【" + cr.Money + "】，扣费失败原因【" + o.message + "】", "充值", CurrentUser);
        //                return GetActionResult(new RPC_Result(false, "扣费失败，失败原因【" + o.message + "】"));
        //            }
        //        }
        //        else
        //        {
        //            return GetActionResult(new RPC_Result(false, "充值金额不正确"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4Logger.Error(ex);
        //        return GetActionResult(new RPC_Result(false, "异常"));
        //    }
        //}

        #endregion

    }
}
