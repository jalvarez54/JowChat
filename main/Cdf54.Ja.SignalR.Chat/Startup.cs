using Owin;

namespace Cdf54.Ja.SignalR.Chat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            System.Diagnostics.Trace.TraceInformation("Startup: Method={0} ", "Configuration()");

            ConfigureAuth(app);
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}
