using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class AssignAgent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "NoAgentLowerEnterprise.Manage.assignAgent";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    btnSubmit.Visible = false;
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
            lbl_account.Text = Request.QueryString["accountID"];
            SMSModel.RPCResult<List<Model.EnterpriseUser>> r = ZHSMSProxy.GetZHSMSPlatService().GetAgentEnterprises();
            SMSModel.RPCResult<Model.EnterpriseUser> rr = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(lbl_account.Text);
            if (r.Success)
            {
                foreach (var v in r.Value)
                {
                    ListItem li = new ListItem();
                    li.Value = v.AccountCode;
                    li.Text = v.Name;
                    dd_agents.Items.Add(li);
                }
                dd_agents.Items.Insert(0, new ListItem("--请选择--", "-2"));
            }
            if (rr.Success)
            {
                if (dd_agents.Items.FindByValue(rr.Value.ParentAccountCode) != null)
                {
                    dd_agents.Items.FindByValue(rr.Value.ParentAccountCode).Selected = true;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (dd_agents.SelectedIndex == 0)
            {
                Message.Alert(this, "请选择代理商企业", "null");
                return;
            }
            SMSModel.RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(lbl_account.Text);
            if (r.Success)
            {
                Model.EnterpriseUser user = r.Value;
                user.ParentAccountCode = dd_agents.SelectedValue;
                user.IsAgent = false;
                SMSModel.RPCResult rt = ZHSMSProxy.GetZHSMSPlatService().UpdateAccontInfo(user);
                if (rt.Success)
                {
                    Message.Success(this, "操作成功", "null");
                    return;
                }
                else
                {
                    Message.Alert(this, rt.Message, "null");
                }
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
        }
    }
}