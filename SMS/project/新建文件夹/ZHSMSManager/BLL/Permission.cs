using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class Permission
    {
        /// <summary>
        /// 获取用户的权限
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static List<SMS.Model.Permission> GetAccountPermissions(List<string> roles)
        {
            List<SMS.Model.Permission> accountPermission = new List<SMS.Model.Permission>();
            foreach (var v in roles)
            {
                List<SMS.Model.Permission> list = DAL.Permission.GetRolePermissions(v);
                foreach (var p in list)
                {
                    if (accountPermission.FirstOrDefault<SMS.Model.Permission>(c => c.MenuID == p.MenuID) != null)
                    {
                        SMS.Model.Permission model = accountPermission.FirstOrDefault<SMS.Model.Permission>(c => c.MenuID == p.MenuID);
                        model.Actions = model.Actions.Union(p.Actions).ToList<SMS.Model.PermissionAction>();
                    }
                    else
                    {
                        accountPermission.Add(p);
                    }
                }
            }
            return accountPermission;
        }
        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static List<SMS.Model.Permission> GetRolePermissions(string role)
        {
            return DAL.Permission.GetRolePermissions(role);
        }
        /// <summary>
        /// 根据菜单码获得菜单可操作的动作
        /// </summary>
        /// <param name="menuCode"></param>
        /// <returns></returns>
        public static List<SMS.Model.PermissionAction> GetPermissionByMenucode(string menuCode)
        {
            return DAL.Permission.GetPermissionByMenucode(menuCode);
        }
        /// <summary>
        /// 添加角色可操作的权限
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="permissionCodes"></param>
        /// <returns></returns>
        public static bool AddRolePermission(string roleID, List<string> permissionCodes)
        {
            if (DAL.Permission.DelRolePermisson(roleID))
            {
                if (DAL.Permission.AddRolePermisson(roleID, permissionCodes))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsUsePermission(string account, string permissionCode)
        {
            return DAL.Permission.IsUsePermission(account, permissionCode);
        }

        public static List<string> GetPermissionByAccount(string account)
        {
            return DAL.Permission.GetPermissionByAccount(account);
        }
    }
}
