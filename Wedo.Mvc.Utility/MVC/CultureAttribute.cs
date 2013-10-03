using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace Wedo.Mvc.Utility
{
    public class SetCultureAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        { 
            string cultureCode = SetCurrentLanguage(filterContext);
            CultureInfo culture = new CultureInfo(cultureCode);
            //CultureInfo culture = new CultureInfo("zh-CN");
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        private static string GetBrowserCulture(ActionExecutingContext filterContext, IEnumerable<string> Cultures)
        {
            /* Gets Languages from Browser */
            IList<string> BrowserLanguages = filterContext.RequestContext.HttpContext.Request.UserLanguages;
            if (BrowserLanguages != null) 
            {
                foreach (var thisBrowserLanguage in BrowserLanguages)
                {
                    foreach (var thisCultureLanguage in Cultures)
                    {
                        if (!thisBrowserLanguage.Contains(thisCultureLanguage)) continue;
                        return thisCultureLanguage;
                    }
                }
            }
            return string.Empty;
        }

        private static string GetCookieCulture(ActionExecutingContext filterContext, ICollection<string> Cultures)
        {
            /* Get the language in the cookie*/
            HttpCookie userCookie = filterContext.RequestContext.HttpContext.Request.Cookies["Culture"];

            if (userCookie != null)
            {
                if (!string.IsNullOrEmpty(userCookie.Value))
                {
                    if (Cultures.Contains(userCookie.Value))
                    {
                        return userCookie.Value;
                    }
                    return string.Empty;
                }
                return string.Empty;
            }
            return string.Empty;
        }



        private static string SetCurrentLanguage(ActionExecutingContext filterContext)
        {
            //语言选项，目前是中文和英文
            IList<string> Cultures = new List<string> { "zh-CN", "en-US" };
            string  CookieValue=GetCookieCulture(filterContext, Cultures);
            if (string.IsNullOrEmpty(CookieValue))
            {
                //默认语言设置
              //  CookieValue = GetBrowserCulture(filterContext, Cultures) ?? "en-US";
              //陈宗波修改，用于修正第一次加载语言不正确问题
                CookieValue = "zh-CN";
            }
            return CookieValue;
        }
    }
}
