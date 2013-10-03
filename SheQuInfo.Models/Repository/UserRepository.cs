using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using SheQuInfo.Models.Model;

namespace SheQuInfo.Models.Repository
{
    public class SqlUserRepository : BaseRepository<User>
    {
        public SqlUserRepository(DbContext context)
            : base(context)
        {
        }

        public void Update(User item, List<Guid> roleIds)
        {
            var temp = _dbSet.FirstOrDefault(s => s.Id == item.Id);
            _dbContext.Entry<User>(temp).CurrentValues.SetValues(item);

            for (int i = 0; i < roleIds.Count; i++)
            {
                if (roleIds[i].Equals(Guid.Empty))
                {
                    roleIds.RemoveAt(i);
                    i--;
                }
            }

            //_dbSet.Attach(item);
            if (roleIds != null && roleIds.Count > 0)
            {
                List<Role> roles = GetRolesFormId(roleIds);
                temp.Roles.Clear();
                temp.Roles.AddRange(roles);
            }

            //_dbContext.Entry<User>(item).State = System.Data.EntityState.Modified;
        }

        private List<Role> GetRolesFormId(List<Guid> ids)
        {
            return this._dbContext.Set<Role>().Where(m => ids.Contains(m.Id)).ToList();
        }

        //public override User GetById(object id)
        //{
        //    return _dbSet.AsNoTracking().FirstOrDefault(s => s.Id == (Guid)id);
        //    //return base.GetById(id);
        //}
    }
}