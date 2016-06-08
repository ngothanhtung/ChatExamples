using System;
using ChatExamples.Components;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ChatExamples.Startup))]

namespace ChatExamples
{
    public partial class Startup
    {
        public void ConfigurationSignalR(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();

            GlobalHost.HubPipeline.AddModule(new RejoingGroupPipelineModule());
        }
    }
}
