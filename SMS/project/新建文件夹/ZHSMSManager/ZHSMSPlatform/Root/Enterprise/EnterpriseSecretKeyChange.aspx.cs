using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class EnterpriseSecretKeyChange : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = Request.QueryString["type"] + "Enterprise.Manage.SecretKeyReset";
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
                SMSModel.RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(lbl_account.Text);
                if (r.Success)
                {
                    if (string.IsNullOrEmpty(r.Value.SecretKey))
                    {
                        lbl_oldSecretKey.Text = "使用企业默认密钥";
                    }
                    else
                    {
                        lbl_oldSecretKey.Text = r.Value.SecretKey;
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //^\w+$
            string secretKey = txt_newSecretKey.Text.Trim();
            string account = Request.QueryString["AccountID"];
            if (secretKey == "")
            {
                Message.Alert(this, "密钥不能为空", "null");
                return;
            }
            if (secretKey.Length != 16)
            {
                Message.Alert(this, "请输入长度为16位的密钥！", "null");
                return;
            }
            //bool flag = Regex.IsMatch(secretKey, @"^\w+$");
            //if (flag)
            //{
                
            //}
            //else
            //{
            //    Message.Alert(this, "密钥由字母和数字组成！", "null");
            //    return;
            //}
            SMSModel.RPCResult ok = ZHSMSProxy.GetZHSMSPlatService().UpdateAccountSecretKey(account, secretKey);
            if (ok.Success)
            {
                Message.Success(this, "操作成功", "null");
            }
            else
            {
                Message.Alert(this, ok.Message, "null");
            }
        }
    }
}