using System;
using System.Runtime.InteropServices;
using System.Security;

namespace HidCerberus.Srv.Core
{
    [SuppressUnmanagedCodeSecurity]
    public partial class HidGuardianControlDevice
    {
        private const uint IoctlHidguardianGetCreateRequest = 0x8000E004;
        private const uint IoctlHidguardianSetCreateRequest = 0x8000A008;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct HidGuardianGetCreateRequest
        {
            public UInt32 RequestId;
            public UInt32 ProcessId;
            public UInt32 DeviceIndex;
            public IntPtr HardwareIdBuffer;
            public UInt32 HardwareIdBufferLength;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct HidGuardianSetCreateRequest
        {
            public UInt32 RequestId;
            public UInt32 DeviceIndex;
            public bool IsAllowed;
        }
    }
}
