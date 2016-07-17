using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mvc5.DocDB.Startup))]
namespace Mvc5.DocDB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
