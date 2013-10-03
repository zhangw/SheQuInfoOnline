using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SheQuInfo.Models;
using SheQuInfo.Models.Logic;
using SheQuInfo.Models.Model;
using SheQuInfo.Models.Repository;
using Wedo.Mvc.Utility;

namespace SheQuInfo.Web.Controller
{
    public partial class UserController : SheQuController
    {
        private UserService mUserService = null;

        public UserController(IUnitWork unitWork)
            : base(unitWork)
        {
            mUserService = new UserService(unitWork);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Roles = Wedo.Mvc.Utility.SelectListHelper.GenerateFromList<Role>(this.mUnitWork.Roles.All().ToList(), "RoleName", "Id", needDefault: false);
            return PartialView();
        }

        [HttpPost]
        public JsonResult Create(User model, List<Guid> Roles, string FlowRoles)
        {
            ResultMSG msg = new ResultMSG();
            msg.MSG = "新建用户成功";
            msg.Result = true;
            try
            {
                mUserService.AddUser(model, Roles);
            }
            catch (Exception ex)
            {
                msg.Result = false;
                msg.MSG = string.Format("新建用户失败,原因:{0}", ex.Message);
            }
            return Json(msg);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var model = this.mUnitWork.Users.GetById(id);
            ViewBag.Roles = Wedo.Mvc.Utility.SelectListHelper.GenerateFromList<Role>(this.mUnitWork.Roles.All().ToList(), model.Roles, "RoleName", "Id");
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult Edit(User model, List<Guid> Roles)
        {
            ResultMSG msg = new ResultMSG();
            msg.MSG = "更新用户成功";
            msg.Result = true;
            try
            {
                mUserService.UpdateUser(model, Roles);
            }
            catch (Exception ex)
            {
                msg.Result = false;
                msg.MSG = string.Format("更新用户失败,原因:{0}", ex.Message);
            }
            return Json(msg);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return PartialView(mUserService.GetAll());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Remove(Guid id)
        {
            ResultMSG msg = new ResultMSG();
            msg.MSG = "删除用户成功";
            msg.Result = true;
            try
            {
                mUserService.RemoveUser(id);
            }
            catch (Exception ex)
            {
                msg.Result = false;
                msg.MSG = string.Format("删除用户失败,原因:{0}", ex.Message);
            }
            return Json(msg);
        }

        public ActionResult UserNavigators()
        {
            var user = mUserService.GetCurrentUser(CurrentUserName);
            if (user != null)
            {
                //var dict = _uService.GetNavigators(user);
                var dict = Navigator.GetCurrentNav(user);
                return PartialView("Navigator", dict);
            }
            else
            {
                return PartialView("Navigator", new List<Navigation>());

                //return PartialView(new Dictionary<string, IEnumerable<Resource>> ());
            }
        }
    }
}