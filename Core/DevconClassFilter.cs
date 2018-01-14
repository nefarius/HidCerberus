using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace HidCerberus.Srv.Core
{
    public partial class DevconClassFilter
    {
        public DevconClassFilter()
        {
            var guid = ClassGuidFromName(XnaCompositeClassName);

            var key = SetupDiOpenClassRegKeyEx(ref guid, REGASM.KEY_READ, DIOCR_INSTALLER, Environment.MachineName,
                IntPtr.Zero);

            var k = RegistryKey.FromHandle(new SafeRegistryHandle(key, true));

            var values = k.GetValue("UpperFilters");
        }

        public static string HidClassName => "HIDClass";

        public static string XnaCompositeClassName => "XnaComposite";

        private static Guid ClassGuidFromName(string name)
        {
            var bufferSize = Marshal.SizeOf(typeof(Guid));
            var guidBuffer = Marshal.AllocHGlobal(bufferSize);

            try
            {
                var ret = SetupDiClassGuidsFromNameEx(XnaCompositeClassName, guidBuffer, 1, out var _,
                    Environment.MachineName,
                    IntPtr.Zero);

                if (!ret)
                    return Guid.Empty;

                var buffer = new byte[bufferSize];
                Marshal.Copy(guidBuffer, buffer, 0, buffer.Length);

                return new Guid(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(guidBuffer);
            }
        }
    }
}