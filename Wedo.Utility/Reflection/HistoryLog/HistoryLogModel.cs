using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wedo.Utility.Reflection.HistoryLog
{
    /// <summary>
    /// 对象变更记录
    /// </summary>
    public class HistoryLogModel
    {
        /// <summary>
        /// 记录编号
        /// </summary>
        public Guid Id { set; get; }

        /// <summary>
        /// 实体名称或是表格名称，与OwnerId共同确认一条记录
        /// </summary>
        public string ModelName { set; get; }

        /// <summary>
        /// 拥有者Id,批某个表的某一行的编号
        /// </summary>
        public string OwnerId { set; get; }

        /// <summary>
        /// 版本，一个记录可以被变更多次，每次用一个版本号表示
        /// </summary>
        public int Version { set; get; }

        /// <summary>
        /// 详细变更记录
        /// </summary>
        public virtual List<HistoryLogItem> ModelChangeLogItems { set; get; }

        /// <summary>
        /// 变更人
        /// </summary>
        public string ChangeMan { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { set; get; }
    }

    /// <summary>
    /// 变更的详细信息
    /// </summary>
    public class HistoryLogItem
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        public Guid Id { set; get; }

        /// <summary>
        /// 父编号
        /// </summary>
        public Guid ParentId { set; get; }

        /// <summary>
        /// 父对象
        /// </summary>
        public virtual HistoryLogModel ModelChangeLog { set; get; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }

        /// <summary>
        /// 原有值
        /// </summary>
        public string SourceValue { set; get; }

        /// <summary>
        /// 更改后的值
        /// </summary>
        public string NewValue { set; get; }
    }
}
