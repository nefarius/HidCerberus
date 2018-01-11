using HidCerberus.Srv.Core;
using HidCerberus.Srv.Properties;
using Nancy.Hosting.Self;

namespace HidCerberus.Srv.NancyFx
{
    public class NancySelfHost
    {
        private NancyHost _nancyHost;

        public void Start()
        {
            var hc = new HidGuardianControlDevice();

            hc.OpenPermissionRequested += (sender, eventArgs) => eventArgs.IsAllowed = true;

            _nancyHost = new NancyHost(Settings.Default.ServiceUrl);
            _nancyHost.Start();
        }

        public void Stop()
        {
            _nancyHost.Stop();
        }
    }
}
