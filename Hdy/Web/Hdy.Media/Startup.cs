using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Framework.Auth;

[assembly: OwinStartup(typeof(Hdy.Media.Startup))]

namespace Hdy.Media
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
