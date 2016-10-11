using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// EnterprisePluglinTemplate
    /// </summary>
    [Serializable]
    public class EnterprisePluglinTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 是否集成式:0:分离式,1:集成式
        /// </summary>
        public bool IsIntegral { get; set; }

        /// <summary>
        /// 应用ID: :网页回呼. :在线客服, :安全认证
        /// </summary>
        public String ProductId { get; set; }

        /// <summary>
        /// 模版名称
        /// </summary>
        public String TempName { get; set; }

        /// <summary>
        /// 图片路径文件名前缀
        /// </summary>
        public String ImgPrefix { get; set; }

        /// <summary>
        /// 模版路径
        /// </summary>
        public String TempPath { get; set; }

        /// <summary>
        /// 是否允许自定义颜色,0:不允许,1:允许
        /// </summary>
        public bool IsAllowDefineColor { get; set; }

        /// <summary>
        /// 模版可选颜色
        /// </summary>
        public String TempColor { get; set; }

        /// <summary>
        /// 站点类型:0:PC站点,1:无线站点
        /// </summary>
        public Byte WebType { get; set; }

    }
}
