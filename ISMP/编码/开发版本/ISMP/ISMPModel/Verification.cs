using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    [Serializable]
    /// <summary>
    /// 用于后端验证
    /// </summary>
    public class Verification
    {
       
        public string AccountId
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string LoginName
        {
            get;
            set;
        }
        public string IPAddress
        {
            get;
            set;
        }
        /// <summary>
        /// 认证码
        /// </summary>
        public string VerifiCode
        {
            get;
            set;
        }
    }
}
