namespace php ACModle

enum State{
	Enable=0,//����
	Blocked=1,//����
	Delete=4//ɾ��
}

enum Operate {
	Enable=0  //����
	Blocked=1, //����
	Recharge=2,  //��ֵ
	Deduction=3, //�ۿ�
	Delete=4, //ɾ��
	Create=5,  //����
} 

enum ErrorCode{
	Unknown, //δ֪����
	DuplicateKey, //�����ظ�
	ParameterError, //��������
	NsufficientBalance, //����
	AccountNotExist, //�˻�������
	ProductNotExist, //��Ʒ������
	ProductBlocked //��Ʒ�˻�����
}


//��������
struct CommonReturn{
	1:bool Result
	2:ErrorCode ErrorCode
}

//��ѯ��������
struct BalanceReturn{
	1:bool Result
	2:ErrorCode ErrorCode
	3:double Balance
}

//��Ʒ�˻�
struct Product{
	1:string ProductID //ID
	2:double Amount //���
	3:double Proportion //��ֵ����
	4:i32 Transaction //��������
	5:string Describe //��������
}

//�˻�
struct Account{ 
    1: string AccountID //ID
    2: list<Product>Products //��Ʒ�б�
  }


  //��Ʒͳ�Ʋ�ѯ����
struct ProductQuerySRCondition{
	1:string ProductID //��ƷID
	2:list<i32> DeductionTypes //�ۿ�����
	3:list<i32> RechargeTypes //��ֵ����
}

//�˻�ͳ�Ʊ���
struct StatisticalReport{
	1:string Product //��Ʒ
	2:i32 Amount //��Ʒ���
	3:double DeductionCount //�ۿ�ͳ��
	4:double RechargeCount //��ֵͳ��
}

//��Ʒ������ϸ
struct ProductDetail{
    1:string Account //�˺�
	2:string Time  //����ʱ��
	3:double Amount //���
	4:double Proportion //��ֵ����
	5:double RechargeAmount //��ֵ���
	6:i32 Transaction //��������
	7:string Describe //��������
	
}

//������ϸ����
struct DetailReport{
	1:string Account //�˻�
	2:string Product //��Ʒ
	3:list<ProductDetail> Products 
}