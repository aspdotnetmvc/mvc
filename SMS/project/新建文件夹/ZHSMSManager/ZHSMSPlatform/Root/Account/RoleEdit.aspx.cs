using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHCRM.Root.Account
{
    public partial class RoleEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "System.Role.Edite";
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
            string roleID= Request.QueryString["roleID"];
            Model.Role role = BLL.Role.GetRole(roleID);
            if (role != null)
            {
                lbl_roleCode.Text = role.RoleID;
                txt_remark.Text = role.Remark;
                txt_roleName.Text = role.RoleName;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Model.Role role = BLL.Role.GetRole(Request.QueryString["roleID"]);
            role.RoleName = txt_roleName.Text.Trim();
            role.Remark = txt_remark.Text.Trim();
            BLL.Role.Update(role);
            Message.Alert(this, "操作成功", "null");
        }
    }
}