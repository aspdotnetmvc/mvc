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


namespace ZKD.Root.Report
{
    public partial class Rep_HistoryList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
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
            string txtnum = txt_Num.Text.Trim();
            if (DateTime.Compare(starttime, endtime) >= 0)
            {
                Message.Alert(this, "开始时间应小于结束时间", "null");
                return;
            }
            DataTable dt = CreateTable();
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            RPCResult<List<ReportStatistics>> r = ZHSMSProxy.GetZKD().GetStatisticsReportByAccount(user.AccountCode, starttime, endtime);
            if (r.Success)
            {
                lbl_message.Visible = false;

                var rep = r.Value;
                if (!string.IsNullOrWhiteSpace(txtnum))
                {
                    //筛选包含指定号码的记录
                    rep = rep.Where(s => containNum(s, txtnum)).OrderByDescending(s => s.BeginSendTime).ToList();
                }
                else
                {
                    rep = rep.OrderByDescending(s => s.BeginSendTime).ToList();
                }
                
                Label1.Text = "当前有" + rep.Count + "条返回状态报告";
                
                if (rep.Count > 0)
                {
                    foreach (ReportStatistics s in rep)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Account"] = s.Account;
                        dr["BeginSendTime"] = s.BeginSendTime;
                        dr["FailureCount"] = s.FailureCount;
                        dr["LastResponseTime"] = s.LastResponseTime;
                        dr["Numbers"] = s.Numbers;
                        dr["SendCount"] = s.SendCount;
                        dr["SendTime"] = s.SendTime;
                        dr["SerialNumber"] = s.SerialNumber;
                        dr["SplitNumber"] = s.SplitNumber;
                        dr["Succeed"] = s.Succeed;
                        dr["SMSContent"] = s.SMSContent;
                        List<string> num = s.Telephones;
                        string str = "";
                        foreach (string st in num)
                        {
                            str += st + ",";
                        }
                        dr["PhoneCount"] = str == "" ? "" : str.Substring(0, str.Length - 1);
                        dt.Rows.Add(dr);
                    }
                }
            }else
            {
                Message.Alert(this, r.Message, "null");
                return;

            }
           // dt.DefaultView.Sort = "SendTime desc";
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        /// <summary>
        /// 判断是否包含某个号码
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private bool containNum(ReportStatistics s, string num)
        {
            if (s.Telephones == null)
                return false;
            else
            {
                return s.Telephones.Count(t => t.IndexOf(num) > -1) > 0 ? true : false;
            }
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
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
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





        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {


        }


        protected void btn_n_Click(object sender, EventArgs e)
        {
            load();
        }

    }
}