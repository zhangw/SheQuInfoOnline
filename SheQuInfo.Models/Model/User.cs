using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SheQuInfo.Models.Model
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User : Entity
    {
        /// <summary>
        /// 邮件地址
        /// </summary>
        [Display(Name = "邮件地址")]

        //[Required]
        [UIHint("email")]
        public string Email { set; get; }

        /// <summary>
        /// 所在角色
        /// </summary>
        [Display(Name = "所在角色")]
        public virtual List<Role> Roles { set; get; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string UserDesc { set; get; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名必须填写")]
        [Display(Name = "用户名")]
        public string UserName { set; get; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "必须设置密码")]
        [Display(Name = "用户密码")]
        [UIHint("password")]
        public string UserPass { set; get; }
    }
}