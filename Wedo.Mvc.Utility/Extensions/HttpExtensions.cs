using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Wedo.Mvc.Utility
{
    public static class HttpExtensions
    {

        public static void CreateCookie(this HttpResponseBase response, string name, string value, DateTime? Expires = null)
        {
            if (response.Cookies[name] != null)
            {
                response.Cookies[name].Value = value;
            }
            else
            {
                HttpCookie cookie = new HttpCookie(name);
                cookie.Value = value;
                cookie.Expires = Expires.GetValueOrDefault(DateTime.MaxValue);
                response.Cookies.Add(cookie);
            }
        }

        public static string GetCookieValue(this HttpRequestBase Request, string name)
        {
            var cookie = Request.Cookies[name];
            if (cookie == null)
                return "";
            return cookie.Value ?? "";
        }

        public static void CCookie(this HttpResponse response, string name, string value, DateTime? Expires = null)
        {
            if (response.Cookies[name] != null)
            {
                response.Cookies[name].Value = value;
            }
            else
            {
                HttpCookie cookie = new HttpCookie(name);
                cookie.Value = value;
                cookie.Expires = Expires.GetValueOrDefault(DateTime.MaxValue);
                response.Cookies.Add(cookie);
            }
        }

        public static string GetCValue(this HttpRequest Request, string name)
        {
            var cookie = Request.Cookies[name];
            if (cookie == null)
                return "";
            return cookie.Value ?? "";
        }

        
    }
}
