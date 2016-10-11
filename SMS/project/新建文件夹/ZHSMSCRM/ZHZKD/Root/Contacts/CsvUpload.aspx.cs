using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZKD.Root.Contacts
{
    public partial class CsvUpload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
                dd_l.Items.Clear();
                DataTable dt = BLL.PhoneAndGroup.GetGroupByAccountCode(user.AccountCode);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dd_l.Items.Add(new ListItem(dt.Rows[i]["TelPhoneGroupName"].ToString(), dt.Rows[i]["GID"].ToString()));
                    }
                }
                dd_l.Items.Remove("0");
                dd_l.Items.Insert(0, new ListItem("--请选择--", "-1"));
            }
        }
        private DataTable fileUpload()
        {
            if (this.FileUpload1.PostedFile.FileName != null)
            {
                string FileName = this.FileUpload1.FileName;
                string FileType = FileName.Substring(FileName.LastIndexOf('.') + 1).ToLower();
                string FilePath = Server.MapPath(@"~/Temp/");
                return GetFileData(FileUpload1, FileName, FilePath).Tables[0];
            }
            else
            {
                return null;
            }
        }

        public DataSet GetFileData(FileUpload fu, string FileName, string FilePath)
        {
            //Save the upload file to local  
            fu.PostedFile.SaveAs(FilePath + FileName);
            //ConnectionString of File  
            string strConnCSV = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties='text;HDR=No;FMT=Delimited;CharacterSet=65001;'";
            OleDbConnection oledb = new OleDbConnection(strConnCSV);
            string strSelect = "select * from " + FileName.Replace(FilePath, "");
            DataSet ds = new DataSet();
            try
            {
                oledb.Open();
                OleDbDataAdapter oledda = new OleDbDataAdapter(strSelect, oledb);
                oledda.Fill(ds);
                oledda.Dispose();
                File.Delete(FilePath + FileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oledb.Close();
            }
            return ds;
        }

        protected void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = fileUpload();
                bool r = ToSql(dt);//将XLS文件的数据导入数据库                

                if (r)
                {
                    LblMessage.Text = "数据导入成功！";
                }
                else
                {
                    LblMessage.Text = "数据导入失败！";
                }
            }
            catch (Exception ex)
            {
                LblMessage.Text = ex.Message;
            }
        }

        public bool ToSql(DataTable dt)
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            string GroupID = string.Empty;
            if (dd_l.SelectedValue == "-1")
            {
                DataTable dtt = BLL.PhoneAndGroup.GetGroupByTelPhoneGroupNameAndAccountCode("0", user.AccountCode);
                if (dtt.Rows.Count > 0)
                {
                    GroupID = dtt.Rows[0][0].ToString();
                }
            }
            else
            {
                GroupID = dd_l.SelectedValue;
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 1; i < dt.Rows.Count;i++ )
                {
                    if (dt.Rows[i][0].ToString() == "" || dt.Rows[i][0].ToString() == null)
                    {
                        Message.Alert(this, "联系人不为空", "null");
                        return false;
                    }
                    //bool r = IsNumeric(dt.Rows[i][4].ToString());
                    //if (!r)
                    //{
                    //    Message.Alert(this, "请输入正确的手机号码", "null");
                    //    return false;
                    //}
                    bool rr = BLL.PhoneAndGroup.PhoneUpload(
                         user.AccountCode, dt.Rows[i][0].ToString(),
                         dt.Rows[i][1].ToString(),
                         dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(),
                         dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString(),
                         dt.Rows[i][6].ToString(), dt.Rows[i][7].ToString(),
                         dt.Rows[i][8].ToString(), dt.Rows[i][9].ToString(), GroupID);
                    if (!rr)
                    {
                        Message.Alert(this, "上传号码失败", "null");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
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
                //数字
                return true;
            }
            else
            {
                //非数字
                return false;
            }
        }
    }
}