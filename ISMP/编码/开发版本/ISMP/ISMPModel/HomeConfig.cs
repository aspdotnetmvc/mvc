using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 首页配置
    /// </summary>
    [Serializable]
    public class HomeConfig
    {
        /// <summary>
        /// Id
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// 首页区域编号(div 的Id)
        /// </summary>
        public String RegionId { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public String RoleId { get; set; }

        /// <summary>
        /// 优先级（一个用户有多个角色时，取优先级最高的记录）
        /// </summary>
        public int Priority { get; set; }

    }
}
