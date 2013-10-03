using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc; 

namespace Wedo.MVC.Controllers.Filter
{
    /// <summary>
    /// 访问验证
    /// </summary>
    public class WorkorderAuthorizationFilter : IAuthorizationFilter
    { 
        public WorkorderAuthorizationFilter()
        {
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.AuthenticationType == "Windows")
                return;

            string path = string.Format("/{0}/{1}",
                                         filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                                         filterContext.ActionDescriptor.ActionName);

            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Error")
                return;

            if (path == "/Account/LogOn")
                return;

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("/Account/LogOn");
                return;
            }

            //暂时不去检查权限的问题
            //string userName = filterContext.HttpContext.User.Identity.Name;

            //if (!_service.HasPermision(userName, path))
            //{
            //    //filterContext.Result = new RedirectResult("/Error/NoAccess");
            //    throw new UnauthorizedAccessException("您当前没有访问的权限");
            //}
        }
    }
}
