using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    [Serializable]
    public enum EnterpriseState
    {
        /// <summary>
        /// 停用
        /// </summary>
        Disable = 1,
        /// <summary>
        /// 启用
        /// </summary>
        Enable = 2,
        /// <summary>
        /// 冻结
        /// </summary>
        Frozen = 12
    }

    /// <summary>
    /// 企业
    /// </summary>
    [Serializable]
    public class Enterprise : Account, IContactInformation
    {
        /// <summary>
        /// ID 
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 企业电话
        /// </summary>
        public String Telephone { get; set; }
        /// <summary>
        /// 企业地址
        /// </summary>
        public String Address { get; set; }
        /// <summary>
        /// 企业网址
        /// </summary>
        public String WebSite { get; set; }

        /// <summary>
        /// 企业规模
        /// </summary>
        public String Scale { get; set; }
        /// <summary>
        /// 企业性质
        /// </summary>
        public String CustomerProperty { get; set; }
        /// <summary>
        /// 营业执照号码
        /// </summary>
        public String LicenseNumber { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public String Contact { get; set; }
        /// <summary>
        /// 联系人身份证号码
        /// </summary>
        public String ContactID { get; set; }
        /// <summary>
        /// 联系人性别
        /// </summary>
        public String Sex { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public String ContactTelephone { get; set; }
        /// <summary>
        /// 是否法人
        /// </summary>
        public sbyte IsLegalPerson { get; set; }
        /// <summary>
        /// 主营项目
        /// </summary>
        public String MainProject { get; set; }
        /// <summary>
        /// 注册资金
        /// </summary>
        public String RegisterCapital { get; set; }
        /// <summary>
        /// 行业ID
        /// </summary>
        public String IndustryID { get; set; }

        /// <summary>
        /// 公司成立时间
        /// </summary>
        public DateTime EstablishedTime { get; set; }
        /// <summary>
        /// 经办人姓名
        /// </summary>
        public String Operator { get; set; }
        /// <summary>
        /// 提单来源
        /// </summary>
        public String Source { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public String ProvinceCode { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public String CityCode { get; set; }

        /// <summary>
        /// 企业证件列表
        /// </summary>
        public List<EnterpriseCredentials> Credentials { get; set; }


        /// <summary>
        /// 手机号码
        /// </summary>
        public String Mobile { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public String QQ { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        public String WeChat { get; set; }

        /// <summary>
        /// 记录创建时间
        /// </summary>
        public DateTime EnterCreateTime { get; set; }

        /// <summary>
        /// 是否删除:0否，1是
        /// </summary>
        public sbyte EnterIsDelete { get; set; }

        /// <summary>
        /// 企业状态
        /// </summary>
        public EnterpriseState EnterState { get; set; }
    }

    /// <summary>
    /// 企业证件信息
    /// </summary>
    [Serializable]
    public class EnterpriseCredentials
    {
        /// <summary>
        /// ID
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public String EnterpriseId { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public String CredentialType { get; set; }
        /// <summary>
        /// 证件路径
        /// </summary>
        public String CredentialFile { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public String Note { get; set; }
    }

    /// <summary>
    /// 行业
    /// </summary>
    /// 
    [Serializable]
    public class Industry
    {
        /// <summary>
        /// 行业ID
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 行业编码
        /// </summary>
        public String IndustryCode { get; set; }

        /// <summary>
        /// 行业名称
        /// </summary>
        public String IndustryName { get; set; }

        /// <summary>
        /// 父级行业ID
        /// </summary>
        public String ParentID { get; set; }

        /// <summary>
        /// 所有子行业
        /// </summary>
        public List<Industry> Child { get; set; }

        
    }

    /// <summary>
    /// 导入企业记录
    /// </summary>
    [Serializable]
    public class ImportEnterpriseInfo
    {
        /// <summary>
        /// 企业AccountId
        /// </summary>
        public String EnterpriseAccountId { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public String EnterpriseName { get; set; }
        /// <summary>
        /// 企业网址
        /// </summary>
        public String WebSite { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public String Contact { get; set; }
        /// <summary>
        /// 联系人手机
        /// </summary>
        public String Mobile { get; set; }
        /// <summary>
        /// 产品Ids
        /// </summary>
        public String ProductIds { get; set; }
        /// <summary>
        /// 对应产品的套餐Id
        /// </summary>
        public String Packages { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public String JavaScript { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 直属经销商AccountId
        /// </summary>
        public String AgentAccountId { get; set; }
        /// <summary>
        /// 直属经销商名称
        /// </summary>
        public String AgentName { get; set; }
        /// <summary>
        /// 操作人AccountId
        /// </summary>
        public String ApplyAccountId { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        public String ApplyName { get; set; }
        /// <summary>
        /// 导入批次Id
        /// </summary>
        public String BatchId { get; set; }
        /// <summary>
        /// 导入时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 排序Id
        /// </summary>
        public int SortId { get; set; }
        /// <summary>
        /// 是否全部成功
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}
