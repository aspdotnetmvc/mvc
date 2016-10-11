using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHCRM.Root.Account
{
    public partial class AccountAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "System.Account.Add";
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
            List<Model.Role> list = BLL.Role.GetRoles();
            foreach (var v in list)
            {
                cb_roles.Items.Add(new ListItem(v.RoleName, v.RoleID));
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txt_code.Text.Trim() == "admin")
            {
                Message.Alert(this, "已存在此用户", "null");
                return;
            }
            bool ok = BLL.SysAccount.Exists(txt_code.Text.Trim());
            if (ok)
            {
                Message.Alert(this, "已存在此用户", "null"); 
                return;
            }
            if (txt_code.Text == "")
            {
                Message.Alert(this, "请填写帐号", "null");
                return;
            }
            if (txt_name.Text == "")
            {
                Message.Alert(this, "请填写姓名", "null");
                return;
            }
            if (txt_pass.Text == "")
            {
                Message.Alert(this, "请填写帐号密码", "null");
                return;
            }
            Model.SysAccount account = new Model.SysAccount();
            account.UserCode = txt_code.Text.Trim();
            account.UserName = txt_name.Text.Trim();
            account.Status = rb_defalut.SelectedItem.Value == "0" ? false : true;
            account.PassWord = txt_pass.Text.Trim();
            account.AddTime = DateTime.Now;
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
                Message.Alert(this, "请给帐号选个角色", "null");
                return;
            }
            //BLL.SysAccount.Add(account);
            //Message.Success(this, "添加成功", "null");
            SMSModel.RPCResult r = ZHSMSPlatform.Root.ZHSMSProxy.GetZHSMSPlatService().AddSysAccount(account);
            if (r.Success)
            {
                Message.Success(this, "添加成功", "null");
            }
            else
            {
                Message.Alert(this, "添加失败");
            }
        }
    }
}