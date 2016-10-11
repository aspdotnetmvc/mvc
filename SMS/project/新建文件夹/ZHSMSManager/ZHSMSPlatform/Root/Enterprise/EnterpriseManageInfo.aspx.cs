using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class EnterpriseManageInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                this.ViewState["Permissions"] = BLL.Permission.GetPermissionByAccount(account.UserCode);
            }
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {
            string strEC = Request.QueryString["EnterpriseCode"];
            lblAgent.Text = strEC;
            List<Model.SysAccount> accounts = BLL.SysAccount.GetAccounts();
            if (accounts.Count > 0)
            {
                accounts = accounts.OrderByDescending(c => c.AddTime).ToList();
                foreach (Model.SysAccount account in accounts)
                {
                    ddlChannel.Items.Add(new ListItem(account.UserName,account.UserCode));
                    ddlCS.Items.Add(new ListItem(account.UserName,account.UserCode));
                }

                List<Model.EnterpriseManage> em = BLL.EnterpriseManage.GetEnManageByEnCode(strEC);
                if (em.Count > 0)
                {
                    foreach(Model.EnterpriseManage emInfo in em)
                    {
                        ddlChannel.SelectedValue = emInfo.ChannelManager;
                        ddlCS.SelectedValue = emInfo.CSManager;
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Model.EnterpriseManage em = new Model.EnterpriseManage();
            em.EnterpriseCode = lblAgent.Text;
            em.ChannelManager = ddlChannel.SelectedValue;
            em.CSManager = ddlCS.SelectedValue;
            em.Reserve = "";

            if (BLL.EnterpriseManage.Add(em))
            {
                Message.Success(this, "设置成功", "null");
                Response.Redirect("AgentEnterpriseManage.aspx");
            }
            else
            {
                Message.Success(this, "设置失败", "null");
            }
        }
    }
}