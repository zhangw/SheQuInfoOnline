using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SheQuInfo.Models.Model;
using Wedo.Mvc.Utility;

namespace SheQuInfo.Models.Logic
{
    public class UserService
    {
        private IUnitWork mUnitWork;

        public UserService(IUnitWork unit)
        {
            if (this.mUnitWork == null)
                this.mUnitWork = unit;
        }

        #region 私有方法

        /// <summary>
        /// 在用户信息保存前检查合法性
        /// </summary>
        /// <param name="item"></param>
        private void CheckModelBeforeSave(User item, List<Guid> roles, bool isNew)
        {
            CheckObjectNull(item, "用户对象不能为空");

            if (roles == null || roles.Count <= 0)
                throw new ArgumentException("用户至少应该含有一个角色");

            var count = mUnitWork.Users.All().Count(s => s.UserName == item.UserName);

            //新增的用户不能重名，更新的用户名可以和自己相同，但不能与已存在的其他用户相同
            if ((isNew && count > 0) || (!isNew && count > 1))
                throw new EntityExistsException(string.Format("用户名{0}已存在", item.UserName));
        }

        private void CheckObjectNull(object obj, string message)
        {
            if (obj == null)
                throw new ArgumentNullException(message);
        }

        /// <summary>
        /// 获取用户下的所有资源对象
        /// </summary>
        /// <param name="item">指定的用户对象</param>
        /// <param name="onlyNavigator">是否只保留菜单项</param>
        /// <returns>某用户能访问的资源</returns>
        private IDictionary<string, IEnumerable<Resource>> GetResourcesByModuleGroup(User item, bool onlyNavigator)
        {
            List<Resource> list = GetResourcesFromUser(item, onlyNavigator);
            var dic = new Dictionary<string, IEnumerable<Resource>>();
            foreach (var gItem in list.GroupBy(s => s.Module.Title))
            {
                dic[gItem.Key] = gItem;
            }
            return dic;
        }

        /// <summary>
        /// 获取用户下的所有资源对象
        /// </summary>
        /// <param name="item">指定的用户对象</param>
        /// <param name="onlyNavigator">是否只保留菜单项</param>
        /// <returns>某用户能访问的资源</returns>
        private List<Resource> GetResourcesFromUser(User item, bool onlyNavigator)
        {
            List<Resource> list = new List<Resource>();
            foreach (var rItem in item.Roles)
            {
                list.AddRange(rItem.Resources);
            }

            if (onlyNavigator)
                list = list.Where(s => s.Navigator == true).ToList();

            //过滤重复资源
            list = list.Distinct().OrderBy(m => m.Name).ToList();
            return list;
        }

        private List<Role> GetRolesFormId(List<Guid> ids)
        {
            return mUnitWork.Roles.Filter(m => ids.Contains(m.Id)).ToList();
        }

