using HidCerberus.Srv.Properties;
using Nancy.Hosting.Self;

namespace HidCerberus.Srv.Core
{
    public class HidCerberusService
    {
        private NancyHost _nancyHost;
        private HidGuardianControlDevice _hgControl;

        public void Start()
        {
            _hgControl = new HidGuardianControlDevice();

            _hgControl.OpenPermissionRequested += (sender, eventArgs) => eventArgs.IsAllowed = true;

            _nancyHost = new NancyHost(Settings.Default.ServiceUrl);
            _nancyHost.Start();
        }

        public void Stop()
        {
            _hgControl.Dispose();
            _nancyHost.Stop();
        }
    }
}
