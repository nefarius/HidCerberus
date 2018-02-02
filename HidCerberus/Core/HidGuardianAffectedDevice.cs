using System;
using HidCerberus.Util;

namespace HidCerberus.Core
{
    public class HidGuardianAffectedDevice
    {
        private string _hardwareId;

        public string EntityId { get; set; }

        public string HardwareId
        {
            get => _hardwareId;
            set
            {
                _hardwareId = value;
                EntityId = value.ToSha256();
            }
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HidGuardianAffectedDevice)obj);
        }

        protected bool Equals(HidGuardianAffectedDevice other)
        {
            return string.Equals(_hardwareId, other._hardwareId, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(_hardwareId);
        }

        public static bool operator ==(HidGuardianAffectedDevice left, HidGuardianAffectedDevice right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HidGuardianAffectedDevice left, HidGuardianAffectedDevice right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}