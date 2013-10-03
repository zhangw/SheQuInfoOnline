using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using SheQuInfo.Models.Model;

namespace SheQuInfo.Models.Repository
{
    public class SqlRoleRepository : BaseRepository<Role>
    {
        public SqlRoleRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="item"></param>
        public override void Update(Role item)
        {
            var temp = _dbSet.FirstOrDefault(s => s.Id == item.Id);
            _dbContext.Entry<Role>(temp).CurrentValues.SetValues(item);
        }
    }
}