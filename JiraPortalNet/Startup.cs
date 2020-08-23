using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JiraPortal.Startup))]
namespace JiraPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
