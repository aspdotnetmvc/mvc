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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


namespace ZKD.Root.SMSM
{
    public partial class SMS_Send : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
            }

        }
        public void Checklistband()
        {
            DataTable dt1 = (DataTable)Session["dt"];
            if (dt1.Rows.Count > 0)
            {
                CheckBoxList1.DataSource = dt1;
                CheckBoxList1.DataBind();

            }
            for (int x = 0; x < this.CheckBoxList1.Items.Count; x++)
            {
                CheckBoxList1.Items[x].Selected = true;
            }

        }
 

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string[] dest;
            List<string> num = new List<string>();

            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            //DataTable dataTabe = (DataTable)Session["dt"];
            if (!Page.IsValid)
            {
                return;
            }

            dest = Regex.Split(txt_phone.Value, ",");
            if ((!string.IsNullOrWhiteSpace(txt_phone.Value))&&dest.Count() > 0)
            {
                //if (dest.Count() > 1000)
                //{
                //    Message.Alert(this, "单次提交最多不超过1000个号码");
                //    return;
                //}

                //简单判断是否包含非手机号码
                foreach (string destnum in dest)
                {
                    if (!NumberValidate.IsValidMobileNo(destnum))
                    {
                        Message.Alert(this, "请输入正确的手机号码", "null");
                        return;
                    }
                    else
                    {
                        num.Add(destnum);
                    }
                }

            }
            else
            {
                   DataTable dt1 = (DataTable)Session["dt"];
                   if (dt1.Rows.Count <= 1000)
                   {

                       foreach (ListItem li in CheckBoxList1.Items)
                       {
                           if (li.Selected == true)
                           {
                               num.Add(li.Text);
                           }
                       }
                       if (num.Count == 0)
                       {
                           Message.Alert(this, "请输入或选择电话号码", "null");
                           return;
                       }
                   }
                   else
                   {
                       //记录数多于1000 时，从dt1中读取
                        var q =
                            from c in dt1.AsEnumerable()
                            select c["phone"].ToString();

                        num = q.ToList();
                   }
              /*  if (num.Count > 1000)
                {
                    Message.Alert(this, "单次提交最多不超过1000个号码");
                    return;
                }*/
            }

            if (txt_Content.Value.ToString().Trim() == "")
            {
                Message.Alert(this, "请输入短信内容", "null");
                return;
            }

            Encoding gb = System.Text.Encoding.GetEncoding("gb2312");//转换编码
            byte[] bytes = gb.GetBytes(txt_Content.Value.ToString());//获取字节数组
            if (bytes.Length > 1000)
            {
                Message.Alert(this, "短信内容不能超过500中文字符", "null");
                return;
            }

            SMSModel.SMS sms = new SMS();
            bool tt = false;
            if (txt_SendTime.Value != "")
            {
                sms.SMSTimer = Convert.ToDateTime(txt_SendTime.Value);
                tt = true;
            }
            RPCResult<Guid> r = null;
            if (num.Count > 1000)
            {
                int fail = 0;
                int succ = 0;
                int step = 1000;
                for (int i = 0;i*step<num.Count;i++)
                {
                    int start = i * step;
                    int count = (i + 1) * step - 1 < num.Count - 1 ? step : num.Count - i * step - 1;
                    var subList = num.GetRange(start,count);

                    r = ZHSMSProxy.GetZKD().SendSMS(user.AccountCode, user.Password, txt_Content.Value.ToString().Trim(), txt_wapURL.Value.ToString(), subList, tt, sms.SMSTimer);
                    
                    if (r.Success)
                    {
                        succ++;
                        sms.SerialNumber = r.Value;
                        sms.Account = user.AccountCode;
                        sms.Content = txt_Content.Value;
                        sms.Number = subList;
                        sms.SendTime = DateTime.Now;
                        sms.WapURL = txt_wapURL.Value;

                        BLL.SMSdo.SMSAdd(sms);
                    }
                    else
                    {
                        fail++;
                        Message.Alert(this,r.Message);
                      //  Message.Error(this, r.Message, "null");
                    }
                    //Thread.Sleep(100);
                    
                }
                if (fail == 0)
                {
                    Message.Success(this, "短信已全部成功加入到发送队列", "null");
                }
                else
                {
                    Message.Success(this, "因记录数太多，拆分发送，其中成功次数："+succ+",失败次数："+fail, "null");
                }

            }
            else
            {
                r = ZHSMSProxy.GetZKD().SendSMS(user.AccountCode, user.Password, txt_Content.Value.ToString().Trim(), txt_wapURL.Value.ToString(), num, tt, sms.SMSTimer);

                if (r.Success)
                {
                    sms.SerialNumber = r.Value;
                    sms.Account = user.AccountCode;
                    sms.Content = txt_Content.Value;
                    sms.Number = num;
                    sms.SendTime = DateTime.Now;
                    sms.WapURL = txt_wapURL.Value;

                    BLL.SMSdo.SMSAdd(sms);
                    Message.Success(this, r.Message, "null");
                }
                else
                {
                    Message.Error(this, r.Message, "null");
                }
            }
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
                      // str = sr.ReadToEnd();
                    //strdata += str.Replace("\r\n", ",") ;
                    
                }
                File.Delete(path);
                load(strdata);

                DataTable dt1 = (DataTable)Session["dt"];
                if (dt1.Rows.Count <= 1000)
                {
                    Checklistband();
                }
                else
                {
                    //记录数大于1000时，不在界面上显示 
                    CheckBoxList1.Items.Clear();
                    lblMsg.Text = "已从文件中读取"+dt1.Rows.Count+"条手机号，因号码数太多，不在界面上显示。";
                    if (Session["errorNum"] != null)
                    {
                        string errNum = Session["errorNum"].ToString();
                        string errorCount = Session["errorCount"].ToString();
                        if (!string.IsNullOrWhiteSpace(errNum))
                        {
                            lblMsg.Text = "已从文件中读取" + dt1.Rows.Count + "条手机号，因号码数太多，不在界面上显示。"
                                + "<br/>另有不符合规范的号码数量：" + errorCount + "。<br/>"
                            + "<a href='/Root/SMSM/DownloadErrorNum.aspx' target='_blank'><b>点击下载</b></a>";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // throw ex;
            }
        }

        private void  load(string data)
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
                string errNum = "";
                Session["errorNum"] = "";
                int errorCount = 0;
                for (int i = 0; i < arr.Length; i++)
                {
                    DataRow row = t.NewRow();
                    
                    if (arr[i] != "")
                    {
                        if (NumberValidate.IsValidMobileNo(arr[i]))
                        {
                            row["phone"] = arr[i];

                            t.Rows.Add(row);
                        }
                        else
                        {
                            errNum += arr[i] + ",";
                            errorCount++;
                            if (errorCount % 10 == 0)
                            {
                                errNum += "\r\n";
                            }
                            
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(errNum))
                {
                    Session["errorNum"] = errNum;
                    Session["errorCount"] = errorCount;
                   // Message.Alert(this, "", "null");
                }
            }
            Session["dt"] = t;
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
    }
}