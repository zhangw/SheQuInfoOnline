using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SheQuInfo.Models.Model;

namespace SheQuInfo.Models.Mappings
{
    public class ModuleConfiguration : EntityConfiguration<Module>
    {
        public ModuleConfiguration()
            : base()
        {
            ToTable("Modules");
            Property(s => s.ModuleName).HasColumnType("varchar").HasMaxLength(100).IsRequired();
            Property(s => s.Title).HasColumnType("varchar").HasMaxLength(100).IsRequired();
            Property(s => s.ModuleDesc).HasColumnType("varchar").HasMaxLength(100).IsOptional();

            HasMany(s => s.Resources).WithRequired(s => s.Module).HasForeignKey(s => s.ModuleId);
        }
    }

    public class ResourceConfiguration : EntityConfiguration<Resource>
    {
        public ResourceConfiguration()
            : base()
        {
            ToTable("Resources");

            Property(s => s.Name).HasColumnType("varchar").HasMaxLength(100).IsRequired();
            Property(s => s.Desc).HasColumnType("varchar").HasMaxLength(100).IsOptional();
            Property(s => s.Url).HasColumnType("varchar").HasMaxLength(100).IsRequired();

            Property(s => s.Title).HasColumnType("varchar").HasMaxLength(100);

            Property(s => s.Navigator).HasColumnType("Bit").IsRequired();

            HasMany(s => s.Roles).WithMany(s => s.Resources);
        }
    }

    public class RoleConfiguration : EntityConfiguration<Role>
    {
        public RoleConfiguration()
            : base()
        {
            ToTable("Roles");

            Property(s => s.RoleName).HasColumnType("varchar").HasMaxLength(100).IsRequired();
            Property(s => s.RoleDesc).HasColumnType("varchar").HasMaxLength(100).IsOptional();
        }
    }

    public class UserConfiguration : EntityConfiguration<User>
    {
        public UserConfiguration()
            : base()
        {
            ToTable("Users");
            Property(s => s.UserName).HasColumnType("varchar").HasMaxLength(100);
            Property(s => s.UserPass).HasColumnType("varchar").HasMaxLength(100);
            Property(s => s.UserDesc).HasColumnType("varchar").HasMaxLength(100).IsOptional();
            Property(s => s.Email).HasColumnType("varchar").HasMaxLength(100).IsOptional();

            HasMany(s => s.Roles).WithMany(s => s.Users);
        }
    }
}