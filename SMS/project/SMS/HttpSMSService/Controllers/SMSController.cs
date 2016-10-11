using HttpSMSService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HttpSMSService.Controllers
{
    public class SMSController : Controller
    {
        #region 常量

        private const string _ERR_SERVICE = "-7:服务器内部错误";
        private const string _ERR_DESTTOLONG = "-6:一次接收短信的手机号码不能超过1000个";
        private const string _ERR_INVALIDUSER = "-5:用户名不存在";
        private const string _ERR_INVALIDPASS = "-4:密码错误";
        private const string _ERR_USERISEMPTY = "-3:用户名不能为空";
        private const string _ERR_PASSISEMPTY = "-2:密码不能为空";
        private const string _ERR_DESTISEMPTY = "-1:短信接收者不能为空";
        private const string _ERR_SUCCESS = "1:成功";

        #endregion
        public ActionResult SendSMS(string User, string Pass, string Mobiles, string Content, string SendTime)
        {
            SendSMSResult ret = new SendSMSResult();
            // 判断参数合法性
            if (string.IsNullOrWhiteSpace(User))
            {
                ret.Result = _ERR_USERISEMPTY;
            }
            else if (string.IsNullOrWhiteSpace(Pass))
            {
                ret.Result = _ERR_PASSISEMPTY;
            }
            else if (string.IsNullOrWhiteSpace(Mobiles))
            {
                ret.Result = _ERR_DESTISEMPTY;
            }
            else
            {
                // 发送短信
                try
                {

                    var r = InterfaceProxy.GetSendService().SendSMS(User, Pass, Content, Mobiles.Split(',').ToList(), "http");
                    if (r.Success)
                    {
                        ret.Result = _ERR_SUCCESS;
                        ret.MsgID = r.Value.Message.ID;
                        if (r.Value.Message.AuditType == SMS.Model.AuditType.Manual)
                        {
                            Util.SendToDoToSMSAuditor(r.Value.Message);
                        }
                    }
                    else
                    {
                        ret.Result = r.Message;
                    }
                }
                catch (Exception ex)
                {
                    ret.Result = _ERR_SERVICE;
                }
            }

            string data = JsonSerialize.Serialize<SendSMSResult>(ret);
            return base.Content(data);
        }
        public ActionResult GetBalance(string User, string Pass)
        {
            string _Result = "";
            int _SmsBalance = 0;
            if (string.IsNullOrWhiteSpace(User))
            {
                _Result = _ERR_USERISEMPTY;
            }

            else if (string.IsNullOrWhiteSpace(Pass))
            {
                _Result = _ERR_PASSISEMPTY;
            }
            // 查询余额
            try
            {
                SMS.Model.RPCResult<SMS.Model.UserBalance> r = InterfaceProxy.GetSendService().GetBalanceByPlainPass(User, Pass);
                if (r.Success)
                {
                    _Result = _ERR_SUCCESS;
                    _SmsBalance = r.Value.SmsBalance;
                }
                else
                {
                    _Result = r.Message;
                }
            }
            catch (Exception ex)
            {
                _Result = _ERR_SERVICE;
            }
            GetBalanceResult ret = new GetBalanceResult();
            ret.Result = _Result;
            ret.SmsBalance = _SmsBalance.ToString();
            string data = JsonSerialize.Serialize<GetBalanceResult>(ret);
            return Content(data);
        }
        public ActionResult GetReport(string User, string Pass, string MsgID)
        {
            GetReportResult ret = new GetReportResult();
            if (string.IsNullOrWhiteSpace(User))
            {
                ret.Result = _ERR_USERISEMPTY;
            }

            if (string.IsNullOrWhiteSpace(Pass))
            {
                ret.Result = _ERR_PASSISEMPTY;
            }
            try
            {
                var r = InterfaceProxy.GetSendService().GetReport(User, Pass, MsgID);

                if (r.Success)
                {
                    ret.Result = _ERR_SUCCESS;
                    List<SMS.Model.StatusReport> rts = r.Value;

                    ret.Reports = (from sr in rts
                                   select new Report()
                                   {
                                       MsgID = sr.SMSID,
                                       Mobile = sr.Number,
                                       Status = sr.Succeed ? "true" : "false"
                                   }).ToList();

                }
                else
                {
                    ret.Result = r.Message;
                }
            }
            catch (Exception ex)
            {
                ret.Result = _ERR_SERVICE;
            }

            string data = JsonSerialize.Serialize<GetReportResult>(ret);
            return Content(data);
        }

        public ActionResult GetMO(string User, string Pass)
        {
            GetSMSResult ret = new GetSMSResult();

            if (string.IsNullOrWhiteSpace(User))
            {
                ret.Result = _ERR_USERISEMPTY;
            }

            if (string.IsNullOrWhiteSpace(Pass))
            {
                ret.Result = _ERR_PASSISEMPTY;
            }
            try
            {

                SMS.Model.RPCResult<List<SMS.Model.MOSMS>> mos = InterfaceProxy.GetSendService().GetSMS(User, Pass);

                if (mos.Success)
                {
                    ret.Result = _ERR_SUCCESS;
                    List<SMS.Model.MOSMS> msgs = mos.Value;

                    ret.MONum = msgs.Count.ToString();

                    ret.Msgs = (from mo in msgs
                                select new MO()
                                    {
                                        Mobile = mo.UserNumber,
                                        Content = mo.Message,
                                        MOTime = mo.ReceiveTime.ToString()
                                    }).ToList();
                }
                else
                {
                    ret.Result = mos.Message;
                }
            }
            catch (Exception ex)
            {
                ret.Result = _ERR_SERVICE;
            }
            string data = JsonSerialize.Serialize<GetSMSResult>(ret);
            return Content(data);
        }
    }
}
