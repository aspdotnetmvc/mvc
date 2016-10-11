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
    public partial class BulkUploadContacts : System.Web.UI.Page
    {
        protected System.Web.UI.HtmlControls.HtmlInputFile FileExcel;
        protected System.Web.UI.WebControls.Button BtnImport;
        protected System.Web.UI.WebControls.Label LblMessage;
        private string MobilePattern = System.Configuration.ConfigurationManager.AppSettings["MobilePattern"];
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            base.OnInit(e);
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
        /// <summary>
        /// 上传Excel文件
        /// </summary>
        /// <param name="inputfile">上传的控件名</param>
        /// <returns></returns>
        private string UpLoadXls(System.Web.UI.HtmlControls.HtmlInputFile inputfile)
        {
            string orifilename = string.Empty;
            string uploadfilepath = string.Empty;
            string modifyfilename = string.Empty;
            string fileExtend = "";//文件扩展名
            int fileSize = 0;//文件大小
            try
            {
                if (inputfile.Value != string.Empty)
                {
                    //得到文件的大小
                    fileSize = inputfile.PostedFile.ContentLength;
                    if (fileSize == 0)
                    {
                        throw new Exception("导入的Excel文件大小为0，请检查是否正确！");
                    }
                    //得到扩展名
                    fileExtend = inputfile.Value.Substring(inputfile.Value.LastIndexOf(".") + 1);
                    if (fileExtend.ToLower() != "xls")
                    {
                        throw new Exception("你选择的文件格式不正确，只能导入EXCEL文件！");
                    }
                    //路径
                    uploadfilepath = Server.MapPath("~/Temp/");
                    //新文件名
                    modifyfilename = System.Guid.NewGuid().ToString();
                    modifyfilename += "." + inputfile.Value.Substring(inputfile.Value.LastIndexOf(".") + 1);
                    //判断是否有该目录
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(uploadfilepath);
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }
                    orifilename = uploadfilepath + "\\" + modifyfilename;
                    //如果存在,删除文件
                    if (File.Exists(orifilename))
                    {
                        File.Delete(orifilename);
                    }
                    // 上传文件
                    inputfile.PostedFile.SaveAs(orifilename);
                }
                else
                {
                    throw new Exception("请选择要导入的Excel文件!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return orifilename;
        }
        //// <summary>
        /// 从Excel提取数据--》Dataset
        /// </summary>
        /// <param name="filename">Excel文件路径名</param>
        private bool ImportXlsToData(string fileName)
        {
            try
            {
                if (fileName == string.Empty)
                {
                    throw new ArgumentNullException("Excel文件上传失败！");
                }

                string oleDBConnString = String.Empty;
                oleDBConnString = "Provider=Microsoft.Jet.OLEDB.4.0;";
                oleDBConnString += "Data Source=";
                oleDBConnString += fileName;
                oleDBConnString += ";Extended Properties=Excel 8.0;";
                OleDbConnection oleDBConn = null;
                OleDbDataAdapter oleAdMaster = null;
                DataTable m_tableName = new DataTable();
                DataSet ds = new DataSet();

                oleDBConn = new OleDbConnection(oleDBConnString);
                oleDBConn.Open();
                m_tableName = oleDBConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (m_tableName != null && m_tableName.Rows.Count > 0)
                {
                    m_tableName.TableName = m_tableName.Rows[0]["TABLE_NAME"].ToString();
                }
                string sqlMaster;
                sqlMaster = " SELECT *  FROM [" + m_tableName.TableName + "]";
                oleAdMaster = new OleDbDataAdapter(sqlMaster, oleDBConn);
                oleAdMaster.Fill(ds, "m_tableName");
                oleAdMaster.Dispose();
                oleDBConn.Close();
                oleDBConn.Dispose();
                return AddDatasetToSQL(ds, 9);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将Dataset的数据导入数据库
        /// </summary>
        /// <param name="pds">数据集</param>
        /// <param name="Cols">数据集列数</param>
        /// <returns></returns>
        private bool AddDatasetToSQL(DataSet pds, int Cols)
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];

            string GroupID = string.Empty;
            if (dd_l.SelectedValue == "-1")
            {
                DataTable dt = BLL.PhoneAndGroup.GetGroupByTelPhoneGroupNameAndAccountCode("0", user.AccountCode);
                if (dt.Rows.Count > 0)
                {
                    GroupID = dt.Rows[0][0].ToString();
                }
            }
            else
            {
                GroupID = dd_l.SelectedValue;
            }
            if (pds != null && pds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < pds.Tables[0].Rows.Count; i++)
                {
                    if (pds.Tables[0].Rows[i][0].ToString() == "" || pds.Tables[0].Rows[i][0].ToString() == null)
                    {
                        Message.Alert(this, "联系人不为空 ,行号：" + (i + 2), "null");
                        return false;
                    }
                    bool r = IsNumeric(pds.Tables[0].Rows[i][4].ToString());
                    if (!r)
                    {
                        Message.Alert(this, "请输入正确的手机号码,行号：" + (i + 2), "null");
                        return false;
                    }


                }
            }
            else
            {
                throw new Exception("导入数据为空！");
            }
            bool rr = BLL.PhoneAndGroup.PhoneUpload(
                        user.AccountCode, GroupID, pds.Tables[0]);
            if (!rr)
            {
                Message.Alert(this, "上传号码失败", "null");
                return false;
            }
            return true;

        }
        private bool IsNumeric(string str)
        {

            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(MobilePattern);
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

        protected void BtnImport_Click(object sender, EventArgs e)
        {
            string filename = string.Empty;
            try
            {
                filename = UpLoadXls(FileExcel);//上传XLS文件
                bool r = ImportXlsToData(filename);//将XLS文件的数据导入数据库                
                if (filename != string.Empty && System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);//删除上传的XLS文件
                }
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

    }
}