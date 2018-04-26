using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BatBreaker2.Startup))]
namespace BatBreaker2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
