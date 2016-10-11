using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.GatewayConfig
{
    public partial class ChannelEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = "Channel.Manage.Edit";
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
            string channel = Request.QueryString["channel"];
            SMSModel.RPCResult<SMSModel.Channel> r = ZHSMSProxy.GetZHSMSPlatService().GetSMSChannel(channel);
            if (r.Success)
            {
                SMSModel.Channel config = r.Value;
                if (config != null)
                {

                    lbl_channelID.Text = config.ChannelID;
                    txt_channelName.Text = config.ChannelName;
                    txt_remark.Text = config.Remark;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string channel = Request.QueryString["channel"];
            SMSModel.Channel c = new SMSModel.Channel();

            c.ChannelID = channel;
            c.ChannelName = txt_channelName.Text;
            c.Remark = txt_remark.Text;
            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().UpdateChannel(c);

            if (r.Success)
            {
                Message.Success(this, "操作成功", "null");
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
        }
    }
}