using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using ZHSMSPlatform.Root;

namespace ZHCRM
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                num.Value = Utils.GetCookie("ZHSMSPlatName");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string userName = num.Value.Trim();
            string userPwd = psw.Value.Trim();
            string codes = code.Value.Trim();

            if (userName.Equals("") || userPwd.Equals(""))
            {
                lblTip.Visible = true;
                lblTip.Text = "请输入用户名或密码";
                return;
            }
            if (codes.Equals(""))
            {
                lblTip.Visible = true;
                lblTip.Text = "请输入验证码";
                return;
            }
            if (Session[DTKeys.SESSION_CODE] == null)
            {
                lblTip.Visible = true;
                lblTip.Text = "系统找不到验证码";
                return;
            }
            if (codes.ToLower() != Session[DTKeys.SESSION_CODE].ToString().ToLower())
            {
                lblTip.Visible = true;
                lblTip.Text = "验证码输入不正确";
                code.Value = "";
                return;
            }
            //Model.SysAccount account = BLL.SysAccount.GetAccount(userName);
            //if (account == null)
            //{
            //    lblTip.Visible = true;
            //    lblTip.Text = "用户不存在";
            //    return;
            //}
            int ok = BLL.Login.Logon(num.Value.Trim(),  psw.Value.Trim());
            if (ok == 1)
            {
                //写入Cookies
                if (cbRememberId.Checked)
                {
                    Utils.WriteCookie("ZHSMSPlatName", userName, 14400);
                }
                else
                {
                    Utils.WriteCookie("ZHSMSPlatName", userName, -14400);
                }
                Response.Redirect("Root/index.aspx");
            }
            else
            {
                lblTip.Visible = true;
                lblTip.Text = ok == 0 ? "用户名或密码错误" : "帐号已被禁用";
                return;
            }

        }
    }
}