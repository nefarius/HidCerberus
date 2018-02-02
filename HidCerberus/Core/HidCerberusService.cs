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

            var r = new CerberusRule()
            {
                HardwareId = @"HID_DEVICE",
                IsAllowed = true,
                IsPermanent = true,
                ProcessIdentifiers = {new ProcessByImageNameIdentifier {ImageName = "rundll32.exe"}}
            };

            arules.Upsert(r);

            Log.Information("CerberusRuleId: {Id}", r.CerberusRuleId);

            Log.Warning("Entry: {id}", arules.FindById("63d85cdf03374acf10e019eecedd4f56adc266ef5c10dba3ae8aba1e0fcddf0d"));

            _hgControl = new HidGuardianControlDevice();

            _hgControl.OpenPermissionRequested += (sender, args) =>
            {
                var pid = args.ProcessId;

                Log.Information("Open request received from {PID}", pid);
                
                var rules = CerberusDatabase.Instance.GetCollection<CerberusRule>();

                foreach (var hardwareId in args.HardwareIds)
                {
                    Log.Information("Looking up rule for Hardware ID: {HardwareId}", hardwareId);

                    Log.Information("CerberusRuleId: {Id}", hardwareId.ToUpper().ToSha256());

                    var rule = rules.FindById(hardwareId.ToUpper().ToSha256());

                    if (rule == null)
                    {
                        Log.Warning("Rule not found in DB");
                        continue;
                    }

                    Log.Information("Found rule for Hardware ID: {HardwareId}", hardwareId);

                    foreach (var identifier in rule.ProcessIdentifiers)
                    {
                        if (!identifier.IdentifyByPid(pid)) continue;

                        Log.Information("Found process identifier [Identifier}", identifier);

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
