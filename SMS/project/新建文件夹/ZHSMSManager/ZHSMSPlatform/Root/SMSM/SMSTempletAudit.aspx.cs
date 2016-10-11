using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SMSModel;

namespace ZHSMSPlatform.Root.SMSM
{
    public partial class SMSTempletAudit : System.Web.UI.Page
    {
        private const string audit = "Templet.Audit.AuditHistory";
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            this.ViewState["audit"] = true;
            if (account.UserCode != "admin")
            {
                List<string> permissions = BLL.Permission.GetPermissionByAccount(account.UserCode);
                if (!permissions.Contains(audit))
                {
                    this.ViewState["audit"] = false;
                }
            }

            if (!IsPostBack)
            {
                txt_S.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss");
                txt_E.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        protected void gvSMSTemplet_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSMSTemplet.PageIndex = e.NewPageIndex;
            search();
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            search();
        }

         private void search()
        {
            DataTable dt = CreateTable();
            
            SMSModel.RPCResult<List<Model.SMSTemplet>> r =null; 
            if (rbtnAll.Checked)
            {
                //全部
                r=ZHSMSProxy.GetZHSMSPlatService().GetAllSMSTemplet(DateTime.Parse(txt_S.Text), DateTime.Parse(txt_E.Text));
            }
            else if (rbtnSuccess.Checked)
            {
                //审核通过
                r = ZHSMSProxy.GetZHSMSPlatService().GetSuccessSMSTemplet(null,DateTime.Parse(txt_S.Text), DateTime.Parse(txt_E.Text));
            }
            else
            {
                //失败
                r = ZHSMSProxy.GetZHSMSPlatService().GetFailureSMSTemplet(null, DateTime.Parse(txt_S.Text), DateTime.Parse(txt_E.Text));
            }
            if (r.Success)
            {
                List<Model.SMSTemplet> list = r.Value.OrderByDescending(s=>s.SubmitTime).ToList();
                foreach (var s in list)
                {
                    DataRow dr = dt.NewRow();
                    dr["TempletID"] = s.TempletID;
                    dr["AccountCode"] = s.AccountCode;
                    dr["AccountName"] = s.AccountName;
                    dr["TempletContent"] = s.TempletContent + s.Signature;
                    dr["SubmitTime"] = s.SubmitTime.ToString("yyyy-MM-dd HH:mm:ss");
                    dr["AuditTime"] = s.AuditTime;
                    switch (s.AuditState)
                    {
                        case Model.TempletAuditType.NoAudit:
                            dr["AuditState"] = "未审核";
                            break;
                        case Model.TempletAuditType.Success:
                            dr["AuditState"] = "审核成功";
                            break;
                        case Model.TempletAuditType.Failure:
                            dr["AuditState"] = "审核失败";
                            break;
                    }
                    dr["Remark"] = s.Remark;
                    dr["UserCode"] = s.UserCode;
                    dt.Rows.Add(dr);
                }

            }
            gvSMSTemplet.DataSource = dt;
            gvSMSTemplet.DataBind();
        }

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("TempletID", Type.GetType("System.String"));
            table.Columns.Add("AccountCode", Type.GetType("System.String"));
            table.Columns.Add("AccountName", Type.GetType("System.String"));
            table.Columns.Add("TempletContent", Type.GetType("System.String"));
            table.Columns.Add("SubmitTime", Type.GetType("System.String"));
            table.Columns.Add("AuditTime", Type.GetType("System.String"));
            table.Columns.Add("AuditState", Type.GetType("System.String"));
            table.Columns.Add("Remark", Type.GetType("System.String"));
            table.Columns.Add("UserCode", Type.GetType("System.String"));
            return table;
        }

        protected int CurrentPage
        {
            get
            {
                if (ViewState["CurrentPage"] == null)
                    return 0;
                else
                    return (int)ViewState["CurrentPage"];
            }
            set
            {
                ViewState["CurrentPage"] = value;
            }
        }

        protected void PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
        {
            GridViewRow pagerRow = gvSMSTemplet.BottomPagerRow;
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            gvSMSTemplet.PageIndex = pageList.SelectedIndex;
            this.CurrentPage = pageList.SelectedIndex;
            search();
        }

        protected void gvSMSTemplet_DataBound(object sender, EventArgs e)
        {
            try
            {
                GridViewRow pagerRow = gvSMSTemplet.BottomPagerRow;
                LinkButton linkBtnFirst = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnFirst");
                LinkButton linkBtnPrev = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnPrev");
                LinkButton linkBtnNext = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnNext");
                LinkButton linkBtnLast = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnLast");
                if (gvSMSTemplet.PageIndex == 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                }
                else if (gvSMSTemplet.PageIndex == gvSMSTemplet.PageCount - 1)
                {
                    linkBtnLast.Enabled = false;
                    linkBtnNext.Enabled = false;
                }
                else if (gvSMSTemplet.PageCount <= 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                    linkBtnNext.Enabled = false;
                    linkBtnLast.Enabled = false;
                }
                DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
                Label pageLabel = (Label)pagerRow.Cells[0].FindControl("CurrentPageLabel");
                if (pageList != null)
                {
                    for (int i = 0; i < gvSMSTemplet.PageCount; i++)
                    {
                        int pageNumber = i + 1;
                        ListItem item = new ListItem(pageNumber.ToString() + "/" + gvSMSTemplet.PageCount.ToString(), pageNumber.ToString());
                        if (i == gvSMSTemplet.PageIndex)
                        {
                            item.Selected = true;
                        }
                        pageList.Items.Add(item);
                    }
                }
                if (pageLabel != null)
                {
                    int currentPage = gvSMSTemplet.PageIndex + 1;
                    pageLabel.Text = "当前页： " + currentPage.ToString() +
                      " / " + gvSMSTemplet.PageCount.ToString();
                }
            }
            catch
            {
            }
        }
    }
}