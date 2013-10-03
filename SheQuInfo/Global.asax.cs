using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using SheQuInfo.Models;
using Workorder.WebPresentationModel.Filter;

namespace SheQuInfo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            IUnityContainer container = GetContainer_Config();

            GlobalFilters.Filters.Add(new SqInfoAuthorizationFilter(container.Resolve<IUnitWork>()));
            GlobalFilters.Filters.Add(new SqInfoAuthorizationExceptionFilter());

            RegisterGlobalFilters(GlobalFilters.Filters);

            RegisterRoutes(RouteTable.Routes);
            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory(container));
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        private IUnityContainer GetContainer_Config()
        {
            IUnityContainer container = new UnityContainer();
            container.LoadConfiguration("sqlwork");
            return container;
        }
    }
}