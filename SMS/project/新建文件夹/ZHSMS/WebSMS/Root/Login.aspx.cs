using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMSModel;

namespace WebSMS.Root
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }



        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text.Trim();
            string userPwd = txtUserPwd.Text.Trim();
          //  string code = txtCode.Text.Trim();
            string userpwd2 = tools.DESEncrypt.Encrypt(userPwd);
            if (userName.Equals("") || userPwd.Equals(""))
            {
                return;
            }
            //if (code.Equals(""))
            //{
            //    lblTip.Visible = true;
            //    lblTip.Text = "请输入验证码";
            //    return;
            //}
            //if (Session[DTKeys.SESSION_CODE] == null)
            //{
            //    lblTip.Visible = true;
            //    lblTip.Text = "系统找不到验证码";
            //    return;
            //}
            //if (code.ToLower() != Session[DTKeys.SESSION_CODE].ToString().ToLower())
            //{
            //    lblTip.Visible = true;
            //    lblTip.Text = "验证码输入不正确";
            //    return;
            //}
            RPCResult<Account> r = PretreatmentProxy.GetPretreatment().GetAccount(userName);
            if (r.Success)
            {
                if (r.Value != null)
                {
                    Account acc = r.Value;
                    if (acc.Password == userpwd2)
                    {
                        Session["AccountID"] = userName;
                        Session["Password"] = userpwd2;
                        Response.Redirect("index.aspx");
                    }
                    Message.Alert(this, "输入的密码错误");
                }
                Message.Alert(this, "用户不存在");
            }

            return;
        }
    }
}