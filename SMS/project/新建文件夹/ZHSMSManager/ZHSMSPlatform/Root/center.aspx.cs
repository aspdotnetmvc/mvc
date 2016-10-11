using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;

namespace ZHCRM
{
    public partial class center : System.Web.UI.Page
    {
        protected Model.SysAccount account;

        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!Page.IsPostBack)
            {
                account = (Model.SysAccount)Session["Login"];
                DateTime a = DateTime.Now;
                DateTime b = DateTime.Now.AddDays(-3);
                LoadData(a,b);
            }
        }
        private void LoadData(DateTime beg, DateTime end)
        {
            string sthml = string.Empty;
            sthml += " <div class=\"gonggao_title\">公告列表</div>";
            sthml += "  <ul class=\"gonggao_con\">";
            SMSModel.RPCResult<List<Model.Annunciate>> no = ZHSMSPlatform.Root.ZHSMSProxy.GetZHSMSPlatService().GetAunnuciateByMangge(beg, end);
            if (no != null)
            {
                if (no.Value.Count > 0)
                {
                    foreach (Model.Annunciate g in no.Value)
                    {
                        sthml += " 	<li class=\"gg_con_title\">";
                        sthml += " <a href=\"javascript:void(0);\">";
                        sthml += "    <span class=\"title\">" + g.AnnunciateTitle + "</span>";
                        sthml += "   <span class=\"time\">" + g.CreateTime + "</span>";
                        sthml += " </a>";
                        sthml += "  </li>";
                        sthml += " <li class=\"gg_con_box\">";
                        sthml += " 	<div class=\"gg_open_title\">" + g.AnnunciateTitle + "<br /><span class=\"open_time\">" + g.CreateTime + "</span></div>";
                        sthml += "   <div class=\"gg_open_con\">" + g.AnnunciateContent + "<br />";
                        sthml += "   </div>";
                        sthml += " </li>";
                    }
                }
            }
            sthml += " </ul>";
            sthml += "</div>";
            this.tdInfo.InnerHtml = sthml;

        }
    }
}