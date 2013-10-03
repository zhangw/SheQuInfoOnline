using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc; 
using System.Web.Security;

namespace Wedo.MVC.Controllers.Filter
{
    public class CustomGlobalActionFilterAttribute : ActionFilterAttribute
    {
 

        public CustomGlobalActionFilterAttribute()
        { 
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string path = string.Format("/{0}/{1}",
                                        filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                                        filterContext.ActionDescriptor.ActionName);

            if ("/account/logon" == path.Trim().ToLower())
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            //redirect if not authenticated
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //use the current url for the redirect
                string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;
                //send them off to the login page
                string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                string loginUrl = FormsAuthentication.LoginUrl + redirectUrl;
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
                return;
            }
            else
            {
                //if (!_service.HasPermision(filterContext.HttpContext.User.Identity.Name, path))
                //    throw new UnauthorizedAccessException("您没有权限访问");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
