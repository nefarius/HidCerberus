using System;
using Microsoft.Win32;

namespace HidCerberus.Util
{
    public static class RegistryKeyExtensions
    {
        public static void SetNonNullValue<T>(this RegistryKey key, string name, T value)
        {
            if (value != null) key.SetValue(name, value);
        }

        public static void SetNonNullValueDword(this RegistryKey key, string name, object value)
        {
            if (value != null) key.SetNonNullValue(name, Convert.ToInt32(value));
        }
    }
}