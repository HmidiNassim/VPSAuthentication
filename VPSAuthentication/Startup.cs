using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using VPSAuthentication.App_Start;
using System.Web.Http;

[assembly: OwinStartup(typeof(VPSAuthentication.Startup))]

namespace VPSAuthentication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);
            //FormatterConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}
