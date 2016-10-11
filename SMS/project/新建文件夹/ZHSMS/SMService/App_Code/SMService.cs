using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using System.Text.RegularExpressions;
using PretreatmentService;
using SMSModel;
using CommonUtils;

/// <summary>
/// SMService 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/",Description="短信服务的相关接口，包含发送短信的接口SendSMS，查询发送状态报告的接口SendReport，查询余额的接口")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
// [System.Web.Script.Services.ScriptService]
public class SMService : System.Web.Services.WebService
{

    #region 常量

    private const string _ERR_DESTTOLONG  = "-6:一次接收短信的手机号码不能超过100个";
    private const string _ERR_INVALIDUSER = "-5:用户名不存在";
    private const string _ERR_INVALIDPASS = "-4:密码错误";
    private const string _ERR_USERISEMPTY = "-3:用户名不能为空";
    private const string _ERR_PASSISEMPTY = "-2:密码不能为空";
    private const string _ERR_DESTISEMPTY = "-1:短信接收者不能为空";

    #endregion

    public SMService()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string SendSMS(string UserName, string UserPass, string Destination, string SMContent)
    {
        string[] dest;

        if (UserName == "")
        {
            return _ERR_USERISEMPTY;
        }

        if (UserPass == "")
        {
            return _ERR_PASSISEMPTY;
        }

        if (Destination == "")
        {
            return _ERR_DESTISEMPTY;
        }
        else
        {
            dest = Regex.Split(Destination, ",");
            if (dest.Count() > 100)
            {
                return _ERR_DESTTOLONG;
            }
        }

        ISMS interface_sms = (ISMS)Activator.GetObject(typeof(ISMS), ConfigurationManager.AppSettings["Pretreatment"]);
        SMS model_sms = new SMS();
        // 是否审核
        model_sms.Audit = AuditType.Manual;
        // 是否过滤
        model_sms.Filter = FilterType.Failure;
        // 消息级别
        model_sms.Level = LevelType.Level0;
        // 用户名
        model_sms.Account = UserName;
        // 短信内容
        model_sms.Content = SMContent;
        model_sms.SPNumber = "";
        // 接收短信的号码
        List<string> num = new List<string>();
        foreach (string destnum in dest)
        {
            num.Add(destnum);
        }
        model_sms.Number = num;
        model_sms.SendTime = DateTime.Now;
        model_sms.SerialNumber = Guid.NewGuid();
        // 是否需要短信报告
        model_sms.StatusReport = StatusReportType.Enabled;
        // 通道类型
        model_sms.Channel = ChannelType.Industry;

        // 发送短信
        RPCResult<Guid> r = interface_sms.SendSMS(model_sms);

        SendSMSResult ret = new SendSMSResult();
        ret.Result = r.Message;
        ret.MsgId = r.Value.ToString();
        string data = JsonSerialize.Serialize<SendSMSResult>(ret);

        return data;
    }

    [WebMethod]
    public string GetSMS(string UserName,string UserPass)
    {
        if (UserName == "")
        {
            return _ERR_USERISEMPTY;
        }

        if (UserPass == "")
        {
            return _ERR_PASSISEMPTY;
        }

        return "";
    }

    [WebMethod]
    public string GetReport(string UserName, string UserPass,string Guid)
    {
        if (UserName == "")
        {
            return _ERR_USERISEMPTY;
        }

        if (UserPass == "")
        {
            return _ERR_PASSISEMPTY;
        }

        return "";
    }

    [WebMethod]
    public string GetBalance(string UserName, string UserPass)
    {
        if (UserName == "")
        {
            return _ERR_USERISEMPTY;
        }

        if (UserPass == "")
        {
            return _ERR_PASSISEMPTY;
        }

        return "";
    }

}