        #endregion 私有方法

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="item"></param>
        public void AddUser(User item, List<Guid> roles)
        {
            CheckModelBeforeSave(item, roles, true);
            var ritems = GetRolesFormId(roles);

            //_guid = item.Id = Guid.NewGuid();
            item.UserPass = StringHelper.HashPassword(item.UserPass);
            item.Roles.AddRange(ritems);
            mUnitWork.Users.Create(item);
            mUnitWork.Commit();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<User> Filter(Expression<Func<User, bool>> filter)
        {
            return mUnitWork.Users.Filter(filter).ToList();
        }

        public List<User> GetAll()
        {
            return mUnitWork.Users.All().OrderBy(m => m.UserName).ToList();
        }

        public CurrentUser GetCurrentUser(string name)
        {
            var model = GetUserByName(name);
            if (model != null)
            {
                var roles = model.Roles.Select(m => m.RoleType.ToString()).ToArray();
                return new CurrentUser(model.UserName, model.UserName, "", roles);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定用户的菜单列表
        /// </summary>
        /// <param name="item">指定的用户对象</param>
        /// <returns>
        /// 返回获取的菜单集合，格式如下:[{模块标题1,[资源列表1]},{模块标题2,[资源列表2]}]
        /// </returns>
        public IDictionary<string, IEnumerable<Resource>> GetNavigators(User item)
        {
            CheckObjectNull(item, "用户对象不能为空");

            item = mUnitWork.Users.All().FirstOrDefault(s => s.Id == item.Id);

            if (item == null)
                throw new EntityNotExistsException("用户已不存在");

            if (item.Roles == null || item.Roles.Count <= 0)
                return null;

            return GetResourcesByModuleGroup(item, true);
        }

        /// <summary>
        /// 根据用户名查询用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User GetUserByName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;
            var uitem = mUnitWork.Users.All().FirstOrDefault(s => s.UserName == userName);
            return uitem;
        }

        /// <summary>
        /// 判断用户对路径是否有访问权限
        /// </summary>
        /// <param name="item">用户</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool HasPermision(User item, string path)
        {
            CheckObjectNull(item, "用户对象不能为空");
            CheckObjectNull(path, "访问路径不能为空");
            item = mUnitWork.Users.All().FirstOrDefault(s => s.Id == item.Id);
            var resources = GetResourcesFromUser(item, false);
            if (resources == null || resources.Count <= 0)
                return false;
            return resources.Find(s => !string.IsNullOrEmpty(s.Url) && s.Url.ToLower().Trim() == path.ToLower().Trim()) != null;
        }

        /// <summary>
        /// 判断用户对路径是否有访问权限
        /// </summary>
        /// <param name="userName">用户</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool HasPermision(string userName, string path)
        {
            return HasPermision(Filter(s => s.UserName == userName).FirstOrDefault(), path);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="item"></param>
        public void RemoveUser(User item)
        {
            mUnitWork.Users.Delete(item);
            mUnitWork.Commit();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        public void RemoveUser(Guid id)
        {
            mUnitWork.Users.Delete(id);
            mUnitWork.Commit();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        public void RemoveUser(string userName)
        {
            mUnitWork.Users.Delete(s => s.UserName == userName);
            mUnitWork.Commit();
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="item"></param>
        public void UpdateUser(User item, List<Guid> roles)
        {
            CheckModelBeforeSave(item, roles, false);
            var model = GetAll().Find(m => m.Id == item.Id);
            if (item.UserPass.IsNotNullOrWhiteSpace() && item.UserPass != model.UserPass)
            {
                model.UserPass = StringHelper.HashPassword(item.UserPass);
            }
            ChangeProperties(model, item);

            if (roles != null && roles.Count > 0)
            {
                for (int i = 0; i < roles.Count; i++)
                {
                    if (roles[i].Equals(Guid.Empty))
                    {
                        roles.RemoveAt(i);
                        i--;
                    }
                }
                List<Role> rs = GetRolesFormId(roles);
                if (model.Roles != null)
                {
                    model.Roles.Clear();
                    model.Roles.AddRange(rs);
                }
                else
                {
                    model.Roles = rs;
                }
            }
            mUnitWork.Commit();
        }

        /// <summary>
        /// 修改属性，忽略外键,和一些重要数据
        /// </summary>
        /// <param name="target"></param>
        /// <param name="src"></param>
        private void ChangeProperties(User target, User src)
        {
            target.ModifiedBy = src.ModifiedBy;
            target.ModifiedDate = DateTime.Now;
            target.UserDesc = src.UserDesc;
            target.UserName = src.UserName;

            //target.UserPass = src.UserPass;
        }

        #region membership

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="oldpass">旧密码</param>
        /// <param name="newPass">新密码</param>
        /// <returns></returns>
        public bool ChangePassword(string userName, string oldpass, string newPass)
        {
            if (string.IsNullOrWhiteSpace(newPass))
                throw new ArgumentNullException("newpass", "新密码不能为空");

            if (oldpass == newPass)
                throw new ArgumentException("新旧密码相同，不需要更改");

            var uitem = CheckUserName(userName);

            if (uitem.UserPass != StringHelper.HashPassword(oldpass))
                throw new ArgumentException("旧密码不符合", oldpass);

            uitem.UserPass = StringHelper.HashPassword(newPass);
            uitem.ModifiedDate = DateTime.Now;

            mUnitWork.Users.Update(uitem);
            mUnitWork.Commit();
            return true;
        }

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Login(string userName, string password)
        {
            string sql = "select UserPass from Users where `UserName`='{0}' ";

            //var uitem = CheckUserName(userName);
            string ps = mUnitWork.SqlQuery<string>(string.Format(sql, userName)).FirstOrDefault();
            return StringHelper.HashPassword(password) == ps;
        }

        /// <summary>
        /// 检查用户是否合法，包括是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private User CheckUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException("username", "用户对象不能为空");
            var uitem = mUnitWork.Users.All().FirstOrDefault(s => s.UserName == userName);
            if (uitem == null)
                throw new EntityNotExistsException(string.Format("指定的用户名称{0}不存在", userName));
            return uitem;
        }

        #endregion membership
    }
}