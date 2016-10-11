using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 员工表(企业员工信息）
    /// </summary>
    [Serializable]
    public class AgentEmployee : Account, IContactInformation
    {
        public AgentEmployee()
            : base()
        {
            IsDelete = false;
            LinkType = LinkType.AgentEmployee;
            RegisterType = AccountRegisterType.UserName;
            
        }
        private string _Id="";
        /// <summary>
        /// 
        /// </summary>
        public String Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                LinkId = value;
            }
        }
        /// <summary>
        /// 所属Agent 的Account Id
        /// </summary>
        public string AgentAccountId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 性别（男，女）
        /// </summary>
        public String Sex { get; set; }

        /// <summary>
        /// 电话（固话）
        /// </summary>
        public String Telephone { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public String QQ { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        public String WeChat { get; set; }


        public String Mobile { get; set; }

        public String Email { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public String Address { get; set; }

      
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public new DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public new bool IsDelete { get; set; }


        /// <summary>
        /// 是否 启用分管（不启用则管理全部，启用则管理AgentEmployeeEnterprise 表指定的企业)
        /// </summary>
        public bool EnableEnterprisePermission
        {
            get;
            set;
        }

    }
}