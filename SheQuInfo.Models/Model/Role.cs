using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SheQuInfo.Models.Model
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role : Entity
    {
        /// <summary>
        /// 拥有的资源
        /// </summary>
        [Display(Name = "拥有的资源")]
        public virtual List<Resource> Resources { set; get; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [Display(Name = "角色描述")]
        public string RoleDesc { set; get; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Display(Name = "角色名称")]
        [Required]
        public string RoleName { set; get; }

        /// <summary>
        /// 角色类型,
        /// 1.系统管理员
        /// </summary>
        [Display(Name = "角色类型")]
        public int RoleType { get; set; }

        /// <summary>
        /// 拥有的用户
        /// </summary>
        [Display(Name = "拥有的用户")]
        public virtual List<User> Users { set; get; }
    }
}