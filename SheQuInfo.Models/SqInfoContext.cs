using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using SheQuInfo.Models.Mappings;
using SheQuInfo.Models.Model;

namespace SheQuInfo.Models
{
    public class SqInfoContext : DbContext
    {
        public SqInfoContext(string nameOrConnnectString)
            : base(nameOrConnnectString)
        {
        }

        #region privileges

        /// <summary>
        /// 模块
        /// </summary>
        public DbSet<Module> Modules { set; get; }

        /// <summary>
        /// 资源
        /// </summary>
        public DbSet<Resource> Resources { set; get; }

        /// <summary>
        /// 角色
        /// </summary>
        public DbSet<Role> Roles { set; get; }

        /// <summary>
        /// 用户
        /// </summary>
        public DbSet<User> Users { set; get; }

        #endregion privileges

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //privileges
            modelBuilder.Configurations.Add<User>(new UserConfiguration());
            modelBuilder.Configurations.Add<Role>(new RoleConfiguration());
            modelBuilder.Configurations.Add<Module>(new ModuleConfiguration());
            modelBuilder.Configurations.Add<Resource>(new ResourceConfiguration());
        }
    }
}