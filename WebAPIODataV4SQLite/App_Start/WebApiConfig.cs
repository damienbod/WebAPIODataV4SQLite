using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;
using System.Web.OData.Batch;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Formatter;
using System.Web.OData.Formatter.Deserialization;
using System.Web.OData.Formatter.Serialization;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using CacheCow.Server;
using Microsoft.OData.Edm;
using Microsoft.Practices.Unity.WebApi;
using WebApiContrib.Tracing.Slab;
using WebAPIODataV4SQLite.App_Start;
using WebAPIODataV4SQLite.Controllers;
using WebAPIODataV4SQLite.DomainModel;

namespace WebAPIODataV4SQLite
{
    public static class WebApiConfig
    {
        public static HttpConfiguration Register()
        {
            var config = new HttpConfiguration();
			config.EnableSystemDiagnosticsTracing();
			config.Services.Replace(typeof(ITraceWriter), new SlabTraceWriter());
			config.Services.Add(typeof(IExceptionLogger), new SlabLoggingExceptionLogger());

            config.MapHttpAttributeRoutes();
			config.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());
            RegisterFilterProviders(config);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

	        var server = new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer);

			var cacheCowCacheHandler = new CachingHandler(config);
			config.MessageHandlers.Add(cacheCowCacheHandler);

			// Required if you want to use application/xml
			//var odataFormatters = ODataMediaTypeFormatters.Create();
			//config.Formatters.InsertRange(0, odataFormatters);

			//config.MapODataServiceRoute("odata", "odata", GetModel(), new MyODataPathHandler(), ODataRoutingConventions.CreateDefault());
            // config.MapODataServiceRoute("odata", "odata", model: GetModel());
			config.MapODataServiceRoute("odataBatch", "odata", GetModel(), new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
            return config;
        }

		class MyODataPathHandler : DefaultODataPathHandler
		{
			public override string Link(ODataPath path)
			{
				if (path.PathTemplate == "~")
				{
					return path.ToString() + "/";
				}
				return base.Link(path);
			}
		}

        public static IEdmModel GetModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.ContainerName = "SqliteContext";
            builder.EntitySet<AnimalType>("AnimalType");
            builder.EntitySet<EventData>("EventData");

            builder.EntitySet<Player>("Player");
            builder.EntityType<PlayerStats>();

            SingletonConfiguration<SkillLevels> skillLevels = builder.Singleton<SkillLevels>("SkillLevels");
            builder.EntityType<SkillLevel>();

			builder.EntitySet<AdmDto>("Adms");
            return builder.GetEdmModel();
        }

        private static void RegisterFilterProviders(HttpConfiguration config)
        {
            var providers = config.Services.GetFilterProviders().ToList();
            config.Services.Add(typeof(System.Web.Http.Filters.IFilterProvider), new WebApiUnityActionFilterProvider(UnityConfig.GetConfiguredContainer()));
            var defaultprovider = providers.First(p => p is ActionDescriptorFilterProvider);
            config.Services.Remove(typeof(System.Web.Http.Filters.IFilterProvider), defaultprovider);
        }
    }
}
