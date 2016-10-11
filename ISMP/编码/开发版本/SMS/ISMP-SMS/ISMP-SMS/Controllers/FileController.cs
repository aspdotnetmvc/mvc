using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISMPModel;

namespace ISMP_SMS.Controllers
{
    public class FileController : BaseController
    {
        /// <summary>
        /// 读取上传文件(文本文件)并返回文件内容
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public ActionResult GetUploadContent(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return Content("没有文件！", "text/plain");
            }

            try
            {
                StreamReader sr = new StreamReader(file.InputStream);
                string s = sr.ReadToEnd();
                RPC_Result r = new RPC_Result(true);
                r.Message = s;
                sr.Close();
                return Content(r.ToActionResult());
            }
            catch(Exception ex)
            {
                RPC_Result r = new RPC_Result(false);
                r.Message = "读取上传文件错误";
                return GetActionResult(r);
            }
        }

    }
}
