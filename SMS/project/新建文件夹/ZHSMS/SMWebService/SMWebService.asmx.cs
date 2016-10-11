using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Text.RegularExpressions;
using ServiceModel;

namespace SMWebService
{
    /// <summary>
    /// SMWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://www.smswebservice.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class SMWebService : System.Web.Services.WebService
    {
        #region 常量

        private const string _ERR_MSGIDISEMPTY = "-8:短信标识不能为空";
        private const string _ERR_SERVICE = "-7:服务器内部错误";
        private const string _ERR_DESTTOLONG = "-6:一次接收短信的手机号码不能超过1000个";
        private const string _ERR_INVALIDUSER = "-5:用户名不存在";
        private const string _ERR_INVALIDPASS = "-4:密码错误";
        private const string _ERR_USERISEMPTY = "-3:用户名不能为空";
        private const string _ERR_PASSISEMPTY = "-2:密码不能为空";
        private const string _ERR_DESTISEMPTY = "-1:短信接收者不能为空";
        private const string _ERR_SUCCESS = "1:成功";

        #endregion

        [WebMethod]
        public string SendSMS(string User, string Pass, string Destinations, string Content, string SendTime)
        {
            string result = "";
            string msgid = "";
            string[] dest;

            if (User == "")
            {
                result = _ERR_USERISEMPTY;
            }
            else
            {
                if (Pass == "")
                {
                    result = _ERR_PASSISEMPTY;
                }
                else
                {
                    if (Destinations == "")
                    {
                        result = _ERR_DESTISEMPTY;
                    }
                    else
                    {
                        dest = Regex.Split(Destinations, ",");
                        if (dest.Count() > 1000) // 接收短信的终端数量大于1000个
                        {
                            result = _ERR_DESTTOLONG;
                        }
                        else // 发送短信
                        {

                            // 接收短信的号码
                            List<string> num = new List<string>();
                            foreach (string destnum in dest)
                            {
                                num.Add(destnum);
                            }

                            // 发送短信
                            try
                            {
                                // 密码加密
                                //SMS.Model.RPCResult<string> enpass = InterfaceProxy.GetSendService().Encrypt(Pass);

                                var r = InterfaceProxy.GetSendService().SendSMS(User, Pass, Content, num,"webservice");
                                if (r.Success)
                                {
                                    result = _ERR_SUCCESS;
                                    msgid = r.Value.SMSNumbers[0].ID.ToString();
                                    if (r.Value.Message.AuditType == SMS.Model.AuditType.Manual)
                                    {
                                        Util.SendToDoToSMSAuditor(r.Value.Message);
                                    }
                                }
                                else
                                {
                                    result = r.Message;
                                }
                            }
                            catch (Exception ex)
                            {
                                result = _ERR_SERVICE;
                            }

                        }
                    }
                }
            }

            SendSMSResult ret = new SendSMSResult();
            ret.Result = result;
            ret.MsgID = msgid;
            string data = JsonSerialize.Serialize<SendSMSResult>(ret);

            return data;
        }

        public string DecodeFromBase64(string input)
        {
            byte[] bytes = System.Convert.FromBase64String(input);
            var retr = System.Text.Encoding.UTF8.GetString(bytes);
            return retr;
        }

        [WebMethod]
        public string GetSMS(string User, string Pass)
        {
            string result = "";
            int MONum = 0;
            int GetNum = 0;
            List<MO> MO = new List<MO>();

            if (User == "")
            {
                result = _ERR_USERISEMPTY;
            }
            else
            {
                if (Pass == "")
                {
                    result = _ERR_PASSISEMPTY;
                }
                else
                {
                    try
                    {
                        // 密码加密
                        //SMS.Model.RPCResult<string> enpass = InterfaceProxy.GetSendService().Encrypt(Pass);

                        SMS.Model.RPCResult<List<SMS.Model.MOSMS>> mos = InterfaceProxy.GetSendService().GetSMS(User, Pass);

                        if (mos.Success)
                        {
                            result = _ERR_SUCCESS;
                            List<SMS.Model.MOSMS> msgs = mos.Value;

                            GetNum = msgs.Count;

                            for (int k = 0; k < msgs.Count; k++)
                            {
                                MO mo = new MO();
                                mo.Src = msgs[k].UserNumber;
                                mo.Content = msgs[k].Message;
                                mo.MOTime = msgs[k].ReceiveTime.ToString("yyyy-MM-dd HH:mm:ss");
                                MO.Add(mo);
                            }
                        }
                        else
                        {
                            result = mos.Message;
                        }
                    }
                    catch (Exception ex)
                    {
                        result = _ERR_SERVICE;
                    }
                }
            }

            GetSMSResult ret = new GetSMSResult();
            ret.Result = result;
            ret.MONum = MONum.ToString();
            ret.GetNum = GetNum.ToString();
            ret.Msgs = MO;
            string data = JsonSerialize.Serialize<GetSMSResult>(ret);

            return data;
        }

        [WebMethod]
        public string GetReport(string User, string Pass, string MsgID)
        {
            string result = "";
            List<Report> Reports = new List<Report>();

            // 判断参数合法性
            if (User == "")
            {
                result = _ERR_USERISEMPTY;
            }
            else
            {
                if (Pass == "")
                {
                    result = _ERR_PASSISEMPTY;
                }
                else
                {
                    if (MsgID == "") // 根据账号查询
                    {
                        try
                        {
                            // 密码加密
                            //SMS.Model.RPCResult<string> enpass = InterfaceProxy.GetSendService().Encrypt(Pass);

                            SMS.Model.RPCResult<List<SMS.Model.StatusReport>> r = InterfaceProxy.GetSendService().GetReport(User, Pass);
                            if (r.Success)
                            {
                                result = _ERR_SUCCESS;
                                List<SMS.Model.StatusReport> rts = r.Value;

                                for (int k = 0; k < rts.Count; k++)
                                {
                                    Report rt = new Report();
                                    rt.MsgID = rts[k].SMSID.ToString();
                                    rt.Destination = rts[k].Number;
                                    rt.Stat = rts[k].StatusCode.ToString();
                                    Reports.Add(rt);
                                }
                            }
                            else
                            {
                                result = r.Message;
                            }
                        }
                        catch (Exception ex)
                        {
                            result = _ERR_SERVICE;
                        }
                    }
                    else // 根据发送的短信标识查询
                    {
                        try
                        {
                            // 密码加密
                            //SMS.Model.RPCResult<string> enpass = InterfaceProxy.GetSendService().Encrypt(Pass);

                            SMS.Model.RPCResult<List<SMS.Model.StatusReport>> r = InterfaceProxy.GetSendService().GetReport(User, Pass, MsgID);
                            if (r.Success)
                            {
                                result = _ERR_SUCCESS;
                                List<SMS.Model.StatusReport> rts = r.Value;

                                for (int k = 0; k < rts.Count; k++)
                                {
                                    Report rt = new Report();
                                    rt.MsgID = rts[k].SMSID.ToString();
                                    rt.Destination = rts[k].Number;
                                    rt.Stat = rts[k].StatusCode.ToString();
                                    Reports.Add(rt);
                                }
                            }
                            else
                            {
                                result = r.Message;
                            }
                        }
                        catch (Exception ex)
                        {
                            result = _ERR_SERVICE;
                        }
                    }
                }
            }

            GetReportResult ret = new GetReportResult();
            ret.Result = result;
            ret.Reports = Reports;
            string data = JsonSerialize.Serialize<GetReportResult>(ret);

            return data;
        }

        [WebMethod]
        public string GetBalance(string User, string Pass)
        {
            string Result = "";
            int SmsBalance = 0;
            int MmsBalance = 0;

            // 判断参数合法性
            if (User == "")
            {
                Result = _ERR_USERISEMPTY;
            }
            else
            {
                if (Pass == "")
                {
                    Result = _ERR_PASSISEMPTY;
                }
                else
                {
                    // 查询余额
                    try
                    {
                        // 密码加密
                        //SMS.Model.RPCResult<string> enpass = InterfaceProxy.GetSendService().Encrypt(Pass);

                        SMS.Model.RPCResult<SMS.Model.UserBalance> r = InterfaceProxy.GetSendService().GetBalanceByPlainPass(User, Pass);
                        if (r.Success)
                        {
                            Result = _ERR_SUCCESS;
                            SmsBalance = r.Value.SmsBalance;
                            MmsBalance = r.Value.MmsBalance;
                        }
                        else
                        {
                            Result = r.Message;
                        }
                    }
                    catch (Exception ex)
                    {
                        Result = _ERR_SERVICE;
                    }
                }
            }

            GetBalanceResult ret = new GetBalanceResult();
            ret.Result = Result;
            ret.SmsBalance = SmsBalance.ToString();
            ret.MmsBalance = MmsBalance.ToString();
            string data = JsonSerialize.Serialize<GetBalanceResult>(ret);

            return data;
        }
    }
}
