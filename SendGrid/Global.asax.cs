using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MongoDB.Driver;
using StructureMap;

namespace SendGrid
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Error += WebApiApplication_Error;
        }

        void WebApiApplication_Error(object sender, System.EventArgs e)
        {
            var db = ObjectFactory.GetInstance<MongoCollection<Models.Error>>();
            var exception = Server.GetLastError();
            var error = new Models.Error(exception);
        }

    }
}