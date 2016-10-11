using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.UI;
using ServiceModel;
using SMSPlatInterface;
using System.Security;

namespace HttpSMService
{
    public partial class SendSMS : System.Web.UI.Page
    {
        #region 变量定义

        private string _User = "";
        private string _Pass = "";
        private string _Dests = "";
        private string _Content = "";
        private string _SendTime = "";

        private string _Result = "";
        private string _MsgID = "";

        #endregion

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string[] dest;

                _User = Request["User"];
                _Pass = Request["Pass"];
                _Dests = Request["Destinations"];
                _Content = Request["Content"];
                _SendTime = Request["SendTime"];

                // 判断参数合法性
                if (string.IsNullOrWhiteSpace(_User))
                {
                    _Result = _ERR_USERISEMPTY;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_Pass))
                    {
                        _Result = _ERR_PASSISEMPTY;
                    }
                    else
                    {
                        // 判断用户名和密码是否正确

                        if (string.IsNullOrWhiteSpace(_Dests))
                        {
                            _Result = _ERR_DESTISEMPTY;
                        }
                        else
                        {
                            dest = Regex.Split(_Dests, ",");
                            if (dest.Count() > 1000) // 接收短信的终端数量大于1000个
                            {
                                _Result = _ERR_DESTTOLONG;
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
                                    //SMS.Model.RPCResult<string> enpass = InterfaceProxy.GetSendService().EncryptByAccount(_User,_Pass);

                                    // SMS.Model.RPCResult<Guid> r = InterfaceProxy.GetSendService().SendSMS(_User, _Pass, _Content, num);
                                    var r = InterfaceProxy.GetSendService().SendSMS(_User, _Pass, _Content, num,"http");
                                    if (r.Success)
                                    {
                                        _Result = _ERR_SUCCESS;
                                        _MsgID = r.Value.SMSNumbers[0].ID.ToString();
                                        if (r.Value.Message.AuditType == SMS.Model.AuditType.Manual)
                                        {
                                            APPCode.Util.SendToDoToSMSAuditor(r.Value.Message);
                                        }
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
                            }
                        }
                    }
                }

                // 返回结果
                WriteResponse(_Result, _MsgID);
            }
        }


        private void WriteResponse(string result, string msgid)
        {
            SendSMSResult ret = new SendSMSResult();
            ret.Result = result;
            ret.MsgID = msgid;
            string data = JsonSerialize.Serialize<SendSMSResult>(ret);
            Response.Write(data);
            Response.End();
        }
    }
}