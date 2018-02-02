using System.Collections.Generic;
using HidCerberus.Core.Firewall.ProcessIdentifiers;

namespace HidCerberus.Core.Firewall
{
    public class CerberusRule
    {
        public string HardwareId { get; set; }

        public bool IsAllowed { get; set; }

        public bool IsPermanent { get; set; }

        public IEnumerable<IProcessIdentifier> ProcessIdentifiers { get; set; }
    }
}