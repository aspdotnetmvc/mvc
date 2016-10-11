using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISMPModel;
using ISMPInterface;
using Newtonsoft.Json;
using ISMP_SMS.Model;
using BXM.Utils;

namespace ISMP_SMS.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 当前用户
        /// </summary>
        public ISMPUser CurrentUser
        {
            get
            {
                return (ISMPUser)Session["CurrentUser"];
            }
        }
        /// <summary>
        /// ISMP 调用时传入的参数
        /// </summary>
        public FunctionParameter FunctionParameter
        {
            get
            {
                var p = Session["Param"];
                if (p != null)
                {
                    return (FunctionParameter)p;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 公共验证
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Response.AddHeader("P3P", "CP=CAO PSA OUR");
            //如果传入了登录信息，则验证用户登录并设置session
            if (!string.IsNullOrWhiteSpace(Request["Parameter"]))
            {
                FunctionParameter fp = JsonSerialize.Instance.Deserialize<FunctionParameter>(Request["Parameter"]);
                validateUser(fp);
            }
            if (Session["CurrentUser"] == null)
            {
                RedirectToLogin(filterContext);
            }
            base.OnActionExecuting(filterContext);
        }


        public ActionResult GetActionResult(string val)
        {
            return Content(new RPC_Result<string>(true, val, "").ToActionResult());
        }

        /// <summary>
        /// RPC_Result转Json
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public ActionResult GetActionResult(RPC_Result result)
        {
            return Content(result.ToActionResult());
        }
        public ActionResult GetActionResult(SMS.Model.RPCResult result)
        {
            ISMPModel.RPC_Result r = new ISMPModel.RPC_Result(result.Success, result.Message);
            return GetActionResult(r);
        }
        public ActionResult GetActionResult(SMS.Model.RPC_Result result)
        {
            ISMPModel.RPC_Result r = new ISMPModel.RPC_Result(result.Success, result.Message);
            return GetActionResult(r);
        }
        /// <summary>
        /// RPC_Result转Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public ActionResult GetActionResult<T>(RPC_Result<T> result)
        {
            return Content(result.ToActionResult());
        }
        public ActionResult GetActionResult<T>(SMS.Model.RPCResult<T> result)
        {
            RPC_Result<T> r = new RPC_Result<T>(result.Success, result.Value, result.Message);
            return GetActionResult(r);
        }
        /// <summary>
        /// 转换为DataGrid数据格式
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ActionResult GetActionResult_Grid<T>(ResultSet<T> rs)
        {
            return Content(rs.ToString());
        }

        private void RedirectToLogin(ActionExecutingContext filterContext)
        {
            if (Request.IsAjaxRequest())  //判断是ajax请求
            {
                //Response.Write(@"{d:1});window.top.location='/Login';({d:1}");
                //Response.End();
                filterContext.Result = Content(@"{d:1});$.messager.alert('提示','用户长时间未操作，请刷新页面','info');({d:1}");
            }
            else
            {
                filterContext.Result = Content(@"用户长时间未操作，请刷新页面");
            }

        }

        /// <summary>
        /// 验证用户登录信息
        /// </summary>
        public void validateUser(FunctionParameter fp)
        {
            ISMPUser user = new ISMPUser();
            user.AccountId = fp.AccountId;
            user.LoginName = fp.LoginName;
            user.Name = fp.Name;
            user.OperatorAccountId = fp.OperatorAccountId;
            user.OperatorLoginName = fp.OperatorLoginName;
            user.OperatorName = fp.OperatorName;
            user.UserType = fp.UserType;

            if (user.UserType == UserType.Enterprise)
            {

                var r = Util.SMSProxy.GetEnterprise(fp.LoginName);

                user.SMSPassword = r.Value.Password;

            }

            Session["CurrentUser"] = user;
            Session["Param"] = fp;
        }


        public ResultSet<T> PageQuery<T>(IEnumerable<T> list, int page, int rows)
        {
            ResultSet<T> rs = new ResultSet<T>();
            rs.Total = list.ToList().Count;

            if (rs.Total > 0)
            {
                var query = from t in list select t;
                rs.Value = query.Take(rows * page).Skip(rows * (page - 1)).ToList();
            }
            else
            {
                rs.Value = list.ToList();
            }
            return rs;
        }


        public ActionResult Success(string message = "操作成功!")
        {
            return GetActionResult(new ISMPModel.RPC_Result(true, message));
        }

        public ActionResult Error(string message)
        {
            return GetActionResult(new ISMPModel.RPC_Result(false, message));
        }
    }
}
