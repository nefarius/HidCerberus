using HidCerberus.Core;
using Nancy;
using Serilog;
using Topshelf;
using DevconClassFilter = HidCerberus.Core.Devcon.DevconClassFilter;

namespace HidCerberus.Srv
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.RollingFile("Logs\\HidCerberus-{Date}.log")
                .CreateLogger();

            var t = new DevconClassFilter();

            HostFactory.Run(x =>
            {
                StaticConfiguration.DisableErrorTraces = false;

                x.Service<HidCerberusService>(s =>
                {
                    s.ConstructUsing(name => new HidCerberusService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("HidCerberus Configuration Host for HidGuardian Filter Drivers");
                x.SetDisplayName("HidCerberus Service");
                x.SetServiceName("HidCerberus");
            });
        }
    }
}
