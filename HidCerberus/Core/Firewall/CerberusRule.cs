using System.Collections.Generic;
using HidCerberus.Core.Firewall.ProcessIdentifiers;
using HidCerberus.Util;

namespace HidCerberus.Core.Firewall
{
    public class CerberusRule
    {
        private string _hardwareId;

        public CerberusRule()
        {
            ProcessIdentifiers = new List<IProcessIdentifier>();
        }

        public string CerberusRuleId { get; private set; }

        public string HardwareId
        {
            get => _hardwareId;
            set
            {
                _hardwareId = value.ToUpper();
                CerberusRuleId = _hardwareId.ToSha256();
            }
        }

        public bool IsAllowed { get; set; }

        public bool IsPermanent { get; set; }

        public IList<IProcessIdentifier> ProcessIdentifiers { get; set; }
    }
}