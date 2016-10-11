using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ISMPModel
{
    public interface IDBPermission
    {
        string Id { get; set; }
        int IsValid { get; set; }
        Permission Parent { get; set; }

        string GetPermissionData();
        void SetGetPermissionData(string data);
        PermissionType GetPermissionType();
    }

    public static class DBPermission
    {
        /// <summary>
        /// 生成权限独特属性数据
        /// </summary>
        public static string GetPermissionData<T>(this T t) where T : Permission, IDBPermission
        {
            var data = new { IsShow = t.IsShow, IsEnable = t.IsEnable };
            return JsonConvert.SerializeObject(data);
        }

        public static void SetGetPermissionData<T>(this T t,string data) where T : Permission, IDBPermission
        {
            if (data == "")
            {
                t.IsShow = true;
                t.IsEnable = true;
            }
            else
            {
                JObject jobj = JObject.Parse(data);
                if (jobj["IsShow"] == null || jobj["IsShow"].ToString() == "")
                    t.IsShow = true;
                else
                    t.IsShow = (bool)jobj["IsShow"];
                if (jobj["IsEnable"] == null || jobj["IsEnable"].ToString() == "")
                    t.IsEnable = true;
                else
                    t.IsEnable = (bool)jobj["IsEnable"];
            }
        }
    }
}