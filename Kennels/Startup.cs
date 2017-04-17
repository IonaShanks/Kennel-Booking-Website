using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Kennels.Startup))]
namespace Kennels
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
