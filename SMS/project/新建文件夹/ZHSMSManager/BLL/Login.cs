using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class Login 
    {
        //static readonly IDAL.ILogin dal = DALFactory.DataAccess.CreateLogin();

        public static int Logon(string account, string pass)
        {
            return DAL.Login.Logon(account, DESEncrypt.Encrypt(pass));
        }
        public static void IsLogin()
        {
            DAL.Login.IsLogin();
        }
    }
}
