using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.SMSM
{
    public partial class TempletAudit_Failure : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "Templet.Audit.Audit";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    btn_Submit.Enabled = false;
                    return;
                }
            }
            if (!IsPostBack)
            {
                load();
            }
        }

        void load()
        {
            string templetID = Request.QueryString["TempletID"];
            SMSModel.RPCResult<Model.SMSTemplet> r = ZHSMSProxy.GetZHSMSPlatService().GetSMSTemplet(templetID);
            if (r.Success)
            {
                lbl_account.Text = r.Value.AccountName;
                lbl_content.Text = r.Value.TempletContent;
                lbl_submit.Text = r.Value.SubmitTime.ToString();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_cause.Text.Trim()))
            {
                Message.Alert(this, "请填写原因","null");
                return;
            }
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            string templetID = Request.QueryString["TempletID"];
            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AuditSMSTemplet(templetID, account.UserCode, false, txt_cause.Text);
            if (r.Success)
            {
                Message.Success(this, "操作成功", "null");
                Response.Redirect("Templet_Audit.aspx");
            }
            else
            {
                Message.Success(this, r.Message, "null");
            }
        }

        protected void btn_back_Click(object sender, EventArgs e)
        {
            Response.Redirect("Templet_Audit.aspx");
        }
    }
}