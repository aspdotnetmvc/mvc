using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SMSModel;
using System.Data;
using StatusReportInterface;

namespace WebSMS.Root.Report
{
    public partial class Rep_AuditRecords : JudgeSession
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }
        string GetStatusReport(StatusReportType a)
        {
            string str = "";
            switch (a)
            {
                case StatusReportType.Enabled:
                    str = "发送";
                    break;
                case StatusReportType.Disable:
                    str = "不发送";
                    break;
                case StatusReportType.Push:
                    str = "推送";
                    break;
            }
            return str;
        }
        string GetContentFilter(FilterType a)
        {
            string str = "";
            switch (a)
            {
                case FilterType.NoOperation:
                    str = "无操作";
                    break;
                case FilterType.Failure:
                    str = "发送失败";
                    break;
                case FilterType.Replace:
                    str = "替换";
                    break;
            }
            return str;
        }
        string GetAudit(AuditType a)
        {
            string str = "企业鉴权";
            switch (a)
            {
                case AuditType.Auto:
                    str = "自动审核";
                    break;
                case AuditType.Manual:
                    str = "人工审核";
                    break;
                case AuditType.Enterprise:
                    str = "企业鉴权";
                    break;
            }
            return str;
        }
        string GetBusstype(ChannelType a)
        {
            string str = "";
            switch (a)
            {
                case ChannelType.Commercial:
                    str = "商业短息";
                    break;
                case ChannelType.Industry:
                    str = "行业短信";
                    break;
            }
            return str;
        }
        private void load()
        {
            string se = Request.QueryString["SerialNumber"].ToString();
            DataTable dt = CreateTable();
            RPCResult<AuditRecord> r = PretreatmentProxy.GetStatusReportService().GetAuditRecord(Guid.Parse(se));
            if (r.Success)
            {
                if (r.Value != null)
                {
                    AuditRecord s = r.Value;
                    DataRow dr = dt.NewRow();
                    dr["AccountID"] = s.AccountID;
                    dr["SerialNumber"] = s.SerialNumber;
                    dr["Content"] = s.Content;
                    dr["AuditTime"] = s.AuditTime;
                    dr["Result"] = s.Result == true ? "成功" : "失败";
                    dt.Rows.Add(dr);
                    lbl_message.Visible = false;
                }
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
            Session["dt"] = dt;

        }


      

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        public int CurrentPage
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
            GridViewRow pagerRow = GridView1.BottomPagerRow;
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            GridView1.PageIndex = pageList.SelectedIndex;
            this.CurrentPage = pageList.SelectedIndex;
            load();
        }
        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            try
            {
                GridViewRow pagerRow = GridView1.BottomPagerRow;
                LinkButton linkBtnFirst = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnFirst");
                LinkButton linkBtnPrev = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnPrev");
                LinkButton linkBtnNext = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnNext");
                LinkButton linkBtnLast = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnLast");
                if (GridView1.PageIndex == 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                }
                else if (GridView1.PageIndex == GridView1.PageCount - 1)
                {
                    linkBtnLast.Enabled = false;
                    linkBtnNext.Enabled = false;
                }
                else if (GridView1.PageCount <= 0)
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
                    for (int i = 0; i < GridView1.PageCount; i++)
                    {
                        int pageNumber = i + 1;
                        ListItem item = new ListItem(pageNumber.ToString() + "/" + GridView1.PageCount.ToString(), pageNumber.ToString());
                        if (i == GridView1.PageIndex)
                        {
                            item.Selected = true;
                        }
                        pageList.Items.Add(item);
                    }
                }
                if (pageLabel != null)
                {
                    int currentPage = GridView1.PageIndex + 1;
                    pageLabel.Text = "当前页： " + currentPage.ToString() +
                      " / " + GridView1.PageCount.ToString();
                }
            }
            catch
            {
            }
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            load();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            load();
        }

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("SerialNumber", Type.GetType("System.String"));
            table.Columns.Add("AccountID", Type.GetType("System.String"));
            table.Columns.Add("Content", Type.GetType("System.String"));
            table.Columns.Add("AuditTime", Type.GetType("System.String"));
            table.Columns.Add("Result", Type.GetType("System.String"));
            return table;
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {


        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {


        }



    }
}