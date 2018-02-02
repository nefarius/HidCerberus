using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace HidCerberus.Core.Firewall.ProcessIdentifiers
{
    [UsedImplicitly]
    public class ProcessByImageNameIdentifier : IProcessIdentifier
    {
        public string ImageName { get; set; }

        public bool IdentifyByPid(int pid)
        {
            return (Path.GetFileNameWithoutExtension(ImageName) ?? ImageName).Equals(
                Process.GetProcessById(pid).ProcessName,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}