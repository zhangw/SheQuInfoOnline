using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SheQuInfo.Models.Model
{
    /// <summary>
    /// 模块
    /// </summary>
    public class Module : Entity
    {
        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "模块描述")]
        public string ModuleDesc { set; get; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "模块名称")]
        [Required]
        public string ModuleName { set; get; }

        /// <summary>
        /// 其下资源
        /// </summary>
        [Display(Name = "其下资源")]
        public virtual List<Resource> Resources { set; get; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "模块标题")]
        [Required]
        public string Title { set; get; }
    }
}