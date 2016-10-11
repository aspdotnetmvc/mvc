namespace php ACSMSModel

//状态报告类型
enum StatusReportType{
        // 启用
        Enabled = 1,
        // 不启用
        Disable = 0,
        // 推送
        Push=2,
}
//短信发送级别
enum LevelType {
	    Level0 =0,
        Level1=1,
        Level2=2,
        Level3=3,
        Level4=4,
        Level5=5,
        Level6=6,
        Level7=7,
        Level8=8,
        Level9=9,
        Level10=10,
        Level11=11,
        level12=12,
} 
//短信审核类型
enum AuditType{
        // 人工审核
        Manual=0,
        // 自动
        Auto=1,
}
//企业审核鉴权
enum AccountAuditType{
        Manual = 0,// 人工审核
        Auto = 1,// 自动
}

//过滤类型
enum FilterType{
        // 无操作
        NoOperation =0,
        // 替换
        Replace = 1,
        // 发送失败
        Failure = 2,
}

//短信
struct SMS{
         1:string Account //帐号
         2:string SerialNumber //业务流水号
         3:list<string> Number //发送号码列表
         4:i32 NumberCount // 发送号码个数
         5:string Content//短信内容
         6:StatusReportType StatusReport// 状态报告接收方式
         7:LevelType Level//等级（0-6，数字越高优先级越高）；默认2
         8:string SendTime//发送时间
         9:string SMSTimer// 定时短信
         10:AuditType Audit//短信内容审核方式
         11:FilterType Filter//短信内容过滤方式
         12:string Channel // 短信发送通道
         13:string Signature // 签名
         14:string SPNumber // SPNumber
         15:string WapURL // WapPush URL
         16:string LinkID   // LinkID
}

//敏感词
struct Keywords{
        1:string KeyGroup // 敏感词词组
        2:string Words // 敏感词
        3:string KeywordsType // 敏感词类型
        4:bool Enable // 是否启用
        5:string ReplaceKeywords // 敏感词替换成其他词
}

//短信报告统计
struct ReportStatistics{
        1:string Account
        2:string SerialNumber // 业务流水号
        3:i32 Numbers // 下发的号码数
        4:i32 SendCount // 发送条数
        5:i32 SplitNumber // 短信拆分数
        6:i32 FailureCount // 发送失败统计
        7:i32 Succeed// 发送成功统计
        8:string SendTime // 提交时间
        9:string BeginSendTime // 开始发送时间
        10:string LastResponseTime // 最后接收报告时间
        11:string SMSContent // 短信内容
        12:list<string> Telephones // 此业务发送的号码
}

//状态报告
struct StatusReport{
        1:i32 StatusCode// 状态码
        2:SMS Message//发送的短消息
        3:string Serial// 网关返回的序列号
        4:string Describe// 状态描述
        5:bool Succeed// 是否发送成功
        6:i32 SplitTotal// 短信拆分的总条数
        7:i32 SplitNumber// 当前是第几条拆分
        8:string ResponseTime// 状态报告返回时间
        9:string Gateway//网关的ID
}


//短信统计
struct SMSStatistics{
    1: string AccountID //帐号
    2: i32 SendCount//发送条数
	3: i32 FailureCount//失败条数
}

struct RPCResult{
    1:bool Success
    2:string Message
	3:string Data
}

struct RPCSMSListResult{
    1:bool Success
    2:string Message
	3:i32 Total
	4:i32 PageCount
	5:list<SMS> SMSList
}

struct RPCKeywordsListResult{
    1:bool Success
    2:string Message
	3:i32 Total
	4:i32 PageCount
	5:list<Keywords> Words
}

struct RPCAccountSMSStatisticsListResult{
    1:bool Success
    2:string Message
	3:i32 Total
	4:i32 PageCount
	5:list<SMSStatistics> Statistics
}

struct RPCStatusReportListResult{
    1:bool Success
    2:string Message
	3:i32 Total
	4:i32 PageCount
	5:list<StatusReport> Records
}

struct RPCReportStatisticListResult{
    1:bool Success
    2:string Message
	3:i32 Total
	4:i32 PageCount
	5:list<ReportStatistics> Reports
}

struct SMSSendResult{
    1:i32 SendCount
	2:i32 FailureCount
	3:bool Success
	4:string Message
}

struct DictionaryC{
   1:string Key
   2:string Value
}

struct RPCDictionaryResult{
    1:bool Success
    2:string Message
	3:i32 Total
	4:i32 PageCount
	5:list<DictionaryC> Dictionarys
}

struct RPCListResult{
    1:bool Success
    2:string Message
	3:i32 Total
	4:i32 PageCount
	5:list<string> Lists
}