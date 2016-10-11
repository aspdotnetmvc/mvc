using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class EnterprisePass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = Request.QueryString["type"] + "Enterprise.Manage.PassReset";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            if (!IsPostBack)
            {
                lbl_account.Text = Request.QueryString["AccountID"]; 
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string account = Request.QueryString["AccountID"];
            if(!txt_Pass.Text.Equals(txt_Pass2.Text))
            {
                Message.Alert(this, "确认密码与新密码不一致，请重新输入", "null");
                return;
            }
            if (!IsPassword(txt_Pass.Text))
            {
                Message.Alert(this, "密码必须是以字母开头，长度在6~18之间，只能包含字母、数字和下划线", "null");
                return;
            }
            SMSModel.RPCResult ok = ZHSMSProxy.GetZHSMSPlatService().ResetEnterprisePass(account, txt_Pass.Text.Trim());
            if (ok.Success)
            {
                Message.Success(this, "操作成功", "null");
            }
            else
            {
                Message.Alert(this, ok.Message, "null");
            }
        }
        private bool IsPassword(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z]\w{5,17}$");
            if (reg1.IsMatch(str))
            {
                //数字
                return true;
            }
            else
            {
                //非数字
                return false;
            }
        }
    }
}