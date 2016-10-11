using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using ZKD.Root;
using SMSModel;

namespace ZKD
{
    public partial class center : System.Web.UI.Page
    {
        protected Model.EnterpriseUser user;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                user = (Model.EnterpriseUser)Session["Login"];
                if (user == null)
                {
                    System.Web.HttpContext.Current.Response.Redirect("../login.aspx", true);
                }
                load();
                LoadData();
            }
        }


        void load()
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            if (user.AccountID != null)
            {
                //剩余短信
                SMSModel.RPCResult<Model.UserBalance> rr = ZHSMSProxy.GetZKD().GetBalance(user.AccountCode, user.Password);
                if (rr.Success)
                {
                    lbl_smsCount.Text = rr.Value.SmsBalance.ToString();
                }
                else
                {
                    Message.Alert(this, rr.Message, "null");
                    return;
                }
                int sendcount = 0;
                int failurecount = 0;

                //发送成功，失败，已发送条数
                RPCResult<List<ReportStatistics>> r = ZHSMSProxy.GetZKD().GetDirectStatisticReportByAccount(user.AccountCode);
                if (r.Success)
                {

                    List<ReportStatistics> cc = r.Value;
                    if (cc.Count > 0)
                    {
                        foreach (ReportStatistics s in cc)
                        {
                            failurecount += s.FailureCount;
                            sendcount += s.SendCount;
                        }
                    }
                }
                else
                {
                    Message.Alert(this, r.Message, "null");
                    return;

                }
                lbl_sendCount.Text = sendcount.ToString();
                lbl_lossCount.Text = failurecount.ToString();
                lbl_sucCount.Text = (sendcount - failurecount).ToString();
                //接收短信条数
                DateTime begin = DateTime.Now.AddDays(-1);
                DateTime end = DateTime.Now;
                List<MOSMS> rrr = BLL.MO.Gets(user.SPNumber, begin, end);
                lbl_recCount.Text = rrr.Count.ToString();
                //审核失败条数
                RPCResult<List<FailureSMS>> audits = ZHSMSProxy.GetZKD().GetSMSByAuditFailure(user.AccountCode);
                if (audits.Success)
                {
                    List<FailureSMS> smss = audits.Value;
                    lbl_audit.Text = smss.Count.ToString();
                }
                else
                {
                    Message.Alert(this, audits.Message, "null");
                    return;

                }

                //未处理短信总数
                RPCResult<int> nCount = ZHSMSProxy.GetZKD().GetSMSCountByAccount(user.AccountCode);
                if (nCount.Success)
                {
                    lbl_undealt.Text = nCount.Value.ToString();
                }
                else
                {
                    lbl_undealt.Text = "0";
                }
            }
        }
       
        private void LoadData()
        {
            string sthml = string.Empty;
            sthml += " <div class=\"gonggao_title\">公告列表</div>";
            sthml += "  <ul class=\"gonggao_con\">";
            Model.EnterpriseUser single = (Model.EnterpriseUser)Session["Login"];
            if (user.AccountID != null)
            {
                DateTime a = DateTime.Now;
                DateTime b = DateTime.Now.AddDays(-3);
                SMSModel.RPCResult<List<Model.Annunciate>> no = ZHSMSProxy.GetZKD().GetDirectAnnuciates(user.AccountCode, b, a);
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
}