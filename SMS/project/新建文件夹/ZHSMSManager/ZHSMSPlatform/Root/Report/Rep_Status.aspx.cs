using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SMSModel;
using System.Data;

namespace ZHSMSPlatform.Root.Report
{
    public partial class Rep_Status : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                this.ViewState["Permissions"] = BLL.Permission.GetPermissionByAccount(account.UserCode);
            }
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
            }
            return str;
        }

        private void load()
        {
            string se = Request.QueryString["SerialNumber"].ToString();
            DataTable dt = CreateTable();
            RPCResult<List<StatusReport>> r = ZHSMSProxy.GetZHSMSPlatService().GetSMSStatusReport(Guid.Parse(se), DateTime.Parse(Request.QueryString["SendTime"]));
            if (r.Success)
            {
                lbl_message.Visible = false;
                foreach (StatusReport s in r.Value)
                {
                    DataRow dr = dt.NewRow();
                    dr["Serial"] = s.Serial;
                    dr["Describe"] = s.Describe;
                    dr["Gateway"] = s.Gateway;
                    SMS sm = s.Message;
                    dr["Audit"] = GetAudit(sm.Audit);
                    dr["SMSContent"] = sm.Content;
                    dr["ContentFilter"] = GetContentFilter(sm.Filter);
                    dr["Level"] = sm.Level;
                    dr["SendTime"] = sm.SendTime;
                    dr["StatusReport"] = GetStatusReport(sm.StatusReport);
                    List<string> num = sm.Number;
                    string str = "";
                    foreach (string st in num)
                    {
                        str += st + ",";
                    }
                    dr["PhoneCount"] = str == "" ? "" : str.Substring(0, str.Length - 1);
                    dr["ReportStatus"] = s.StatusCode;
                    dr["ResponseTime"] = s.ResponseTime.ToString();
                    dr["SplitNumber"] = s.SplitNumber;
                    dr["SplitTotal"] = s.SplitTotal;
                    dr["Succeed"] = s.Succeed == true ? "是" : "否";
                    dt.Rows.Add(dr);
                }

            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
            dt.DefaultView.Sort = "ResponseTime desc";
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }


        //string GetReportStatus(ReportStatusType a)
        //{
        //    string str = "";
        //    switch (a)
        //    {
        //        case ReportStatusType.Response:
        //            str = "已经发回报告";
        //            break;
        //        case ReportStatusType.Send:
        //            str = "已经发送";
        //            break;

        //    }
        //    return str;
        //}

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView1.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
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


        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("Describe", Type.GetType("System.String"));
            table.Columns.Add("Gateway", Type.GetType("System.String"));
            table.Columns.Add("Message", Type.GetType("System.String"));
            table.Columns.Add("ReportStatus", Type.GetType("System.String"));
            table.Columns.Add("ResponseTime", Type.GetType("System.String"));
            table.Columns.Add("Serial", Type.GetType("System.String"));
            table.Columns.Add("SplitNumber", Type.GetType("System.String"));
            table.Columns.Add("SplitTotal", Type.GetType("System.String"));
            table.Columns.Add("Succeed", Type.GetType("System.String"));
            table.Columns.Add("SMSID", Type.GetType("System.String"));
            table.Columns.Add("AccountID", Type.GetType("System.String"));
            table.Columns.Add("SMSContent", Type.GetType("System.String"));
            table.Columns.Add("StatusReport", Type.GetType("System.String"));
            table.Columns.Add("Level", Type.GetType("System.String"));
            table.Columns.Add("SendTime", Type.GetType("System.String"));
            table.Columns.Add("Audit", Type.GetType("System.String"));
            table.Columns.Add("ContentFilter", Type.GetType("System.String"));
            table.Columns.Add("AuditTime", Type.GetType("System.String"));
            table.Columns.Add("PhoneCount", Type.GetType("System.String"));
            table.Columns.Add("BussType", Type.GetType("System.String"));
            return table;
        }

    }
}