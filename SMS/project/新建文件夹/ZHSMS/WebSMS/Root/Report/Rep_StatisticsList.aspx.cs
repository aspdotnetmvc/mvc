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
    public partial class Rep_StatisticsList : JudgeSession
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txt_Start.Text = DateTime.Now.AddDays(-1).ToString();
                txt_End.Text = DateTime.Now.ToString();
            }
        }
        private void load()
        {
            DateTime starttime = Convert.ToDateTime(txt_Start.Text);
            DateTime endtime = Convert.ToDateTime(txt_End.Text);
            if (DateTime.Compare(starttime, endtime) >= 0)
            {
                Message.Alert(this, "开始时间应小于结束时间", "null");
                return;
            }
            DataTable dt = CreateTable();
            RPCResult<List<SMS>> r = PretreatmentProxy.GetStatusReportService().GetSMSRecordByAccount(Session["AccountID"].ToString(), starttime, endtime);
            if (r.Success)
            {
                lbl_message.Visible = false;
                Label1.Text = "当前统计状态报告短信条数是：" + r.Value.Count;
                List<SMS> smss = r.Value;
                if (smss.Count > 0)
                {
                    foreach (SMS s in smss)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Audit"] = GetAudit(s.Audit);
                        dr["SMSContent"] = s.Content;
                        dr["ContentFilter"] = GetContentFilter(s.Filter);
                        dr["Level"] = s.Level;
                        dr["SendTime"] = s.SendTime;
                        dr["SerialNumber"] = s.SerialNumber;
                        dr["StatusReport"] = GetStatusReport(s.StatusReport);
                        dr["BussType"] = GetBusstype(s.Channel);
                        
                        dt.Rows.Add(dr);
                    }
                }
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
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



        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("SMSID", Type.GetType("System.String"));
            table.Columns.Add("SerialNumber", Type.GetType("System.String"));
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

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string SerialNumber = e.CommandArgument.ToString();
            if (e.CommandName == "start")
            {
                string d = ((DropDownList)this.GridView1.Rows[0].Cells[0].FindControl("dd_l")).SelectedValue;
                RPCResult r = PretreatmentProxy.GetPretreatment().SetSMSLevel(Guid.Parse(SerialNumber), GetSMSLevelType(d));
                if (r.Success)
                {
                    load();
                    Message.Success(this, r.Message, "null");
                }
                else
                {
                    load();
                    Message.Error(this, r.Message, "null");
                }
            }

        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {


        }

        LevelType GetSMSLevelType(string a)
        {
            LevelType d = LevelType.Level2;
            switch (a)
            {
                case "0":
                    d = LevelType.Level0;
                    break;
                case "1":
                    d = LevelType.Level1;
                    break;
                case "2":
                    d = LevelType.Level2;
                    break;
                case "3":
                    d = LevelType.Level3;
                    break;
                case "4":
                    d = LevelType.Level4;
                    break;
                case "5":
                    d = LevelType.Level5;
                    break;
                case "6":
                    d = LevelType.Level6;
                    break;
                case "7":
                    d = LevelType.Level7;
                    break;
                case "8":
                    d = LevelType.Level8;
                    break;
                case "9":
                    d = LevelType.Level9;
                    break;
                case "10":
                    d = LevelType.Level10;
                    break;
                case "11":
                    d = LevelType.Level11;
                    break;
                case "12":
                    d = LevelType.level12;
                    break;
            }
            return d;
        }

        protected void btn_n_Click(object sender, EventArgs e)
        {
            load();
        }

    }
}