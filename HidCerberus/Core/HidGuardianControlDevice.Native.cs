using System;
using System.Runtime.InteropServices;
using System.Security;

namespace HidCerberus.Core
{
    [SuppressUnmanagedCodeSecurity]
    public sealed partial class HidGuardianControlDevice
    {
        private const uint IoctlHidguardianGetCreateRequest = 0x8000E004;
        private const uint IoctlHidguardianSetCreateRequest = 0x8000A008;

        private const int HardwareIdsArraySize = 512;

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
        internal struct HidGuardianGetCreateRequest
        {
            public UInt32 Size;
            public UInt32 RequestId;
            public UInt32 ProcessId;
            public UInt32 DeviceIndex;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HardwareIdsArraySize)]
            public byte[] HardwareIds;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct HidGuardianSetCreateRequest
        {
            public UInt32 RequestId;
            public UInt32 DeviceIndex;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsAllowed;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsSticky;
        }
    }
}
