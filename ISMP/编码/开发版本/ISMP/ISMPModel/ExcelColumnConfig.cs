using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 导出Excel时列配置
    /// </summary>
    [Serializable]
    public class ExcelColumnConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName">显示的列名称</param>
        /// <param name="columnWidth">列宽，以一个字符的1/256的宽度作为一个单位</param>
        public ExcelColumnConfig(string columnName, int columnWidth = 20*256, bool isAlignRight = false)
        {
            this.ColumnName = columnName;
            this.ColumnWidth = columnWidth;
            this.IsAlignRight = isAlignRight;
        }

        /// <summary>
        /// 显示的列名称
        /// </summary>
        public String ColumnName { get; set; }

        /// <summary>
        /// 列宽，以一个字符的1/256的宽度作为一个单位
        /// </summary>
        public int ColumnWidth { get; set; }

        /// <summary>
        /// 是否靠右显示，默认靠左
        /// </summary>
        public bool IsAlignRight { get; set; }
    }

}
