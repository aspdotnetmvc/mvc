using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SMSModel;
using System.Data;

namespace WebSMS.Root.SMSM
{
    public partial class SMS_Level : JudgeSession
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }
        private void load()
        {
            DataTable dt = CreateTable();

            RPCResult<List<SMS>> r = PretreatmentProxy.GetPretreatment().GetSMSByAccount(Session["AccountID"].ToString());
            if (r.Success)
            {
                lbl_message.Visible = false;
                Label1.Text = "当前要优先级调整短信条数是：" + r.Value.Count;
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
                        dr["Signature"] = s.Signature;
                        List<string> num = s.Number;
                        if (num.Count > 3)
                        {
                            dr["PhoneCount"] = num[0] + "，" + num[1] + "，" + num[2] + " 等" + num.Count + "个号码";
                        }
                        else
                        {
                            foreach (string st in num)
                            {
                                dr["PhoneCount"] += st + ",";
                            }
                        }
                        string s1 = dr["PhoneCount"].ToString();
                        if (s1[s1.Length - 1] == ',')
                        {
                            s1 = s1.Substring(0, s1.Length - 1);
                        }
                        dr["PhoneCount"] = s1;
                        dt.Rows.Add(dr);
                    }
                }
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
            Session["dt"] = dt;

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
                    str = "商业短信";
                    break;
                case ChannelType.Industry:
                    str = "行业短信";
                    break;
            }
            return str;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (((DropDownList)e.Row.FindControl("dd_l") != null))
            {
                DropDownList ddl = (DropDownList)e.Row.FindControl("dd_l");
                ddl.Items.Clear();
                ddl.Items.Add(new ListItem("0", "0"));
                ddl.Items.Add(new ListItem("1", "1"));
                ddl.Items.Add(new ListItem("2", "2"));
                ddl.Items.Add(new ListItem("3", "3"));
                ddl.Items.Add(new ListItem("4", "4"));
                ddl.Items.Add(new ListItem("5", "5"));
                ddl.Items.Add(new ListItem("6", "6"));
                ddl.Items.Add(new ListItem("7", "7"));
                ddl.Items.Add(new ListItem("8", "8"));
                ddl.Items.Add(new ListItem("9", "9"));
                ddl.Items.Add(new ListItem("10", "10"));
                ddl.Items.Add(new ListItem("11", "11"));
                ddl.Items.Add(new ListItem("12", "12"));
                ddl.SelectedValue = ((HiddenField)e.Row.FindControl("HD")).Value;
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
            table.Columns.Add("Signature", Type.GetType("System.String"));
            return table;
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string SerialNumber = e.CommandArgument.ToString();
            if (e.CommandName == "start")
            {
                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    if (SerialNumber == GridView1.Rows[i].Cells[0].Text)
                    {
                        string d = ((DropDownList)this.GridView1.Rows[i].Cells[8].FindControl("dd_l")).SelectedValue;
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



    }
}