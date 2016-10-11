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
    public partial class GetReport : System.Web.UI.Page
    {
        #region 变量定义

        private string _User = "";
        private string _Pass = "";
        private string _MsgID = "";

        private string _Result = "";
        private List<Report> _Reports;

        #endregion

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                _User = Request["User"];
                _Pass = Request["Pass"];
                _MsgID = Request["MsgID"];
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

                        if (string.IsNullOrWhiteSpace(_MsgID)) // 根据账号查询
                        {
                            try
                            {
                                // 密码加密
                                //SMS.Model.RPCResult<string> enpass = InterfaceProxy.GetSendService().EncryptByAccount(_User,_Pass);

                                SMS.Model.RPCResult<List<SMS.Model.StatusReport>> r = InterfaceProxy.GetSendService().GetReport(_User, _Pass);
                                if (r.Success)
                                {
                                    _Result = _ERR_SUCCESS;
                                    List<SMS.Model.StatusReport> rts = r.Value;

                                    _Reports = new List<Report>();
                                    for (int k = 0; k < rts.Count; k++)
                                    {
                                        Report rt = new Report();
                                        rt.MsgID = rts[k].SMSID.ToString();
                                        rt.Destination = rts[k].Number;
                                        rt.Stat = rts[k].StatusCode.ToString();
                                        _Reports.Add(rt);
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
                        else  // 根据短信标识查询
                        {
                            try
                            {
                                // 密码加密
                                //SMS.Model.RPCResult<string> enpass = InterfaceProxy.GetSendService().EncryptByAccount(_User,_Pass);

                                SMS.Model.RPCResult<List<SMS.Model.StatusReport>> r = InterfaceProxy.GetSendService().GetReport(_User, _Pass, _MsgID);
                                if (r.Success)
                                {
                                    _Result = _ERR_SUCCESS;
                                    List<SMS.Model.StatusReport> rts = r.Value;

                                    _Reports = new List<Report>();
                                    for (int k = 0; k < rts.Count; k++)
                                    {
                                        Report rt = new Report();
                                        rt.MsgID = rts[k].SMSID.ToString();
                                        rt.Destination = rts[k].Number;
                                        rt.Stat = rts[k].StatusCode.ToString();
                                        _Reports.Add(rt);
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

                // 返回结果
                WriteResponse(_Result, _Reports);
            }
        }

        private void WriteResponse(string result, List<Report> rts)
        {
            GetReportResult ret = new GetReportResult();
            ret.Result = result;
            ret.Reports = rts;
            string data = JsonSerialize.Serialize<GetReportResult>(ret);
            Response.Write(data);
            Response.End();
        }
    }
}