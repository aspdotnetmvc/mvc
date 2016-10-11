using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.GatewayConfig
{
    public partial class GatewayEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "Gateway.Manage.Edit";
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
            string gateway = Request.QueryString["gateway"];
            SMSModel.RPCResult<SMSModel.GatewayConfiguration> r = ZHSMSProxy.GetZHSMSPlatService().GetGatewayConfig(gateway);
            if (r.Success)
            {
                SMSModel.GatewayConfiguration config = r.Value;
                if (config != null)
                {
                    lbl_gateway.Text = config.Gateway;
                    foreach (var v in config.Operators)
                    {
                        if (cb_operators.Items.FindByValue(v) != null)
                        {
                            cb_operators.Items.FindByValue(v).Selected = true;
                        }
                    }
                    txt_handlity.Text = config.HandlingAbility.ToString();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string gateway = Request.QueryString["gateway"];
            SMSModel.RPCResult<SMSModel.GatewayConfiguration> rpc = ZHSMSProxy.GetZHSMSPlatService().GetGatewayConfig(gateway);
            if (rpc.Success)
            {
                SMSModel.GatewayConfiguration config = rpc.Value;
                config.Operators.Clear();
                foreach (ListItem li in cb_operators.Items)
                {
                    if (li.Selected == true)
                    {
                        config.Operators.Add(li.Value);
                    }
                }
                if (config.Operators.Count==0)
                {
                    Message.Alert(this, "请选择运营商", "null");
                    return;
                }
                config.HandlingAbility = int.Parse(txt_handlity.Text);
                SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().UpdateGatewayConfig(config);
                if (r.Success)
                {
                    Message.Success(this, "操作成功", "null");
                }
                else
                {
                    Message.Alert(this, r.Message, "null");
                }
            }
            else
            {
                Message.Alert(this, rpc.Message, "null");
            }
        }
    }
}