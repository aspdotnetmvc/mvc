using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ServiceModel;
using SMSPlatInterface;

namespace HttpSMService
{
    public partial class GetBalance : System.Web.UI.Page
    {
        #region 变量定义

        private string _User = "";
        private string _Pass = "";

        private string _Result = "";
        private int _SmsBalance = 0;
        private int _MmsBalance = 0;

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
                _User = Request["USER"];
                _Pass = Request["PASS"];
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

                        // 查询余额
                        try
                        {
                            SMS.Model.RPCResult<SMS.Model.UserBalance> r = InterfaceProxy.GetSendService().GetBalanceByPlainPass(_User, _Pass);
                            if (r.Success)
                            {
                                _Result = _ERR_SUCCESS;
                                _SmsBalance = r.Value.SmsBalance;
                                _MmsBalance = r.Value.MmsBalance;
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

                // 返回结果
                WriteResponse(_Result, _SmsBalance, _MmsBalance);
            }
        }
        private void WriteResponse(string result, int smsbalance, int mmsbalance)
        {
            GetBalanceResult ret = new GetBalanceResult();
            ret.Result = result;
            ret.SmsBalance = smsbalance.ToString();
            ret.MmsBalance = mmsbalance.ToString();
            string data = JsonSerialize.Serialize<GetBalanceResult>(ret);
            Response.Write(data);
            Response.End();
        }
    }
}