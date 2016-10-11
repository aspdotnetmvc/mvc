namespace php ACSMSService

include "ACSMSModel.thrift"

service ACSMSService { 

    ACSMSModel.RPCResult AccountDeductSMSCharge(1:string accountID, 2:i32 quantity)
	//发送短信
    ACSMSModel.RPCResult SendSMS(1:ACSMSModel.SMS sms, 2:bool isSMSTimer)
	//获取所有敏感词
	ACSMSModel.RPCKeywordsListResult GetAllKeywords(1:i32 pSize,2:i32 pIndex)
	// 短信状态报告明细（缓存无，数据库获取）
    ACSMSModel.RPCStatusReportListResult GetSMSStatusReport(1:string serialNumber, 2:string sendTime,3:i32 pSize,4:i32 pIndex)
    // 查看短信统计报告（缓存无，数据库获取）
    ACSMSModel.ReportStatistics GetReportStatistics(1:string serialNumber, 2:string sendTime)
    // 短信原始单记录
    ACSMSModel.RPCSMSListResult GetSMSRecordByAccount(1:string account, 2:string beginTime, 3:string endTime,4:i32 pSize,5:i32 pIndex)
    // 总短信统计（发送数，失败数，不统计缓存数据）
    ACSMSModel.SMSSendResult GetSMSStatistics(1:string beginTime, 2:string endTime)
	// add:各企业的短信统计（发送数，失败数，不统计缓存数据）
	ACSMSModel.RPCAccountSMSStatisticsListResult GetAccountSMSStatistics(1:string beginTime, 2:string endTime,3:i32 pSize,4:i32 pIndex)
    // 企业账号短信统计（发送数，失败数，不统计缓存数据）
    ACSMSModel.SMSSendResult GetSMSStatisticsByAccount(1:string accountID, 2:string beginTime, 3:string endTime)
	// 根据账号获取统计报告（缓存获取）
    ACSMSModel.RPCReportStatisticListResult GetDirectStatisticReportByAccount(1:string account,2:i32 pSize,3:i32 pIndex)
}