using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class EnterpriseBalance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = Request.QueryString["type"] + "Enterprise.Manage.ReBlance";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
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
            lbl_account.Text = Request.QueryString["AccountID"];
            SMSModel.RPCResult<Model.EnterpriseUser> rp = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(lbl_account.Text);
            if (rp.Success)
            {
                SMSModel.RPCResult<Model.UserBalance> r = ZHSMSProxy.GetZHSMSPlatService().GetBalance(lbl_account.Text, rp.Value.Password);
                if (r.Success)
                {
                    lbl_smsCount.Text = r.Value.SmsBalance.ToString();
                }
            }
        }
    }
}