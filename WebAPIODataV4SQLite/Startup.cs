using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebAPIODataV4SQLite.Startup))]

namespace WebAPIODataV4SQLite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			// Using the old web server hosting because the OData batch routing does not work inside OWIN at present.
            //app.UseWebApi(WebApiConfig.Register());
        }
    }
}
