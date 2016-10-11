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
    public partial class Rep_StatisticsList : System.Web.UI.Page
    {
        private const string detail = "EnterpriseSms.Status.st";
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
                txt_Start.Text = DateTime.Now.AddDays(-1).ToString();
                txt_End.Text = DateTime.Now.ToString();

            }
        }

        private void search(int pageIndex)
        {
            DateTime starttime = Convert.ToDateTime(txt_Start.Text);
            DateTime endtime = Convert.ToDateTime(txt_End.Text);
            if (DateTime.Compare(starttime, endtime) >= 0)
            {
                Message.Alert(this, "开始时间应小于结束时间", "null");
                return;
            }
            string account = Request.QueryString["AccountID"];
            DataTable dt = CreateTable();
            RPCResult<List<ReportStatistics>> r = ZHSMSProxy.GetZHSMSPlatService().GetStatisticsReportAllByAccount(account, starttime, endtime);
            if (r.Success)
            {
                string txtNum = txt_num.Text.Trim();
                List<ReportStatistics> list = r.Value.OrderByDescending(s => s.BeginSendTime).ToList();
                if (!string.IsNullOrWhiteSpace(txtNum))
                {
                    list = r.Value.Where(s=>containNum(s,txtNum)).OrderByDescending(s => s.BeginSendTime).ToList();
                }
                pageBottom(list.Count, pageIndex);
                int startIndex = pageIndex * GridView1.PageSize;
                if (startIndex != 0)
                {
                    list = list.Skip(startIndex).ToList();
                }
                list = list.Take(GridView1.PageSize).ToList();
                foreach (var s in list)
                {
                    DataRow dr = dt.NewRow();
                    dr["Account"] = account;
                    dr["BeginSendTime"] = s.BeginSendTime;
                    dr["FailureCount"] = s.FailureCount;
                    dr["LastResponseTime"] = s.LastResponseTime;
                    if (s.Telephones != null)
                    {
                        dr["Numbers"] = s.Telephones.Count;
                        List<string> num = s.Telephones;
                        string str = "";
                        foreach (string st in num)
                        {
                            str += st + ",";
                        }
                        dr["PhoneCount"] = str == "" ? "" : str.Substring(0, str.Length - 1);
                    }
                    dr["SendCount"] = s.SendCount;
                    dr["SendTime"] = s.SendTime;
                    dr["SerialNumber"] = s.SerialNumber;
                    dr["SplitNumber"] = s.SplitNumber;
                    dr["Succeed"] = s.Succeed;
                    dr["SMSContent"] = s.SMSContent;

                    dt.Rows.Add(dr);
                }
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
          //  dt.DefaultView.Sort = "SendTime desc";
            this.ViewState["StatisticsReport"] = dt;
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        private bool containNum(ReportStatistics s,string num)
        {
            if (s.Telephones == null)
                return false;
            else
            {
                return s.Telephones.Count(t => t.IndexOf(num) > -1) > 0 ? true : false;
            }
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView1.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account.UserCode != "admin")
                {
                    List<string> permissions = (List<string>)this.ViewState["Permissions"];
                    if (!permissions.Contains(detail))
                    {
                        e.Row.Cells[11].Text = "无权限";
                    }
                }
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)this.ViewState["StatisticsReport"];
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("Account", Type.GetType("System.String"));
            table.Columns.Add("BeginSendTime", Type.GetType("System.String"));
            table.Columns.Add("FailureCount", Type.GetType("System.String"));
            table.Columns.Add("LastResponseTime", Type.GetType("System.String"));
            table.Columns.Add("Numbers", Type.GetType("System.String"));
            table.Columns.Add("SendCount", Type.GetType("System.String"));
            table.Columns.Add("SendTime", Type.GetType("System.String"));
            table.Columns.Add("SerialNumber", Type.GetType("System.String"));
            table.Columns.Add("SplitNumber", Type.GetType("System.String"));
            table.Columns.Add("Succeed", Type.GetType("System.String"));
            table.Columns.Add("SMSContent", Type.GetType("System.String"));
            table.Columns.Add("PhoneCount", Type.GetType("System.String"));

            return table;
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            search(0);
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
            this.CurrentPage = PageDropDownList.SelectedIndex;
            search(this.CurrentPage);
        }

        protected void linkBtnFirst_Click(object sender, EventArgs e)
        {
            //首页
            this.CurrentPage = 0;
            search(this.CurrentPage);
        }

        protected void linkBtnPrev_Click(object sender, EventArgs e)
        {
            //上一页
            this.CurrentPage--;
            search(this.CurrentPage);
        }

        protected void linkBtnNext_Click(object sender, EventArgs e)
        {
            //下一页
            this.CurrentPage++;
            search(this.CurrentPage);
        }

        protected void linkBtnLast_Click(object sender, EventArgs e)
        {
            //尾页
            this.CurrentPage = PageDropDownList.Items.Count - 1;
            search(this.CurrentPage);
        }

        private void pageBottom(int count, int pageIndex)
        {
            div_page.Visible = true;
            if (count == 0 && pageIndex == 0) div_page.Visible = false;
            try
            {
                linkBtnFirst.Enabled = true;
                linkBtnPrev.Enabled = true;
                linkBtnNext.Enabled = true;
                linkBtnLast.Enabled = true;
                int pageCount = count % GridView1.PageSize == 0 ? count / GridView1.PageSize : count / GridView1.PageSize + 1;
                lbRecordCount.Text = count.ToString();
                if (this.CurrentPage == 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                }
                if (this.CurrentPage >= pageCount - 1)
                {
                    linkBtnLast.Enabled = false;
                    linkBtnNext.Enabled = false;
                }
                if (pageCount <= 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                    linkBtnNext.Enabled = false;
                    linkBtnLast.Enabled = false;
                }
                PageDropDownList.Items.Clear();

                for (int i = 0; i < pageCount; i++)
                {
                    int pageNumber = i + 1;
                    ListItem item = new ListItem(pageNumber + "/" + pageCount, pageNumber.ToString());
                    if (i == this.CurrentPage)
                    {
                        item.Selected = true;
                    }
                    PageDropDownList.Items.Add(item);
                }

                int currentPage = this.CurrentPage + 1;
                CurrentPageLabel.Text = currentPage.ToString() +
                  " / " + pageCount;

            }
            catch
            {
            }
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {

        }

    }
}