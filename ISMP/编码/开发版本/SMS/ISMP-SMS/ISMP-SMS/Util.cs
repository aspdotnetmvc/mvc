using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.POIFS;
using NPOI.Util;
using NPOI.SS.UserModel;
using BXM.Utils;
using ISMPModel;
using Newtonsoft.Json;
using ISMP_SMS.Model;
using BXM.Logger;
using SMSPlatform;
using SMS.Model;

namespace ISMP_SMS
{
    public class Util
    {
        #region　系统配置
        private static ISMSPlatformService st = (ISMSPlatformService)Activator.GetObject(typeof(ISMSPlatformService), ConfigurationManager.AppSettings["SMSPlatform"]);
        public static ISMSPlatformService SMSProxy
        {
            get
            {
                return st;
            }

        }

        public static string MobilePattern = ConfigurationManager.AppSettings["MobilePattern"];
        //短信费率 默认0.1
        public static decimal SMSRate = decimal.Parse(ConfigurationManager.AppSettings["SMSRate"]);
        public static string SMSHost = "http://" + ConfigurationManager.AppSettings["SMSHost"];
        public static string ISMPHost = "http://" + ConfigurationManager.AppSettings["ISMPHost"];

        public static string ProductSuffix = ConfigurationManager.AppSettings["ProductSuffix"];
        public static string SMSProductId = ConfigurationManager.AppSettings["SMSProductId"];
        public static string DefaultChannel = ConfigurationManager.AppSettings["defaultChannel"];
        public static string SMSProductName = ConfigurationManager.AppSettings["SMSProductName"];
        public static SMSType SMSType = (SMSType)int.Parse(ConfigurationManager.AppSettings["SMSType"]);
        public static string MenuSuffix = ConfigurationManager.AppSettings["MenuSuffix"];

        //短信模版审核
        public static string AuditType_TempletAudit = SMSProductName + "模版";
        //审核类型列表
        public static List<string> AuditTypeList = new List<string>(new string[] { AuditType_TempletAudit });
        /// <summary>
        /// 前台提交时每个包的最大大小
        /// </summary>
        public static int MaxNumbersPerPackage = int.Parse(ConfigurationManager.AppSettings["MaxNumbersPerPackage"]);
        #endregion

        /// <summary>
        /// 判断是否合法的手机号
        /// </summary>
        /// <param name="MobileNo"></param>
        /// <returns></returns>
        public static bool ValidMobile(string MobileNo)
        {
            return Regex.IsMatch(MobileNo, MobilePattern);
        }

        #region 生成随机密码
        /// <summary>
        /// 生成随机数的种子
        /// </summary>
        /// <returns></returns>
        private static int getNewSeed()
        {
            byte[] rndBytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(rndBytes);
            return BitConverter.ToInt32(rndBytes, 0);
        }     /// <summary>
        /// 生成8位随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GeneratePassword(int len)
        {
            string s = "123456789abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ!@#$%^&*()_+{}|";
            string reValue = string.Empty;
            Random rnd = new Random(getNewSeed());
            while (reValue.Length < len)
            {
                string s1 = s[rnd.Next(0, s.Length)].ToString();
                if (reValue.IndexOf(s1) == -1) reValue += s1;
            }
            return reValue;
        }
        #endregion


        #region 生成spNumber 
        /// <summary>
        /// 生成SpNumber
        /// </summary>
        /// <returns></returns>
        public static string GenSpNumber()
        {
            return Convert.ToString(NumberHelper.GetOrdinaryNumber(999999)).PadLeft(6, '0');
        }
        #endregion

