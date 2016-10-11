using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBTools
{
    public class DBFactory
    {
        public static DBAdapter GetAdapter(DataBaseType dbtype, string connectionString)
        {
            DBAdapter adapter;
            switch (dbtype)
            {
                case DataBaseType.SqlServer:
                    adapter = new MSSQLAdapter();
                    break;
                case DataBaseType.MySql:
                    adapter = new MySqlAdapter();
                    break;
                default:
                    throw new Exception("该数据库的DBAdapter 未实现。" + dbtype.ToString());
            }
            adapter.ConnectionString = connectionString;
            return adapter;
        }

       
    }
}
