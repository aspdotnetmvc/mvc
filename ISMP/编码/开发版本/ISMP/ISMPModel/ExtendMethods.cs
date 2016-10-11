using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ISMPModel
{
    public static class Tools
    {
        /// <summary>
        /// 子类复制父类的值
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parent"></param>
        /// <param name="nullreset">父类属性值为空时是否覆盖子类</param>
        /// <param name="valuereset">子类属性有值时是否被覆盖</param>
        public static void CopyParent(this object target, object parent, bool nullreset = true, bool valuereset = true)
        {
            var ParentType = parent.GetType();
            var Properties = ParentType.GetProperties();
            var tProperties = target.GetType().GetProperties();
            foreach (var Propertie in Properties)
            {
                if (Propertie.CanRead && Propertie.CanWrite)
                {
                    var tp = tProperties.FirstOrDefault(p => p.Name == Propertie.Name);
                    if (tp == null) { continue; }
                    var pvalue = Propertie.GetValue(parent, null);
                    var tvalue = tp.GetValue(target, null);
                    if (!valuereset && tvalue != null) { continue; }
                    if (!nullreset && pvalue == null) { continue; }
                    tp.SetValue(target, Propertie.GetValue(parent, null), null);
                }
            }

        }
        /// <summary>
        /// 判断是否为可空类型
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        public static bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.
              GetGenericTypeDefinition().Equals
              (typeof(Nullable<>)));
        }

        /// <summary>
        /// 转为前台json数据
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static string ToActionResult(this RPC_Result r)
        {
            if (r.Success)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, message = string.IsNullOrWhiteSpace(r.Message) ? "操作成功！" : r.Message, errorcode = r.ErrorCode, type = "success" });
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, message = string.IsNullOrWhiteSpace(r.Message) ? "操作失败！" : r.Message, errorcode = r.ErrorCode, type = "error" });
            }
        }
        /// <summary>
        /// 转为前台json数据
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static string ToActionResult<T>(this RPC_Result<T> r)
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            if (r.Success)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, message = string.IsNullOrWhiteSpace(r.Message) ? "操作成功！" : r.Message, value = r.Value, errorcode = r.ErrorCode, type = "success" }
                   , Newtonsoft.Json.Formatting.Indented, timeFormat);
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, message = string.IsNullOrWhiteSpace(r.Message) ? "操作失败！" : r.Message, value = r.Value, errorcode = r.ErrorCode, type = "error" }
                    , Newtonsoft.Json.Formatting.Indented, timeFormat);
            }
        }

        public static string ToActionResultModel<T>(this RPC_Result<T> r)
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            if (r.Success)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(r.Value, Newtonsoft.Json.Formatting.Indented, timeFormat);
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(null, Newtonsoft.Json.Formatting.Indented, timeFormat);
            }
        }

        /// <summary>
        /// 获取字符串被字符分割部分
        /// </summary>
        /// <param name="target"></param>
        /// <param name="searchValue">分割字符</param>
        /// <param name="index">要获取的被字符分割部分的索引</param>
        /// <param name="failOriginal">获取失败返回原字符串还是null</param>
        public static string GetSubString(this string target, char searchValue = ',', uint index = 0, bool failOriginal = true)
        {
            if(target == null)
            {
                return target;
            }

            if(target.Contains(searchValue))
            {
                var array = target.Split(searchValue);
                if (array != null && array.Length > index)
                {
                    return target.Split(searchValue)[index];
                }
            }

            return failOriginal ? target : null;
        }
    }
}
