using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheWritersNet.Startup))]
namespace TheWritersNet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
