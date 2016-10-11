using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Enterprise
{
    public partial class EnterpriseRecharge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = Request.QueryString["type"] + "Enterprise.Manage.ReCharge";
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
                string code = Request.QueryString["AccountID"];
                SMSModel.RPCResult<Model.EnterpriseUser> rp = ZHSMSProxy.GetZHSMSPlatService().GetEnterprise(code);
                if (rp.Success)
                {
                    Model.EnterpriseUser user = rp.Value;// BLL.EnterpriseUser.GetEnerprise(code);
                    SMSModel.RPCResult<Model.UserBalance> r = ZHSMSProxy.GetZHSMSPlatService().GetBalance(code, user.Password);// ZHSMSProxy.GetPretreatmentService().GetAccount(user.AccountID);
                    if (r.Success)
                    {
                        lbl_Old.Text =r.Value.SmsBalance.ToString();
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Model.ChargeRecord model = new Model.ChargeRecord();
            model.OperatorAccount = ((Model.SysAccount)Session["Login"]).UserCode;
            model.PrepaidTime = DateTime.Now;
            model.PrepaidType = ushort.Parse(rb_type.SelectedValue);
            model.ChargeFlag = 0;
            model.Remark = txt_remark.Text.Trim();
            model.PrepaidAccount = Request.QueryString["AccountID"];

            int count = 0;
            decimal money = 0;
            try
            {
                if (rb_type.SelectedItem.Value == "0")
                {
                    //金额
                    money = decimal.Parse(txt_sms.Text.Trim());
                    count = (int)(decimal.Parse(txt_sms.Text.Trim()) / decimal.Parse((txt_SMSRate.Text.Trim())));

                }
                else
                {
                    //短信条数
                    count = int.Parse(txt_sms.Text.Trim());
                    money = decimal.Parse(txt_sms.Text.Trim()) * decimal.Parse((txt_SMSRate.Text.Trim()));
                }
            }
            catch
            {
                Message.Alert(this, "请输入数字", "null");
                return;
            }
            model.ThenRate = decimal.Parse(txt_SMSRate.Text.Trim());
            model.SMSCount = count;
            model.Money = money;
            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AccountPrepaid(model);
            if (r.Success)
            {
                Message.Alert(this, "充值成功", "null");
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
        }
    }
}