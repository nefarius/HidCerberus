using HidCerberus.Srv.NancyFx;
using Nancy;
using Serilog;
using Topshelf;

namespace HidCerberus.Srv
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.RollingFile("Logs\\HidCerberus.Srv-{Date}.log")
                .CreateLogger();

            HostFactory.Run(x =>
            {
                StaticConfiguration.DisableErrorTraces = false;

                x.Service<NancySelfHost>(s =>
                {
                    s.ConstructUsing(name => new NancySelfHost());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("HidCerberus Configuration Host for HidGuardian Filter Drivers");
                x.SetDisplayName("HidCerberus Service");
                x.SetServiceName("HidCerberus.Srv");
            });
        }
    }
}
