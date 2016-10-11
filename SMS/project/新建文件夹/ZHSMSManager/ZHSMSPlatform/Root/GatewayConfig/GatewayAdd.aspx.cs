using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.GatewayConfig
{
    public partial class GatewayAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "Gateway.Manage.Add";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            if (!IsPostBack)
            {
                //load();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SMSModel.GatewayConfiguration config = new SMSModel.GatewayConfiguration();
            config.Operators = new List<string>();
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
            config.Gateway = txt_gateway.Text;
            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AddGatewayConfig(config);
            if (r.Success)
            {
                Message.Success(this, "添加成功", "null");
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
        }
    }
}