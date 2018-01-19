using System;
using System.Diagnostics;
using HidCerberus.Srv.Properties;
using Nancy.Hosting.Self;
using Serilog;

namespace HidCerberus.Srv.Core
{
    public class HidCerberusService
    {
        private NancyHost _nancyHost;
        private HidGuardianControlDevice _hgControl;

        public void Start()
        {
            Log.Information("Service starting");

#if BLAH
            _hgControl = new HidGuardianControlDevice();

            _hgControl.OpenPermissionRequested += (sender, eventArgs) =>
            {
                var pid = eventArgs.ProcessId;
                var proc = Process.GetProcessById(pid);

                Log.Information("Open request received from {PID}: {Name} ({Path})", 
                    pid, proc.ProcessName, proc.MainModule.FileName);

                foreach (var id in eventArgs.HardwareIds)
                {
                    Log.Information("Hardware ID: {HardwareId}", id);
                }

                Log.Information("For the sake of demonstration we will permanently allow requests and log details");

                eventArgs.IsAllowed = true;
                eventArgs.IsPermanent = true;
            };
#endif

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
