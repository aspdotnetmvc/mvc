using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL
{
    public class PhoneAndGroup
    {
        /// <summary>
        /// 通讯录组
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static bool GroupAdd(string AccountCode, string TelPhoneGroupName, string ReMark)
        {
            return DAL.PhoneAndGroup.GroupAdd(AccountCode, TelPhoneGroupName, ReMark);
        }
        /// <summary>
        /// 获取通讯录
        /// </summary>
        /// <param name="AccountCode"></param>
        /// <returns></returns>
        public static DataTable GetGroupByAccountCode(string AccountCode)
        {
            return DAL.PhoneAndGroup.GetGroupByAccountCode(AccountCode);
        }


        public static DataTable GetGroupForTreeByAccountCode(string AccountCode)
        {
            return DAL.PhoneAndGroup.GetGroupForTreeByAccountCode(AccountCode);
        }
        /// <summary>
        /// 删除通讯录
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static bool GroupDelByID(string ID)
        {
            return DAL.PhoneAndGroup.GroupDelByID(ID);
        }
        public static DataTable GetGroupByTelPhoneGroupNameAndAccountCode(string TelPhoneGroupName, string AccountCode)
        {
            return DAL.PhoneAndGroup.GetGroupByTelPhoneGroupNameAndAccountCode(TelPhoneGroupName, AccountCode);
        }

        /// <summary>
        /// 通讯录批量上传
        /// </summary>
        /// <param name="AccountCode"></param>
        /// <param name="UserName"></param>
        /// <param name="UserBrithday"></param>
        /// <param name="UserSex"></param>
        /// <param name="CompanyName"></param>
        /// <param name="TelPhoneNum"></param>
        /// <param name="CompanyEmail"></param>
        /// <param name="QQ"></param>
        /// <param name="ComPostion"></param>
        /// <param name="WebChat"></param>
        /// <param name="CompanyWeb"></param>
        /// <returns></returns>
        public static bool PhoneUpload(string AccountCode, string UserName, string UserBrithday, string UserSex, string CompanyName, string TelPhoneNum, string CompanyEmail, string QQ, string ComPostion, string WebChat, string CompanyWeb, string GroupID)
        {
            return DAL.PhoneAndGroup.PhoneUpload(AccountCode, UserName, UserBrithday, UserSex, CompanyName, TelPhoneNum, CompanyEmail, QQ, ComPostion, WebChat, CompanyWeb, GroupID);
        }
        public static bool PhoneUpload(string AccountCode, string GroudID, DataTable dt)
        {
            return DAL.PhoneAndGroup.PhoneUpload(AccountCode, GroudID, dt);
        }
        /// <summary>
        /// 返回所有通讯录
        /// </summary>
        /// <param name="AccountCode"></param>
        /// <returns></returns>
        public static DataTable GetPhoneByAccountCode(string AccountCode)
        {
            return DAL.PhoneAndGroup.GetPhoneByAccountCode(AccountCode);
        }
        /// <summary>
        /// 组电话号码
        /// </summary>
        /// <param name="AccountCode"></param>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public static DataTable GetPhoneByAccountCodeAndGroup(string AccountCode, string GroupID)
        {
            return DAL.PhoneAndGroup.GetPhoneByAccountCodeAndGroup(AccountCode, GroupID);
        }
        /// <summary>
        /// 获取电话号码
        /// </summary>
        /// <param name="AccountCode"></param>
        /// <returns></returns>
        public static DataTable GetPhoneByAccountCodes(string AccountCode)
        {
            return DAL.PhoneAndGroup.GetPhoneByAccountCodes(AccountCode);
        }
        /// <summary>
        /// 删除单个号码
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static bool PhoneDelByID(string ID)
        {
            return DAL.PhoneAndGroup.PhoneDelByID(ID);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static bool PhoneConnectGroup(string GroupID, string AccountCode, string PID)
        {
            return DAL.PhoneAndGroup.PhoneConnectGroup(GroupID, AccountCode, PID);
        }

        ////=--------电话号码
        ///// <summary>
        ///// 电话
        ///// </summary>
        ///// <param name="AccountCode"></param>
        ///// <returns></returns>
        //public static DataTable GetPhonesByAccountCode(string AccountCode)
        //{
        //    return DAL.PhoneAndGroup.GetPhonesByAccountCode(AccountCode);
        //}

        //public static DataTable GetPhonesByAccountCodeAndUserName(string AccountCode, string UserName)
        //{
        //    return DAL.PhoneAndGroup.GetPhonesByAccountCodeAndUserName(AccountCode, UserName);
        //}
        //public static DataTable GetPhonesByAccountCodeAndTime(string AccountCode, DateTime a, DateTime b)
        //{
        //    return DAL.PhoneAndGroup.GetPhonesByAccountCodeAndTime(AccountCode, a, b);
        //}
        //public static DataTable GetPhonesByAccountCodeAndGroup(string AccountCode, string GroupID)
        //{
        //    return DAL.PhoneAndGroup.GetPhonesByAccountCodeAndGroup(AccountCode, GroupID);
        //}
        //public static DataTable GetPhonesByAccountCodeAndGroupAndUserName(string AccountCode, string GroupID, string PID)
        //{
        //    return DAL.PhoneAndGroup.GetPhonesByAccountCodeAndGroupAndUserName(AccountCode, GroupID, PID);
        //}
        //public static DataTable GetPhonesByAccountCodeAndGroupAndTime(string AccountCode, string GroupID, DateTime a, DateTime b)
        //{
        //    return DAL.PhoneAndGroup.GetPhonesByAccountCodeAndGroupAndTime(AccountCode, GroupID, a, b);
        //}
    }
}
