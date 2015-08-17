using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LoLUniverse.Startup))]
namespace LoLUniverse
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
