using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IService
{
    /// <summary>
    /// 输出日志的接口，由主界面实现
    /// </summary>
    public interface IForm
    {
        void WriteLog(string serviceCode, string message, string type,DateTime time);
        void ExecuteCmd(string cmd);
    }
}
