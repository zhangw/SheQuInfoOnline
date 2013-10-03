using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Web.Caching;
using System.Web.Security;
using Wedo.Mvc.Utility;

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class NavigationAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            Navigation nav = Navigator.GetNav(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
            if (nav != null)
                Roles = nav.Roles;
            base.OnAuthorization(filterContext);
        }
    }

    public class Navigation
    {
        public Navigation()
        {
            ID = Guid.NewGuid();
        }
        public Guid ID { get; set; }
        /// <summary>
        /// 控制器的名称
        /// </summary>
        [XmlAttribute]
        public string Controller { get; set; }

        /// <summary>
        /// Action名称
        /// </summary>
        [XmlAttribute]
        public string Action { get; set; }

        /// <summary>
        /// 适用角色 多个角色用|分割,如|1|管理员|
        /// </summary>
        [XmlAttribute]
        public string Roles { get; set; }

        /// <summary>
        /// 是否为导航
        /// </summary>
        [XmlAttribute]
        public bool IsNav { get; set; }

        /// <summary>
        /// 模块的名称，一级菜单
        /// </summary>
        [XmlAttribute]
        public string Module { get; set; }

        /// <summary>
        /// 页面的名称
        /// </summary>
        [XmlAttribute]
        public string Page { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [XmlAttribute]
        public int Order { get; set; }

        /// <summary>
        /// Controller描述
        /// </summary>
        [XmlAttribute]
        public string ControllerDesc { get; set; }

        /// <summary>
        /// Action描述
        /// </summary>
        [XmlAttribute]
        public string ActionDesc { get; set; }
    }
    public class Navigations
    {
        [XmlElement("Navigation")]
        public Navigation[] Navigation { get; set; }
    }

    public interface INavigator
    {
        void Resole();
        bool HasPermission(string controller, string actionname);
        Navigation GetNav(string controller, string actionname);
        IEnumerable<Navigation> GetCurrentNav(CurrentUser user);
        IEnumerable<Navigation> GetAllNav();
    }
    public class DefaultXmlNavigator : INavigator
    {
        public void Resole()
        {
            XmlSerializer xsr = new XmlSerializer(typeof(Navigations));

            XmlTextReader reader = new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory + "/App_Data/Navigation.xml");
            Navigations dd = xsr.Deserialize(reader) as Navigations;
            reader.Close();
            HttpRuntime.Cache.Add("Navigator", dd.Navigation,
                new CacheDependency(AppDomain.CurrentDomain.BaseDirectory + "/App_Data/Navigation.xml"),
                Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0, 0), CacheItemPriority.NotRemovable,
                (s, o, r) =>
                {
                    if (r == CacheItemRemovedReason.DependencyChanged)
                        Resole();
                }
                );
        }
        public bool HasPermission(string controller, string actionname)
        {
            //4 Test
            string[] roles = { "SuperAdmin" };
            Navigation nav = GetNav(controller, actionname);
            return nav != null && roles.Any(r => string.IsNullOrEmpty(nav.Roles) || nav.Roles.Contains(r));
        }
        public Navigation GetNav(string controller, string actionname)
        {
            IEnumerable<Navigation> navs = HttpContext.Current.Cache["Navigator"] as IEnumerable<Navigation>;
            return navs.FirstOrDefault(s => s.Controller == controller && s.Action == actionname);
        }
        public IEnumerable<Navigation> GetCurrentNav(CurrentUser user)
        {
            return GetNavs(user);
        }
        public IEnumerable<Navigation> GetAllNav()
        {
            return GetNavs(true);

        }
        private IEnumerable<Navigation> GetNavs(bool isAll)
        {
            IEnumerable<Navigation> navs = HttpContext.Current.Cache["Navigator"] as IEnumerable<Navigation>;
            if (!isAll)
            {
                string[] roles = { "1" };//Admin
                navs = navs.Where(s => s.IsNav && roles.Any(r => string.IsNullOrEmpty(s.Roles) || s.Roles.Contains(r)));
            }
            return navs;
        }

        private IEnumerable<Navigation> GetNavs(CurrentUser user)
        {
            IEnumerable<Navigation> navs = HttpContext.Current.Cache["Navigator"] as IEnumerable<Navigation>;
            if (navs == null)
            {
                Resole();
                navs = HttpContext.Current.Cache["Navigator"] as IEnumerable<Navigation>;
            }
            if (user.Roles!=null &&user.Roles.Length>0)
            {
                navs = navs.Where(s => s.IsNav && user.Roles.Any(r => string.IsNullOrEmpty(s.Roles) || s.Roles.Contains("|" + r + "|")));
            }
            return navs;

        }
    }

    public static class Navigator
    {
        //declare INavigator interface
        private static INavigator currentNavigator = new DefaultXmlNavigator();
        /// <summary>
        /// 
        /// </summary>
        public static INavigator GetNavigator
        {
            get
            {
                return currentNavigator;
            }
            private set
            {
                if (value != null)
                {
                    currentNavigator = value;
                }
            }
        }
        /// <summary>
        /// register navigator
        /// </summary>
        /// <param name="nav"></param>
        public static void RegisterNavigator(INavigator nav)
        {
            GetNavigator = nav;
        }
        public static void Resole()
        {
            currentNavigator.Resole();
        }
        public static bool HasPermission(string controller, string actionname)
        {
            return currentNavigator.HasPermission(controller, actionname);
        }
        public static Navigation GetNav(string controller, string actionname)
        {
            return currentNavigator.GetNav(controller, actionname);
        }
        public static IEnumerable<Navigation> GetCurrentNav(CurrentUser user)
        {
            return currentNavigator.GetCurrentNav(user);
        }
        public static IEnumerable<Navigation> GetAllNav()
        {
            return currentNavigator.GetAllNav();
        }
    }
}