using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using ZKD.Root;
using SMSModel;

namespace ZKD.Root.Notice
{
    public partial class RecNotice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                DateTime a = DateTime.Now;
                DateTime b = DateTime.Now.AddDays(-3);
                LoadData(a, b);
            }
        }
        private void LoadData(DateTime beg, DateTime end)
        {
            string sthml = string.Empty;
            //sthml += " <div class=\"gonggao_title\"></div>";
            sthml += "  <ul class=\"gonggao_con\">";
            Model.EnterpriseUser single = (Model.EnterpriseUser)Session["Login"];
            if (single.AccountID != null)
            {
                SMSModel.RPCResult<List<Model.Annunciate>> no = ZHSMSProxy.GetZKD().GetDirectAnnuciates(single.AccountCode, beg, end);
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

        protected void btn_nn_Click(object sender, EventArgs e)
        {
            DateTime beg = Convert.ToDateTime(txt_S.Text);
            DateTime end = Convert.ToDateTime(txt_E.Text);

            if (DateTime.Compare(beg, end) >= 0)
            {
                Message.Alert(this, "开始时间应小于结束时间", "null");
                return;
            }
            LoadData(beg, end);
        }
    }
}