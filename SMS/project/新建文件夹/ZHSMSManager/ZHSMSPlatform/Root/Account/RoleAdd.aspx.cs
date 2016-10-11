using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHCRM.Root.Account
{
    public partial class RoleAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "System.Role.Add";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool ok = BLL.Role.Exists(txt_roleCode.Text.Trim());
            if (ok)
            {
                Message.Alert(this, "已存在此角色代码", "null");
                return;
            }
            Model.Role role = new Model.Role();
            role.RoleName = txt_roleName.Text.Trim();
            role.RoleID = txt_roleCode.Text.Trim();
            role.Remark = txt_remar.Text.Trim();
            role.AddTime = DateTime.Now;
            BLL.Role.Add(role);
            Message.Alert(this, "操作成功", "null");
        }
    }
}