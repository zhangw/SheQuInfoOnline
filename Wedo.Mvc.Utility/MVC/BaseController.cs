using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Linq.Expressions;
using System.Globalization;
using Wedo.Mvc.Pager; 
namespace Wedo.Mvc.Utility
{
    public class BaseController : Controller
    {
        private string _CurrentUser;

        private CurrentUser _User;

        /// <summary>
        /// 当前用户，需要在Global中重写PostAuthenticateRequest事件
        /// </summary>
        protected virtual CurrentUser GetCurrentUser
        {
            get {
                if (_User == null)
                {
                    _User = User.GetCurrentUserInfo();
                }
                return _User; 
            }
        }

        /// <summary>
        /// 当前登陆用户名称
        /// </summary>
        protected  string CurrentUserName
        {
            get
            {
                if (_CurrentUser == null)
                {
                    _CurrentUser= User.Identity.Name;
                }
                return _CurrentUser;
            }
        }

        #region Action
        /// <summary>
        /// 根据是否是js请求 返回页面
        /// </summary>
        protected ActionResult ReturnAjaxView()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            else
                return View();
        }

        /// <summary>
        /// 根据是否是js请求 返回页面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected ActionResult ReturnAjaxView(object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            else
                return View(model);
        }

        /// <summary>
        /// 根据是否是js请求 返回页面
        /// </summary>
        protected ActionResult ReturnAjaxView(string viewName, object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(viewName, model);
            }
            else
                return View(viewName, model);

        }

        /// <summary>
        /// 默认允许所有类型数据返回
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected new JsonResult Json(object obj)
        {
            return base.Json(obj, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult ErrorMSG(string msg)
        {
            return Json(new ResultMSG(msg));
        }

        protected JsonResult SuccessMsg(string msg)
        {
            return Json(new ResultMSG(msg,true));
        }
        #endregion
        
     }
}
