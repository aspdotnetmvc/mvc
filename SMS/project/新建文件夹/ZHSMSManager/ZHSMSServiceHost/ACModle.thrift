namespace php ACModle

enum State{
	Enable=0,//启用
	Blocked=1,//冻结
	Delete=4//删除
}

enum Operate {
	Enable=0  //启用
	Blocked=1, //冻结
	Recharge=2,  //充值
	Deduction=3, //扣款
	Delete=4, //删除
	Create=5,  //创建
} 

enum ErrorCode{
	Unknown, //未知错误
	DuplicateKey, //主键重复
	ParameterError, //参数错误
	NsufficientBalance, //余额不足
	AccountNotExist, //账户不存在
	ProductNotExist, //产品不存在
	ProductBlocked //产品账户冻结
}


//返回类型
struct CommonReturn{
	1:bool Result
	2:ErrorCode ErrorCode
}

//查询余额返回类型
struct BalanceReturn{
	1:bool Result
	2:ErrorCode ErrorCode
	3:double Balance
}

//产品账户
struct Product{
	1:string ProductID //ID
	2:double Amount //金额
	3:double Proportion //充值比例
	4:i32 Transaction //交易类型
	5:string Describe //交易描述
}

//账户
struct Account{ 
    1: string AccountID //ID
    2: list<Product>Products //产品列表
  }


  //产品统计查询条件
struct ProductQuerySRCondition{
	1:string ProductID //产品ID
	2:list<i32> DeductionTypes //扣款类型
	3:list<i32> RechargeTypes //充值类型
}

//账户统计报表
struct StatisticalReport{
	1:string Product //产品
	2:i32 Amount //产品余额
	3:double DeductionCount //扣款统计
	4:double RechargeCount //充值统计
}

//产品交易明细
struct ProductDetail{
    1:string Account //账号
	2:string Time  //交易时间
	3:double Amount //金额
	4:double Proportion //充值比例
	5:double RechargeAmount //充值金额
	6:i32 Transaction //交易类型
	7:string Describe //交易描述
	
}

//交易明细报表
struct DetailReport{
	1:string Account //账户
	2:string Product //产品
	3:list<ProductDetail> Products 
}