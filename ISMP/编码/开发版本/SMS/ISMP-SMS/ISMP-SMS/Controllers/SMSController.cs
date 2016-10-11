using System.Web.Mvc;
using System.Configuration;
using ISMPModel;
using ISMPInterface;
using System.Collections.Generic;
using Newtonsoft.Json;
using BXM.Utils;

namespace ISMP_SMS.Controllers
{
    public class SMSController : Controller
    {
        /// <summary>
        /// 短信ISMP 需要的配置接口
        /// </summary>
        /// <returns></returns>
        public ActionResult ISMPConfigList()
        {
            string host = Util.SMSHost;

            IProductConfiguration config = new ProductConfiguration();

            config.ProductId = Util.SMSProductId;

            config.PaymentsType = new List<string>() { "短信充值" };

            #region Functions
            config.Functions = new List<IProductFunction>();

            config.Functions.Add(new ProductFunction()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSActive" + Util.ProductSuffix,
                Name = "短信激活" + Util.MenuSuffix,
                FunctionType = FunctionType.Activate,
                Url = "----"
            });

            config.Functions.Add(new ProductFunction()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSSale" + Util.ProductSuffix,
                Name = "短信销售" + Util.MenuSuffix,
                FunctionType = FunctionType.Sale,
                Url = host + @"/Sale"
            });

