using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SheQuInfo.Models
{
    /// <summary>
    /// 所有实体的基类
    /// </summary>
    public abstract class Entity
    {
        public Entity()
        {
            CreatedBy = "";
            CreatedDate = DateTime.Now;
            ModifiedBy = "";
            ModifiedDate = DateTime.Now;
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// 创建人
        /// </summary>
        [DisplayName("创建人")]
        public string CreatedBy { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime CreatedDate { set; get; }

        /// <summary>
        /// 编号
        /// </summary>
        [DisplayName("编号")]
        public Guid Id { set; get; }

        /// <summary>
        /// 修改人
        /// </summary>
        [DisplayName("修改人")]
        public string ModifiedBy { set; get; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DisplayName("修改时间")]
        public DateTime ModifiedDate { set; get; }
    }
}