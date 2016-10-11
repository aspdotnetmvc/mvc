using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Notice
{
    public partial class NoticeDetail : System.Web.UI.Page
    {
        private Model.SysAccount account;
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                this.ViewState["Permissions"] = BLL.Permission.GetPermissionByAccount(account.UserCode);
            }
            LoadData();
        }
        private void LoadData()
        {
            string AnnunciateID = Request.QueryString["AnnunciateID"];
            string sthml = string.Empty;
            sthml += " <div class=\"gonggao_title\">公告详情</div>";
            sthml += "  <ul class=\"gonggao_con\">";
            SMSModel.RPCResult<Model.Annunciate> no = ZHSMSProxy.GetZHSMSPlatService().GetAnnunciate(AnnunciateID);
            if (no != null)
            {
                sthml += " 	<li class=\"gg_con_title\">";
                sthml += " <a href=\"javascript:void(0);\">";
                sthml += "    <span class=\"title\">" + no.Value.AnnunciateTitle + "</span>";
                sthml += "   <span class=\"time\">" + no.Value.CreateTime + "</span>";
                sthml += " </a>";
                sthml += "  </li>";
                sthml += " <li class=\"gg_con_box\">";
                sthml += " 	<div class=\"gg_open_title\">" + no.Value.AnnunciateTitle + "<br /><span class=\"open_time\">" + no.Value.CreateTime + "</span></div>";
                sthml += "   <div class=\"gg_open_con\">" + no.Value.AnnunciateContent + "<br />";
                sthml += "   </div>";
                sthml += " </li>";
            }
            sthml += " </ul>";
            sthml += "</div>";
            this.tdInfo.InnerHtml = sthml;

        }
    }
}