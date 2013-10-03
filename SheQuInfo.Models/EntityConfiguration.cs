using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace SheQuInfo.Models
{
    public class EntityConfiguration<T> : EntityTypeConfiguration<T> where T : Entity
    {
        public EntityConfiguration()
        {
            HasKey(s => s.Id).Property(s => s.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(s => s.CreatedBy).HasColumnType("varchar").HasMaxLength(100).IsRequired();
            Property(s => s.CreatedDate).HasColumnType("datetime").IsRequired();
            Property(s => s.ModifiedBy).HasColumnType("varchar").HasMaxLength(100).IsRequired();
            Property(s => s.ModifiedDate).HasColumnType("datetime").IsRequired();
        }
    }
}