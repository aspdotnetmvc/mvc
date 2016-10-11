using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 联系方式接口
    /// </summary>
    public interface IContactInformation
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        String Mobile { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        String Email { get; set; }
    }
}
