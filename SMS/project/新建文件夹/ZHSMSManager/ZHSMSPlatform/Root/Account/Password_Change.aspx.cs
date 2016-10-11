using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZHSMSPlatform.Root;

namespace ZHCRM.Root.Account
{
    public partial class Password_Change : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account != null)
                {
                    lbl_code.Text = account.UserCode;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txt_newpass.Text.Trim() == "")
            {
                Message.Alert(this, "新密码不能为空", "null");
                return;
            }
            if (txt_confirm.Text != txt_newpass.Text)
            {
                Message.Alert(this, "新密码与确认密码不一致", "null");
                return;
            }
            Model.SysAccount account = BLL.SysAccount.GetAccount(lbl_code.Text);
            if (account != null)
            {
                if (BLL.SysAccount.GetEncrypt(txt_oldpass.Text.Trim()) != account.PassWord)
                {
                    Message.Alert(this,"原始密码不对", "null");
                    return;
                }
                BLL.SysAccount.ChanagePass(account.UserCode, txt_newpass.Text.Trim());
                Message.Success(this, "修改成功", "null");
                return;
            }
            else
            {
                Message.Alert(this, "系统不存在此用户", "null");
            }
        }
    }
}