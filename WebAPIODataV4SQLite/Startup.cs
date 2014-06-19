using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebAPIODataV4SQLite.Startup))]

namespace WebAPIODataV4SQLite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWebApi(WebApiConfig.Register());
        }
    }
}
