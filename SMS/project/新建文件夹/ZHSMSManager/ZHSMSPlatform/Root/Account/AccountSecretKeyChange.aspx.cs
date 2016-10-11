using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Account
{
    public partial class AccountSecretKeyChange : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account != null)
                {
                    account = BLL.SysAccount.GetAccount(account.UserCode);
                    lbl_code.Text = account.UserCode;
                    //if (string.IsNullOrEmpty(account.SecretKey))
                    //{
                    //    lbl_oldSecretKey.Text = "使用的系统默认密钥";
                    //}
                    //else
                    //{
                    //    lbl_oldSecretKey.Text = account.SecretKey;
                    //}
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string secretKey = txt_newSecretKey.Text.Trim();
            if (txt_pass.Text.Trim() == "")
            {
                Message.Alert(this, "密码不能为空", "null");
                return;
            }
            if (string.IsNullOrEmpty(secretKey))
            {
                Message.Alert(this, "新密钥不能为空", "null");
                return;
            }
            bool flag = Regex.IsMatch(secretKey, @"^\w+$");
            if (flag)
            {
                if (secretKey.Length != 16)
                {
                    Message.Alert(this, "请输入长度为16为的密钥！", "null");
                    return;
                }
            }
            else
            {
                Message.Alert(this, "密钥有字母和数字组成！", "null");
                return;
            }
            //Model.SysAccount account = BLL.SysAccount.GetAccount(lbl_code.Text);
            //if (account != null)
            //{
            //    if (ZHSMSProxy.GetZHSMSPlatService().EncryptBysecretKey(account.SecretKey, txt_pass.Text.Trim()).Value != account.PassWord)
            //    {
            //        Message.Alert(this, "密码不对", "null");
            //        return;
            //    }
            //   bool ok =  BLL.SysAccount.UpdateSecretKey(account.UserCode, secretKey, ZHSMSProxy.GetZHSMSPlatService().EncryptBysecretKey(secretKey, txt_pass.Text.Trim()).Value);
            //   if (ok)
            //   {
            //       Message.Success(this, "修改成功", "null");
            //       return;
            //   }
            //   else
            //   {
            //       Message.Alert(this, "修改密钥失败", "null");
            //   }
            //}
            //else
            //{
            //    Message.Alert(this, "系统不存在此用户", "null");
            //}
        }
    }
}