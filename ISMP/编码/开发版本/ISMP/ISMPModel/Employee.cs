using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 员工表(企业员工信息）
    /// </summary>
    [Serializable]
    public class Employee : Account, IContactInformation
    {
        public Employee():base()
        {
            IsChannelManager = false;
            IsChannelServant = false;
            IsDelete = false;
            LinkType = LinkType.Employee;
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
        /// 部门ID
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public Department Department { get; set; }

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
        /// 是否销售经理
        /// </summary>
        public bool IsChannelManager { get; set; }
        /// <summary>
        /// 是否运营客服
        /// </summary>
        public bool IsChannelServant { get; set; }

    }
}