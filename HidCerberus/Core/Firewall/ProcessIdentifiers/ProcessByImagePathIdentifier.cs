using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace HidCerberus.Core.Firewall.ProcessIdentifiers
{
    [UsedImplicitly]
    public class ProcessByImagePathIdentifier : IProcessIdentifier
    {
        public string ImagePath { get; set; }

        public bool IdentifyByPid(int pid)
        {
            return ImagePath.Equals(Process.GetProcessById(pid).MainModule.FileName,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}