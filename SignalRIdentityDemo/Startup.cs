using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SignalRIdentityDemo.Startup))]
namespace SignalRIdentityDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