            config.Functions.Add(new ProductFunction()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSRecharge" + Util.ProductSuffix,
                Name = "短信充值" + Util.MenuSuffix,
                FunctionType = FunctionType.Recharge,
                Url = host + @"/Sale/Recharge"
            });

            config.Functions.Add(new ProductFunction()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSCommand" + Util.ProductSuffix,
                Name = "命令" + Util.MenuSuffix,
                FunctionType = FunctionType.Command,
                Url = host + "/SMS/Command"
            });


            config.Functions.Add(new ProductAudit()
            {
                AuditsType = Util.AuditTypeList,
                Id = "SMSAudit" + Util.ProductSuffix,
                Name = "短信产品审核" + Util.MenuSuffix,
                FunctionType = FunctionType.Audit,
                Url = host + "/Platform/Audit"
            });

            config.Functions.Add(new ProductFunction()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSAuditEdit" + Util.ProductSuffix,
                Name = "短信产品审核失败编辑重提" + Util.MenuSuffix,
                FunctionType = FunctionType.AuditEdit,
                Url = host + "/Platform/AuditEdit"
            });

            config.Functions.Add(new ProductFunction()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSSaleCheckSubmit" + Util.ProductSuffix,
                Name = "套餐验证" + Util.MenuSuffix,
                FunctionType = FunctionType.SaleCheckSubmit,
                Url = host + "/SMS/SaleCheckSubmit"
            });

            config.Functions.Add(new ProductFunction()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSEnterpriseSMSStatus" + Util.ProductSuffix,
                Name = "短信详单" + Util.MenuSuffix,
                FunctionType = FunctionType.Status,
                Url = host + "/Enterprise/EnterpriseSMSStatus"
            });

            config.Functions.Add(new ProductFunction()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSEnterpriseSMSHomePageStatistics" + Util.ProductSuffix,
                Name = "短信首页统计" + Util.MenuSuffix,
                FunctionType = FunctionType.HomePageStatistics,
                Url = host + "/Enterprise/EnterpriseSMSHomePageStatistics"
            });

            config.Functions.Add(new ProductFunction()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSEnterpriseSMSAutoRechargeAsk" + Util.ProductSuffix,
                Name = "短信企业自充值可选金额或套餐查询" + Util.MenuSuffix,
                FunctionType = FunctionType.AutoRechargeAsk,
                Url = host + "/Enterprise/EnterpriseSMSAutoRechargeAsk"
            });

            config.Functions.Add(new ProductFunction()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSEnterpriseSMSAutoRecharge" + Util.ProductSuffix,
                Name = "短信企业自充值" + Util.MenuSuffix,
                FunctionType = FunctionType.AutoRecharge,
                Url = host + "/Enterprise/EnterpriseSMSAutoRecharge"
            });

            #endregion

            config.Modules = new List<IProductModule>();



            #region 中呼端功能

            IProductModule GatewayConfig = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSGateway" + Util.ProductSuffix,
                Name = "网关配置" + Util.MenuSuffix,
                Url = host + "/Platform/GatewayConfig",
                OrderNumber = 100,
                Permissions = new List<IPermission>()

            };
            config.Modules.Add(GatewayConfig);

            IProductModule Channel = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSChannel" + Util.ProductSuffix,
                Name = "通道配置" + Util.MenuSuffix,
                Url = host + "/Platform/ChannelManage",
                OrderNumber = 200,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(Channel);

            IProductModule SMSSystemBlackList = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSSystemBlackList" + Util.ProductSuffix,
                Name = "系统黑名单" + Util.MenuSuffix,
                Url = host + "/Platform/SMSSystemBlackList",
                OrderNumber = 300,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSSystemBlackList);

            IProductModule SMSKeywordsGroup = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSKeywordsGroup" + Util.ProductSuffix,
                Name = "敏感词组" + Util.MenuSuffix,
                Url = host + "/Platform/KeywordsGroup",
                OrderNumber = 400,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSKeywordsGroup);

            IProductModule SMSKeywords = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSKeywords" + Util.ProductSuffix,
                Name = "敏感词" + Util.MenuSuffix,
                Url = host + "/Platform/Keywords",
                OrderNumber = 500,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSKeywords);

            IProductModule SMSSendSMSAudit = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSSendSMSAudit" + Util.ProductSuffix,
                Name = "短信审核" + Util.MenuSuffix,
                Url = host + "/Platform/SendSMSAudit",
                OrderNumber = 600,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSSendSMSAudit);


            IProductModule SMSSendSMSAuditRecord = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSSendSMSAuditRecord" + Util.ProductSuffix,
                Name = "短信审核记录" + Util.MenuSuffix,
                Url = host + "/Platform/SendSMSAuditRecord",
                OrderNumber = 700,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSSendSMSAuditRecord);


            IProductModule SMSSendSMSList = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSSendSMSList" + Util.ProductSuffix,
                Name = "短信质检" + Util.MenuSuffix,
                Url = host + "/Platform/SMSSendSMSList",
                OrderNumber = 750,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSSendSMSList);

            IProductModule SMSTemplateAuditRecord = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSTemplateAuditRecord" + Util.ProductSuffix,
                Name = "模板审核记录" + Util.MenuSuffix,
                Url = host + "/Platform/TemplateAuditRecord",
                OrderNumber = 800,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSTemplateAuditRecord);
            
            IProductModule EnterpriseChannelSettingList = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "EnterpriseChannelSettingList" + Util.ProductSuffix,
                Name = "企业通道设置" + Util.MenuSuffix,
                Url = host + "/Platform/EnterpriseChannelSettingList",
                OrderNumber = 900,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(EnterpriseChannelSettingList);
         
            IProductModule SMSEnterpriseSetting = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSEnterpriseSetting" + Util.ProductSuffix,
                Name = "企业设置" + Util.MenuSuffix,
                Url = host + "/Platform/EnterpriseSetting",
                OrderNumber = 1000,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSEnterpriseSetting);

            IProductModule SMSStatistics = new ProductModule()
            {
                FunctionRole = FunctionRole.System,
                Id = "SMSStatistics" + Util.ProductSuffix,
                Name = "短信统计" + Util.MenuSuffix,
                Url = host + "/Platform/SMSStatistics",
                OrderNumber = 2100,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSStatistics);
            #endregion



            #region 企业端功能

            IProductModule SMSContacts = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSContacts" + Util.ProductSuffix,
                Name = "通讯录" + Util.MenuSuffix,
                Url = host + "/Enterprise/SMSContacts",
                OrderNumber = 10100,
                Permissions = new List<IPermission>(),
            };
            config.Modules.Add(SMSContacts);

            IProductModule SendSMS = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSSendSMS" + Util.ProductSuffix,
                Name = "发送短信" + Util.MenuSuffix,
                Url = host + "/Enterprise/SendSMS",
                OrderNumber = 10200,
                Permissions = new List<IPermission>(),
            };
            config.Modules.Add(SendSMS);

            IProductModule SMSView = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSView" + Util.ProductSuffix,
                Name = "短信查询" + Util.MenuSuffix,
                Url = host + "/Enterprise/SMSView",
                OrderNumber = 10250,
                Permissions = new List<IPermission>(),
            };
            config.Modules.Add(SMSView);

            IProductModule SMSTemplet = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSTemplet" + Util.ProductSuffix,
                Name = "短信模板" + Util.MenuSuffix,
                Url = host + "/Enterprise/SMSTemplet",
                OrderNumber = 10300,
                Permissions = new List<IPermission>(),
            };
            config.Modules.Add(SMSTemplet);

            IProductModule SMSStatus = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSStatus" + Util.ProductSuffix,
                Name = "状态报告(3天以内)" + Util.MenuSuffix,
                Url = host + "/Enterprise/SMSStatus",
                OrderNumber = 10400,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSStatus);

            IProductModule SMSStatusHistory = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSStatusHistory" + Util.ProductSuffix,
                Name = "历史状态报告" + Util.MenuSuffix,
                Url = host + "/Enterprise/SMSStatusHistory",
                OrderNumber = 10500,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSStatusHistory);


            IProductModule SMSMoList = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSMoList" + Util.ProductSuffix,
                Name = "接收短信" + Util.MenuSuffix,
                Url = host + "/Enterprise/SMSMoList",
                OrderNumber = 10600,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSMoList);

            IProductModule SMSAuditFailure = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSAuditFailure" + Util.ProductSuffix,
                Name = "审核失败短信" + Util.MenuSuffix,
                Url = host + "/Enterprise/SMSAuditFailure",
                OrderNumber = 10700,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSAuditFailure);

            IProductModule SMSEnterpriseBlackList = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSEnterpriseBlackList" + Util.ProductSuffix,
                Name = "企业黑名单" + Util.MenuSuffix,
                Url = host + "/Enterprise/SMSEnterpriseBlackList",
                OrderNumber = 10800,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSEnterpriseBlackList);

            IProductModule SMSChangePassword = new ProductModule()
            {
                FunctionRole = FunctionRole.Enterprise,
                Id = "SMSChangePassword" + Util.ProductSuffix,
                Name = "设置发送密码" + Util.MenuSuffix,
                Url = host + "/Enterprise/ChangePassword",
                OrderNumber = 10900,
                Permissions = new List<IPermission>()
            };
            config.Modules.Add(SMSChangePassword);

            #endregion

            return Content(JsonSerialize.Instance.Serialize(config));
        }

    }
}
