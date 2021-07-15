using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using System.Data.Entity;
using WebScraping.Data.Concrete.Ef;
using System.Timers;
using WebScraping.WebUI.Controllers;

namespace WebScraping.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            
            var controller = DependencyResolver.Current.GetService<HomeController>();

            Timer timer = new Timer(TimeSpan.FromMinutes(60).TotalMilliseconds);
            timer.AutoReset = true;
            timer.Elapsed += controller.CallBookMethod;
            timer.Start();

        }
    }
}
