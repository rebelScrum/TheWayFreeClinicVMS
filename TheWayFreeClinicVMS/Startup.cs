using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheWayFreeClinicVMS.Startup))]
namespace TheWayFreeClinicVMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
