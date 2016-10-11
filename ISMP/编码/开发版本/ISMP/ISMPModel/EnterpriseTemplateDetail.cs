using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EnterpriseTemplateDetail
    {
        /// <summary>
        /// 样式ID
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Enterprise 表的Id
        /// </summary>
        public String EnterpriseId { get; set; }
        /// <summary>
        /// Enterprise 的 AccountId
        /// </summary>
        public string EnterpriseAccountId { get; set; }

        /// <summary>
        /// 网址名
        /// </summary>
        public String WebSiteName { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        public String WebSiteUrl { get; set; }

        /// <summary>
        /// 样式编号
        /// </summary>
        public String EnterpriseTemplateId { get; set; }

        /// <summary>
        /// 集成式或分离式:0:分离式,1:集成式
        /// </summary>
        public bool IsIntegral { get; set; }

        /// <summary>
        /// 当IsIntegral=1时值为逗号分隔的CallBack,Traffic,OnlineChat组合.
        /// </summary>
        public String Products { get; set; }

        /// <summary>
        /// 是否显示400
        /// </summary>
        public bool IsShow400 { get; set; }

        /// <summary>
        /// 400号码
        /// </summary>
        public String EnterpriseNumber { get; set; }

        /// <summary>
        /// 是否显示qq
        /// </summary>
        public bool IsShowQQ { get; set; }

        /// <summary>
        /// qq
        /// </summary>
        public String QQ { get; set; }

        /// <summary>
        /// 是否显示二维码:0不显示,1:显示
        /// </summary>
        public bool IsShowQR { get; set; }

        /// <summary>
        /// 二维码路径
        /// </summary>
        public String QRCodePath { get; set; }

        /// <summary>
        /// 嵌入方式:0:悬浮,1:嵌入
        /// </summary>
        public int EmbedStyle { get; set; }

        /// <summary>
        /// 模版ID
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 如果模版颜色可以自定义的话，为可以自定义的颜色值,css中可直接使用的颜色值,如#999,
        /// </summary>
        public String TempColor { get; set; }

        /// <summary>
        /// 是否显示0否1是
        /// </summary>
        public bool IsDisplay { get; set; }

        /// <summary>
        /// 水平位置:0:左,1:右  
        /// </summary>
        public int HPosition { get; set; }

        /// <summary>
        /// 垂直位置:0:上,1:下
        /// </summary>
        public int VPosition { get; set; }

        /// <summary>
        /// 浮窗样式:0:横式,1:竖式
        /// </summary>
        public int FCStyle { get; set; }

        /// <summary>
        /// 站点类型:0:PC站点,1:无线站点
        /// </summary>
        public int WebType { get; set; }

        /// <summary>
        /// 是否主动邀请
        /// </summary>
        public int IsInvited { get; set; }

        /// <summary>
        /// 邀请间隔描述
        /// </summary>
        public int IntervalSecond { get; set; }

        /// <summary>
        /// 邀请次数
        /// </summary>
        public int InvitedTimes { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public EnterpriseTemplateDetail()
        {
            IsIntegral = false;
            IsShow400 = false;
            IsShowQQ = false;
            EmbedStyle = 0;
            IsDisplay = true;
            HPosition = 1;
            VPosition = 1;
            FCStyle = 0;
            WebType = 0;

        }


    }
}

