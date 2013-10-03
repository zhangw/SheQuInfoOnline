using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Security.Principal; 
using System.Reflection;
using System.Web.Mvc;
using System.Web;
namespace Wedo.Mvc.Utility
{
    //[Serializable]
    //public class CurrentUser : IIdentity
    //{
    //    public CurrentUser()
    //        : this(null, null, null, null, null, null)
    //    {
    //    }
    //    public CurrentUser(string ADAccount, string[] roles)
    //        : this(null, null, ADAccount, null, null, null)
    //    {
    //    }
    //    public CurrentUser(string name, string displayName, string ADAccount, string Department)
    //        : this(name, displayName, ADAccount, Department, null, null)
    //    {
    //    }
    //    public CurrentUser(string name, string displayName, string ADAccount, string Department, string[] roles)
    //        : this(name, displayName, ADAccount, Department, roles, null)
    //    {
    //    }
    //    public CurrentUser(string name, string displayName, string ADAccount, string Department, string[] roles, string email)
    //    {
    //        this.Name = name;
    //        this.DisplayName = displayName;
    //        this.ADAccount = ADAccount;
    //        this.Roles = roles;
    //        this.Department = Department;
    //        this.Email = email;
    //    }
    //    public string Name { get; private set; }

    //    public string AuthenticationType
    //    {
    //        get { return "Windows"; }
    //    }

    //    public bool IsAuthenticated
    //    {
    //        get { return true; }
    //    }
    //    public string DisplayName { get; private set; }
    //    public string[] Roles { get; private set; }
    //    public string ADAccount { get; private set; }
    //    public string Email { get; private set; }
    //    public string Department { get; private set; }


    //}

    public class CurrentUser : IIdentity
    {

        public CurrentUser() : this(null, null, null, null,  null)
        {
        }
        public CurrentUser(string name, string[] roles)
            : this(name, null, null, roles, null)
        {
        }
        public CurrentUser(string name, string displayName, string Department)
            : this(name, displayName, Department, null, null)
        {
        }
        public CurrentUser(string name, string displayName,  string Department, string[] roles)
            : this(name, displayName,  Department, roles, null)
        {
        }

        public CurrentUser(string name, string displayName,   string Department, string[] roles, string email)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.Roles = roles;
            this.Department = Department;
            this.Email = email;
        }

        public string DisplayName { get; private set; }
        public string[] Roles { get; private set; }
        public string Email { get; private set; }
        public string Department { get; private set; }
        public string Name { get; private set; }


        public string AuthenticationType
        {
            get { return "Windows"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }
    }

    public static class SecurityExtensions
    {
        public static string Name(this IPrincipal user)
        {
            return user.Identity.Name;
        }

        public static bool InAnyRole(this IPrincipal user, params string[] roles)
        {
            foreach (string role in roles)
            {
                if (user.IsInRole(role)) return true;
            }
            return false;
        }

        public static CurrentUser GetCurrentUserInfo(this IPrincipal principal)
        {
            if (principal.Identity is CurrentUser)
            {
                return (CurrentUser)principal.Identity;
            }
            else
                return null;
        }
    }

    public static class CookieSecurity
    {
        private static readonly string prefix = "P_";
        public static CurrentUser GetUser(this HttpRequest Request)
        {
            CurrentUser user = null;
            if (Request.Cookies[prefix + "Name"] != null && Request.Cookies[prefix + "Name"].Value.IsNotNullOrWhiteSpace())
            {
                var roles = Request.GetCValue(prefix + "Roles").Split('|');
                user = new CurrentUser(
                     Request.GetCValue(prefix + "Name"), Request.GetCValue(prefix + "DisplayName"),
                     Request.GetCValue(prefix + "Department"),
                     roles,
                     Request.GetCValue(prefix + "Email"));
            }
            return user;
        }

        public static void CreateUserCookie(this HttpResponse Response,HttpRequest Request, CurrentUser user)
        {
            if (user != null)
            {
                if (IsLogoned(Request, user) == false)
                {
                    Response.CCookie(prefix + "Department", user.Department, DateTime.Now.AddHours(1));
                    Response.CCookie(prefix + "DisplayName", user.DisplayName, DateTime.Now.AddHours(1));
                    Response.CCookie(prefix + "Email", user.Email, DateTime.Now.AddHours(1));
                    Response.CCookie(prefix + "Roles",  string.Join("|", user.Roles??new string[]{}), DateTime.Now.AddHours(1));
                    Response.CCookie(prefix + "Name", user.Name, DateTime.Now.AddHours(1));
                }
            }
        }

        public static bool IsLogoned(this HttpRequest Request, CurrentUser user)
        {
            if (user == null) return false;
            return Request.Cookies[prefix + "Name"] != null 
                &&Request.Cookies[prefix + "Name"].Value.IsNotNullOrWhiteSpace() 
                && Request.Cookies[prefix + "Name"].Value == user.Name;
        }
    }

    public class ValidateRole : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CurrentUser user = filterContext.HttpContext.User.GetCurrentUserInfo();
            if (user.Roles == null || user.Roles.Count() == 0)
            {
                ViewResult viewResult = new ViewResult();
                viewResult.ViewName = "Error401";
                filterContext.Result = viewResult;
                //new HttpStatusCodeResult(401);
            }
            //throw new NotImplementedException();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //  throw new NotImplementedException();
        }
    }
}
