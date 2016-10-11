using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BLL
{
    public class Login
    {

        public static string Logon(string account, string pass)
        {
            return DAL.Login.Logon(account, pass);
        }
        public static void IsLogin()
        {
            DAL.Login.IsLogin();
        }
        public static void AddGroupMR(string AccountCode)
        {
            DAL.Login.AddGroupMR(AccountCode);
        }
    }
}
