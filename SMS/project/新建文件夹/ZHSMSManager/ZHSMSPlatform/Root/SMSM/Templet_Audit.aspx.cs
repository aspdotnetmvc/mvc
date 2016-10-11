using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.SMSM
{
    public partial class Templet_Audit : System.Web.UI.Page
    {
        private const string audit = "Templet.Audit.Audit";
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
                //pageBottom(0, 0);
                search(0);
            }
        }


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((bool)this.ViewState["audit"] == false)
                {
                    e.Row.Cells[5].Text = "无权限";
                    e.Row.Cells[6].Text = "无权限";
                }
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if ((bool)this.ViewState["del"] == false)
            {
                Message.Alert(this, "无权限", "null");
                return;
            }
            string group = this.GridView1.DataKeys[e.RowIndex][0].ToString();
            string keywords = this.GridView1.DataKeys[e.RowIndex][1].ToString();

            SMSModel.RPCResult r = ZHSMSProxy.GetZHSMSPlatService().DelKeywords(group, new List<string> { keywords });
            if (r.Success)
            {
                search(this.CurrentPage);
                Message.Success(this, "删除成功", "null");
            }
            else
            {
                Message.Error(this, r.Message, "null");
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string tempID = e.CommandArgument.ToString();//
            if (e.CommandName == "start")
            {
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                ZHSMSProxy.GetZHSMSPlatService().AuditSMSTemplet(tempID, account.UserCode, true, "");
                search(this.CurrentPage);
            }
            if (e.CommandName == "failure")
            {
                Response.Redirect("TempletAudit_Failure.aspx?templetID=" + tempID + "");
            }
        }

        protected void btn_nn_Click(object sender, EventArgs e)
        {
            search(0);
        }

        protected void btn_timer_Click(object sender, EventArgs e)
        {

            Timer1.Enabled = !Timer1.Enabled;
            if (btn_timer.Text.Equals("停止"))
            {
                btn_timer.Text = "开始";
            }
            else
            {
                btn_timer.Text = "停止";
            }
        }
        /// <summary>
        /// 定时刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            txt_E.Text = DateTime.Now.ToString();

            int interval = 3;
            try
            {
                interval = Convert.ToInt32(txt_timespan.Text);
            }
            catch (Exception ex)
            {
                interval = 10;
                txt_timespan.Text = interval.ToString();
            }

            if (interval < 3) interval = 3;
            Timer1.Interval = interval * 1000;
            search(CurrentPage);
        }

        private void search(int pageIndex)
        {
            DataTable dt = CreateTable();
            SMSModel.RPCResult<List<Model.SMSTemplet>> r = ZHSMSProxy.GetZHSMSPlatService().GetAuditSMSTemplet(DateTime.Parse(txt_S.Text), DateTime.Parse(txt_E.Text));
            if (r.Success)
            {
                List<Model.SMSTemplet> list = r.Value;
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
                    dr["TempletID"] = s.TempletID;
                    dr["AccountCode"] = s.AccountCode;
                    dr["AccountName"] = s.AccountName;
                    dr["TempletContent"] = s.TempletContent + s.Signature;
                    dr["SubmitTime"] = s.SubmitTime;
                    dr["Signature"] = s.Signature;
                    dt.Rows.Add(dr);
                }

            }
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
            table.Columns.Add("TempletID", Type.GetType("System.String"));
            table.Columns.Add("AccountCode", Type.GetType("System.String"));
            table.Columns.Add("AccountName", Type.GetType("System.String"));
            table.Columns.Add("TempletContent", Type.GetType("System.String"));
            table.Columns.Add("SubmitTime", Type.GetType("System.String"));
            table.Columns.Add("Signature", Type.GetType("System.String"));
            return table;
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
    }
}