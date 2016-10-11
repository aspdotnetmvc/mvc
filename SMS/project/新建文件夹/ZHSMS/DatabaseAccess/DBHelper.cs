using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseAccess
{
    public class DBHelper : DBTools.DBHelper
    {
        public override string ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["mysql"].ConnectionString;
            }
        }

        public override DBTools.DataBaseType DBType
        {
            get { return DBTools.DataBaseType.MySql; }
        }
        private static DBHelper _Instance = new DBHelper();
        public static DBHelper Instance
        {
            get
            {
                return _Instance;
            }
        }
    }
}
