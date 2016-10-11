using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHCRM.Root.Account
{
    public partial class AccountEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "System.Account.Edite";
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
            string usercode = Request.QueryString["accountID"];
            Model.SysAccount account = BLL.SysAccount.GetAccount(usercode);
            if (account != null)
            {
                lbl_accountID.Text = account.UserCode;
                txt_name.Text = account.UserName;
                rb_defalut.Items.FindByValue(account.Status == true ? "1" : "0").Selected = true;
            }
            List<Model.Role> list = BLL.Role.GetRoles();
            foreach (var v in list)
            {
                cb_roles.Items.Add(new ListItem(v.RoleName, v.RoleID));
            }
            foreach (string s in account.Roles)
            {
                cb_roles.Items.FindByValue(s).Selected = true;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Model.SysAccount account = BLL.SysAccount.GetAccount(Request.QueryString["accountID"]);
            account.UserName = txt_name.Text.Trim();
            account.Status = rb_defalut.SelectedItem.Value == "0" ? false : true;
            account.Roles = new List<string>();
            for (int i = 0; i < cb_roles.Items.Count; i++)
            {
                if (cb_roles.Items[i].Selected)
                {
                    account.Roles.Add(cb_roles.Items[i].Value);
                }
            }
            if (account.Roles.Count == 0)
            {
                Message.Alert(this, "请给帐号分配个角色", "null");
                return;
            }

           // BLL.SysAccount.Update(account);
            SMSModel.RPCResult r = ZHSMSPlatform.Root.ZHSMSProxy.GetZHSMSPlatService().UpdateSysAccount(account);
            if (r.Success)
            {
                Message.Success(this, "修改成功", "null");
            }
            else
            {
                Message.Alert(this,"修改失败");
            }
        }
    }
}