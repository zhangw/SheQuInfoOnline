using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wedo.Utility.File
{
    /// <summary>
    /// 对象配置文件中的单个对象
    /// </summary>
    public class ObjectMapping
    {
        /// <summary>
        /// 对象名称，如ThermoFisher.MDM.Domain.Model.KeyAccount
        /// </summary>
        public string ObjectName { set; get; }

        /// <summary>
        /// 编码 utf8
        /// </summary>
        public Encoding Encoding { set; get; }

        /// <summary>
        /// CSV时为分隔符;当为EXCEL时为SHEET名称,如果有多个SHEET名称，则用逗号隔开
        /// </summary>
        public string Splitter { set; get; }

        /// <summary>
        /// 对象下的所有属性
        /// </summary>
        public List<ObjectMappingItem> Properties { set; get; }
    }

    /// <summary>
    /// 对象配置文件中的单条记录
    /// </summary>
    public class ObjectMappingItem
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { set; get; }

        /// <summary>
        /// 属性类型，如System.String
        /// </summary>
        public string PropertyType { set; get; }

        /// <summary>
        /// 文件中对应的标题
        /// </summary>
        public string FileTitle { set; get; }
    }
}
