using System;
using HidCerberus.Srv.Core;
using HidCerberus.Srv.NancyFx;
using Nancy;
using Topshelf;

namespace HidCerberus.Srv
{
    class Program
    {
        static void Main(string[] args)
        {
            var hc = new HidGuardianControlDevice();

            hc.OpenPermissionRequested += (sender, eventArgs) => eventArgs.IsAllowed = true;

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
