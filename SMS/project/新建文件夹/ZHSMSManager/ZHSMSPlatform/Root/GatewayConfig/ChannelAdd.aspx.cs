using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.GatewayConfig
{
    public partial class ChannelAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "Channel.Manage.Add";
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
            SMSModel.Channel c = new SMSModel.Channel();

            c.ChannelID = txt_channelID.Text.Trim();
            c.ChannelName = txt_channelName.Text.Trim();
            c.Remark = txt_remark.Text.Trim();
            if (c.ChannelID == "-1-")
            {
                Message.Alert(this, "已存在此短信通道", "null");
                return;
            }
            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().AddChannel(c);
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