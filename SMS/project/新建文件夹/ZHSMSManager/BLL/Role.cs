using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class Role
    {
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public static bool Exists(string role)
        {
            return DAL.Role.Exists(role);
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(SMS.Model.Role role)
        {
            return DAL.Role.Add(role);
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(SMS.Model.Role role)
        {
            return DAL.Role.Update(role);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Del(string role)
        {
            if (DAL.Role.Del(role))
            {
                if (DAL.Permission.DelRolePermisson(role))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static SMS.Model.Role GetRole(string role)
        {
            return DAL.Role.GetRole(role);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public static List<SMS.Model.Role> GetRoles()
        {
            return DAL.Role.GetRoles();
        }
    }
}
