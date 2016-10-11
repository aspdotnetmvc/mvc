using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SMSModel;
using System.IO;
using System.Data;
using System.Threading;


namespace WebSMS.Root.SMSM
{
    public partial class SMS_SendTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dd_Audit.SelectedIndex = 0;
                dd_ContentFilter.SelectedIndex = 0;
                dd_Level.SelectedIndex = 2;
                dd_StatusReport.SelectedIndex = 1;
                dd_Busstype.SelectedIndex = 0;
                this.ViewState["data"] = this.CreateTable();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            long bt = DateTime.Now.Ticks;
            if (txt_c.Text.ToString().Trim() == "")
            {
                Message.Alert(this, "请输入数量级", "null");
                return;
            }
            if (txt_c.Text != "")
            {
                if (!IsNumeric2(txt_c.Text))
                {
                    Message.Alert(this, "请输入数字", "null");
                    return;
                }
            }
            int c = Convert.ToInt32(txt_c.Text.Trim());
            for (int i = 0; i < c; i++)
            {
                SMS acc = Send_par();
                PretreatmentProxy.GetPretreatment().SendSMS(acc);
                Thread.Sleep(100);
            }
            Message.Success(this, " 发送短信,耗时" + (decimal)((DateTime.Now.Ticks - bt) / 10000000) + "秒", "null");
        }

        protected SMS Send_par()
        {
            DataTable dataTabe = (DataTable)this.ViewState["data"];
            if (!Page.IsValid)
            {
                return null;
            }
            if (txt_phone.Text != "")
            {
                if (!IsNumeric(txt_phone.Text))
                {
                    Message.Alert(this, "请输入数字", "null");
                    return null;
                }
            }
            if (txt_Content.Text.ToString().Trim() == "")
            {
                Message.Alert(this, "请输入短信内容", "null");
                return null;
            }
            if (txt_Signature.Text.ToString().Trim() == "")
            {
                Message.Alert(this, "请填写签名", "null");
                return null;
            }
            SMS acc = new SMS();
            acc.Audit = (AuditType)ushort.Parse(dd_Audit.SelectedValue);
            acc.Content = txt_Content.Text.ToString().Trim();
            acc.Filter = (FilterType)ushort.Parse(dd_ContentFilter.SelectedValue);
            acc.Level = (LevelType)ushort.Parse(dd_Level.SelectedValue);
            acc.Account = Session["AccountID"].ToString();
            acc.Password = Session["Password"].ToString();
            List<string> num = new List<string>();

            if (dataTabe.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTabe.Rows)
                {
                    num.Add(dr["phone"].ToString());
                }
            }
            if (txt_phone.Text != "")
            {
                num.Add(txt_phone.Text.ToString());
            }
            acc.Number = num;
            acc.SendTime = DateTime.Now;
            acc.SerialNumber = Guid.NewGuid();
            acc.StatusReport = (StatusReportType)ushort.Parse(dd_StatusReport.SelectedValue);
            acc.Channel = (ChannelType)ushort.Parse(dd_Busstype.SelectedValue);
            acc.Signature = txt_Signature.Text.Trim().ToString();
            GridView1.DataSource = dataTabe;
            GridView1.DataBind();
            return acc;
        }


        protected void btn_import_Click(object sender, EventArgs e)
        {
            try
            {
                string path = this.FileUpload1.PostedFile.FileName;
                if (path == "" || path == null)
                {
                    Message.Alert(this, "请选择一个文件，文件格式为txt", "null");
                    return;
                }

                string fileExt = System.IO.Path.GetExtension(FileUpload1.FileName).ToLower();
                if (fileExt != ".txt")
                {
                    Message.Alert(this, "文件格式为txt", "null");
                    return;
                }

                path = Server.MapPath("~/Temp/") + FileUpload1.FileName;
                FileUpload1.PostedFile.SaveAs(path);
                string strdata = "";
                using (StreamReader sr = new StreamReader(path))
                {
                    string str;
                    while ((str = sr.ReadLine()) != null)
                    {
                        strdata += str + ",";
                    }
                }
                File.Delete(path);
                load(strdata);
            }
            catch
            {
            }
        }

        private void load(string data)
        {
            DataTable t = CreateTable();
            if (data == "")
            {
                Message.Alert(this, "文件是空的", "null");
                return;
            }
            string[] arr = data.Split(',');
            if (arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    DataRow row = t.NewRow();
                    if (arr[i] != "")
                    {
                        if (IsNumeric(arr[i]))
                        {
                            row["phone"] = arr[i];

                            t.Rows.Add(row);
                        }
                        else
                        {
                            Message.Alert(this, "请输入正确的手机号码", "null");
                            return;
                        }
                    }
                }
            }
            this.ViewState["data"] = t;
            GridView1.DataSource = t;
            GridView1.DataBind();
        }

        private DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("phone", Type.GetType("System.String"));
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
            GridViewRow pagerRow = GridView1.BottomPagerRow;
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            GridView1.PageIndex = pageList.SelectedIndex;
            this.CurrentPage = pageList.SelectedIndex;
            DataTable dt = (DataTable)this.ViewState["data"];
            GridView1.DataSource = dt;
            GridView1.DataBind();
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
            DataTable dt = (DataTable)this.ViewState["data"];
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        private bool IsNumeric2(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[-]?\d+[.]?\d*$");

            if (reg1.IsMatch(str))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^0?(13[0-9]|15[012356789]|18[0-9]|14[57])[0-9]{8}$");


            if (reg1.IsMatch(str))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}