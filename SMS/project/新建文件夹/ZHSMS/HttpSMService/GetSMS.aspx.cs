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

namespace HttpSMService
{
    public partial class GetSMS : System.Web.UI.Page
    {
        #region 变量定义

        private string _User = "";
        private string _Pass = "";

        private string _Result = "";
        private int _MONum = 0;
        private int _GetNum = 0;
        private List<MO> _MO;

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
                _User = Request["User"];
                _Pass = Request["Pass"];
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

                        try
                        {
                            // 密码加密
                            //SMS.Model.RPCResult<string> enpass = InterfaceProxy.GetSendService().EncryptByAccount(_User,_Pass);

                            SMS.Model.RPCResult<List<SMS.Model.MOSMS>> mos = InterfaceProxy.GetSendService().GetSMS(_User, _Pass);

                            if (mos.Success)
                            {
                                _Result = _ERR_SUCCESS;
                                List<SMS.Model.MOSMS> msgs = mos.Value;

                                _GetNum = msgs.Count;

                                _MO = new List<MO>();
                                for (int k = 0; k < msgs.Count; k++)
                                {
                                    MO mo = new MO();
                                    mo.Src = msgs[k].UserNumber;
                                    mo.Content = msgs[k].Message;
                                    mo.MOTime = msgs[k].ReceiveTime.ToString("yyyy-MM-dd HH:mm:ss");
                                    _MO.Add(mo);
                                }
                            }
                            else
                            {
                                _Result = mos.Message;
                            }
                        }
                        catch (Exception ex)
                        {
                            _Result = _ERR_SERVICE;
                        }
                    }
                }

                // 返回结果
                WriteResponse(_Result, _MONum, _GetNum, _MO);
            }
        }

        private void WriteResponse(string result, int monum, int getnum, List<MO> mos)
        {
            GetSMSResult ret = new GetSMSResult();
            ret.Result = result;
            ret.MONum = monum.ToString();
            ret.GetNum = getnum.ToString();
            ret.Msgs = mos;
            string data = JsonSerialize.Serialize<GetSMSResult>(ret);
            Response.Write(data);
            Response.End();
        }
    }
}