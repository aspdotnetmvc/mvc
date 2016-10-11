using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZKD.Root.SMSM
{
    public partial class DownloadErrorNum : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            BLL.Login.IsLogin();
         
            object errorNum = Session["errorNum"];
            if (errorNum == null)
            {
                Message.Alert(this,"没有找到错误号码！");
                
                return;
            }

            Response.Clear();
            Response.Charset = "UTF-8";
            Response.Buffer = true;
            this.EnableViewState = false;
            Response.ContentType = "application/octet-stream";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AppendHeader("Content-Disposition", "attachment; filename=errornum.txt");
            Response.Write(errorNum);
            Response.Flush();
            Response.Close();
            Response.End();
        }
    }
}