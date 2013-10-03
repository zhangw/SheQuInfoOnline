using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SheQuInfo.Models.Repository;

namespace SheQuInfo.Models
{
    public interface IUnitWork
    {
        SqlRoleRepository Roles { get; }

        SqlUserRepository Users { get; }

        void Commit();

        int ExSql(string sql, params string[] param);

        IEnumerable<T> SqlQuery<T>(string sql, params object[] param);
    }

    public class SqInfoUnitWork : IUnitWork
    {
        private SqInfoContext mContext = null;
        private bool mDisposed = false;
        private SqlRoleRepository mRoleRepository = null;
        private SqlUserRepository mUserRepository = null;

        public SqInfoUnitWork(SqInfoContext context)
        {
            if (mContext == null)
                mContext = context;
        }

        public SqlRoleRepository Roles
        {
            get
            {
                if (mRoleRepository == null)
                    mRoleRepository = new SqlRoleRepository(mContext);
                return mRoleRepository;
            }
        }

        public SqlUserRepository Users
        {
            get
            {
                if (mUserRepository == null)
                    mUserRepository = new SqlUserRepository(mContext);
                return mUserRepository;
            }
        }

        public void Commit()
        {
            mContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int ExSql(string sql, params string[] param)
        {
            return mContext.Database.ExecuteSqlCommand(sql, param);
        }

        public IEnumerable<T> SqlQuery<T>(string sql, params object[] param)
        {
            return mContext.Database.SqlQuery<T>(sql, param);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                if (disposing)
                {
                    mContext.Dispose();
                }
            }
            mDisposed = true;
        }
    }
}