using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Workorder.WebPresentationModel.Filter
{
    /// <summary>
    /// 获取权限没有访问权限的异常，并输出到指定的页面
    /// </summary>
    public class SqInfoAuthorizationExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled
                && filterContext.Exception is UnauthorizedAccessException)
            {
                UrlHelper url = new UrlHelper(filterContext.RequestContext);
                filterContext.Result = new RedirectResult(url.Action("NoAccess", "Error"));
                filterContext.ExceptionHandled = true;
            }
        }
    }
}