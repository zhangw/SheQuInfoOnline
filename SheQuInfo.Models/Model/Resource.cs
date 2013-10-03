using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SheQuInfo.Models.Model
{
    /// <summary>
    /// 资源
    /// </summary>
    public class Resource : Entity
    {
        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string Desc { set; get; }

        /// <summary>
        /// 所在模块
        /// </summary>
        public virtual Module Module { set; get; }

        public Guid ModuleId { set; get; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [Required]
        public string Name { set; get; }

        /// <summary>
        /// 是否为菜单
        /// </summary>
        [Display(Name = "是否为菜单")]
        public bool Navigator { set; get; }

        /// <summary>
        /// 关联的角色
        /// </summary>
        [Display(Name = "关联的角色")]
        public virtual List<Role> Roles { set; get; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required]
        public string Title { set; get; }

        /// <summary>
        /// 路径
        /// </summary>
        [Display(Name = "路径")]
        [Required]
        public string Url { set; get; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Resource))
                return false;
            return (obj as Resource).Id == Id;
        }
    }
}