        #region 操作Excel

        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel(string strFileName)
        {
            DataTable dt = new DataTable();
            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            HSSFRow headerRow = (HSSFRow)sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                HSSFCell cell = (HSSFCell)headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                HSSFRow row = (HSSFRow)sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel(Stream fileStream)
        {
            DataTable dt = new DataTable();
            HSSFWorkbook hssfworkbook;
            hssfworkbook = new HSSFWorkbook(fileStream);
            HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            HSSFRow headerRow = (HSSFRow)sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                HSSFCell cell = (HSSFCell)headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                HSSFRow row = (HSSFRow)sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        //public static bool IsNumeric(string str)
        //{

        //    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^0?(13[0-9]|15[012356789]|18[0-9]|14[57])[0-9]{8}$");
        //    if (reg1.IsMatch(str))
        //    {
        //        //数字
        //        return true;
        //    }
        //    else
        //    {
        //        //非数字
        //        return false;
        //    }
        //}

        public static MemoryStream DataTableToExcel(DataTable table)
        {
            MemoryStream ms = new MemoryStream();

            using (table)
            {
                IWorkbook workbook = new HSSFWorkbook();
                {
                    ISheet sheet = workbook.CreateSheet();
                    {
                        IRow headerRow = sheet.CreateRow(0);

                        // handling header.
                        foreach (DataColumn column in table.Columns)
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);//If Caption not set, returns the ColumnName value

                        // handling value.
                        int rowIndex = 1;

                        foreach (DataRow row in table.Rows)
                        {
                            IRow dataRow = sheet.CreateRow(rowIndex);

                            foreach (DataColumn column in table.Columns)
                            {
                                dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                            }

                            rowIndex++;
                        }

                        workbook.Write(ms);
                        ms.Flush();
                        ms.Position = 0;
                    }
                }
            }
            return ms;
        }

        #endregion

        #region 反序列化参数

        public static T DeserializeParameter<T>(string param)
        {
            try
            {
                return JsonSerialize.Instance.Deserialize<T>(param);
            }
            catch(Exception ex)
            {
                return default(T);
            }
        }

        #endregion

        #region 向ISMP发送日志

        /// <summary>
        /// 向ISMP发送日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool SendSystemLogToISMP(Sys_Log log)
        {
            try
            {
                string Param = JsonSerialize.Instance.Serialize<Sys_Log>(log);

                string url = Util.ISMPHost + "/CallBack/LogOperation?";
                url += "Param=" + System.Web.HttpUtility.UrlEncode(Param);

                string resultISMP = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(resultISMP, new { success = true, message = string.Empty });
                return o.success;
            }
            catch(Exception ex)
            {
                Log4Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 向ISMP发送日志
        /// </summary>
        /// <param name="module"></param>
        /// <param name="logMessage"></param>
        /// <param name="logDetail"></param>
        /// <param name="operationType"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static bool SendSystemLogToISMP(string module, string logMessage, string logDetail, string operationType, ISMPUser currentUser)
        {
            try
            {
                Sys_Log log = new Sys_Log
                {
                    AccountId = currentUser.OperatorAccountId,
                    LoginName = currentUser.OperatorLoginName,
                    Platform = Util.SMSProductId,
                    Module = module,
                    LogMessage = logMessage,
                    LogType = LogType.Operation,
                    LogTime = DateTime.Now,
                    Log = logDetail,
                    OperationType = operationType,
                    IPAddress = currentUser.IPAddress
                };
                string Param = JsonSerialize.Instance.Serialize<Sys_Log>(log);

                string url = Util.ISMPHost + "/CallBack/LogOperation?";
                url += "Param=" + System.Web.HttpUtility.UrlEncode(Param);

                string resultISMP = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(resultISMP, new { success = true, message = string.Empty });
                return o.success;
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 通过邮件发送警示信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool SendAlertMessageByEmail(string message, string title = "ISMP对接短信产品运行警示信息")
        {
            try
            {
                string url = Util.ISMPHost + "/CallBack/SendAlertMessageByEmail?";
                url += "Title=" + System.Web.HttpUtility.UrlEncode(title);
                url += "&Message=" + System.Web.HttpUtility.UrlEncode(message);

                string resultISMP = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(resultISMP, new { success = true, message = string.Empty });
                return o.success;
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 通过短信发送警示信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool SendAlertMessageBySMS(string message)
        {
            try
            {
                string url = Util.ISMPHost + "/CallBack/SendAlertMessageBySMS?";
                url += "Message=" + System.Web.HttpUtility.UrlEncode(message);

                string resultISMP = BXM.Utils.HTTPRequest.PostWebRequest(url, "", System.Text.Encoding.UTF8);
                var o = JsonConvert.DeserializeAnonymousType(resultISMP, new { success = true, message = string.Empty });
                return o.success;
            }
            catch (Exception ex)
            {
                Log4Logger.Error(ex);
                return false;
            }
        }

        #endregion
    }
}