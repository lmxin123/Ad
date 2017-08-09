using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Framework.Auth.Test.Startup))]
namespace Framework.Auth.Test
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            StartupAuth auth = new StartupAuth();
            auth.ConfigureAuth(app);
        }
    }
}
