using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using WebScraping.Business.Abstract;
using WebScraping.Business.Concrete;
using WebScraping.Data.Abstract;
using WebScraping.Data.Concrete.Ef;

namespace WebScraping.WebUI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IWebsiteService, WebsiteManager>();
            container.RegisterType<IWebsiteRepository, EfWebsiteRepository>();

            container.RegisterType<IWebsiteUrlService, WebsiteUrlManager>();
            container.RegisterType<IWebsiteUrlRepository, EfWebsiteUrlRepository>();

            container.RegisterType<IBookService, BookManager>();
            container.RegisterType<IBookRepository, EfBookRepository>();

            container.RegisterType<IBookNodeService, BookNodeManager>();
            container.RegisterType<IBookNodeRepository, EfBookNodeRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}