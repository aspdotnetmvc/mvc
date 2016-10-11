using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceManageWin
{
    /// <summary>
    /// 当WriteLog 传入的信息是type  = cmd 时，通过本类来执行
    /// </summary>
    public class CMDExecuter
    {
        public MainForm MainForm { get; set; }

        public bool Execute(string serviceCode,string cmd)
        {
            return true;
        }
    }
}
