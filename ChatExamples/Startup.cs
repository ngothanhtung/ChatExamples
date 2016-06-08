using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ChatExamples.Startup))]
namespace ChatExamples
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigurationSignalR(app);
        }
    }
}
