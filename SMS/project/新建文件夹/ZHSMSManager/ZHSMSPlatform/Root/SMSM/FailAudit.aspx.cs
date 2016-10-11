using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMSModel;
using ZHSMSPlatform.Root;
namespace ZHCRM.Root.SMSM
{
    public partial class FailAudit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (txt_reason.Text == "")
            {
                Message.Alert(this, "请输入审核失败原因", "null");
                return;
            }
            int j = 0;
            List<string> senum = new List<string>();
            senum = (List<string>)Session["senum"];
            foreach (string serialNumber in senum)
            {
                j++;
                RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AuditSMS(Guid.Parse(serialNumber), false, account.UserCode, txt_reason.Text.ToString());
            }
            if (j > 0)
            {
                Response.Redirect("SMS_Audit.aspx");
                //Message.Alert(this, "审核失败成功", "null");
                return;
            }
            else
            {
                Message.Alert(this, "审核失败失败", "null");
                return;
            }
        }
    }
}