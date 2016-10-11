using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.GatewayConfig
{
    public partial class KeygroupGateWayBind : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "Gateway.Manage.RelateGateway";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    btnSubmit.Visible = false;
                    return;
                }
            }
            if (!IsPostBack)
            {
                load();
            }
        }
        private void load()
        {
            string gateway = Request.QueryString["gateway"];
            lbl_gateway.Text = gateway;
            SMSModel.RPCResult<Dictionary<string, string>> r = ZHSMSProxy.GetZHSMSPlatService().GetKeyGroups();
            SMSModel.RPCResult<string> ra = ZHSMSProxy.GetZHSMSPlatService().GetKeyGroupGatewayBinds(gateway);
            if (r.Success && ra.Success)
            {
                foreach (var v in r.Value)
                {
                    ListItem li = new ListItem();
                    li.Value = v.Key;
                    li.Text = v.Key;
                    rb_keyGroup.Items.Add(li);
                    if (ra.Value.Contains(li.Value))
                    {
                        li.Selected = true;
                    }
                }
            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string gateway = Request.QueryString["gateway"];
            string keyGroup = "";
            foreach (ListItem li in rb_keyGroup.Items)
            {
                if (li.Selected == true)
                {
                    keyGroup = li.Value;
                }
            }
            if (keyGroup == "")
            {
                Message.Alert(this, "请选择要关联的关键词组", "null");
                return;
            }
            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AddkeyGroupGatewayBind(keyGroup, gateway);
            if (r.Success)
            {
                Message.Success(this, "操作成功", "null");
                return;
            }
            Message.Alert(this, r.Message, "null");
        }
    }
}