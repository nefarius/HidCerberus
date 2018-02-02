using System.Diagnostics;
using System.Linq;
using HidCerberus.Core.Database;
using HidCerberus.Core.Firewall;
using HidCerberus.Core.Firewall.ProcessIdentifiers;
using HidCerberus.Srv.Properties;
using HidCerberus.Util;
using Nancy.Hosting.Self;
using Serilog;

namespace HidCerberus.Core
{
    public class HidCerberusService
    {
        private NancyHost _nancyHost;
        private HidGuardianControlDevice _hgControl;

        public void Start()
        {
            Log.Information("Service starting");

            var arules = CerberusDatabase.Instance.GetCollection<CerberusRule>();

            var all = arules.FindById("f7d6d319790927f44a8a762e2bb92c8f45acdf35eb1b72a0ccd740bca8930ab0");

            arules.Update(new CerberusRule()
            {
                HardwareId = @"HID\VID_054C&PID_05C4",
                IsAllowed = false,
                IsPermanent = false,
                ProcessIdentifiers = { new ProcessByImageNameIdentifier { ImageName = "rundll32.exe" } }
            });

            _hgControl = new HidGuardianControlDevice();

            _hgControl.OpenPermissionRequested += (sender, args) =>
            {
                var pid = args.ProcessId;

                var rules = CerberusDatabase.Instance.GetCollection<CerberusRule>();

                foreach (var hardwareId in args.HardwareIds)
                {
                    var rule = rules.FindById(hardwareId.ToSha256());

                    if (rule == null)
                        continue;

                    foreach (var identifier in rule.ProcessIdentifiers)
                    {
                        if (!identifier.IdentifyByPid(pid)) continue;

                        args.IsAllowed = rule.IsAllowed;
                        args.IsPermanent = rule.IsPermanent;
                    }
                }
            };


            _nancyHost = new NancyHost(Settings.Default.ServiceUrl);
            _nancyHost.Start();

        }

        public void Stop()
        {
            Log.Information("Service stopping");

            _hgControl.Dispose();
            _nancyHost.Stop();
        }
    }
}
