using System.Diagnostics.Tracing;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using WebApiContrib.Tracing.Slab;

namespace WebAPIODataV4SQLite
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			var listener = new ObservableEventListener();
			listener.EnableEvents(WebApiTracing.Log, EventLevel.LogAlways, Keywords.All);
			listener.LogToConsole();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
