using System;
using System.Collections.Generic;
using System.Linq;

namespace HidCerberus.Srv.Util
{
    public static class ByteArrayExtensions
    {
        public static byte[] SeparateAndGetLast(this byte[] source, byte[] separator)
        {
            for (var i = 0; i < source.Length; ++i)
                if (Equals(source, separator, i))
                {
                    var index = i + separator.Length;
                    var part = new byte[source.Length - index];
                    Array.Copy(source, index, part, 0, part.Length);
                    return part;
                }
            throw new Exception("not found");
        }

        public static IEnumerable<byte[]> Separate(this byte[] source, byte[] separator)
        {
            var parts = new List<byte[]>();
            var index = 0;
            byte[] part;
            for (var I = 0; I < source.Length; ++I)
                if (Equals(source, separator, I))
                {
                    part = new byte[I - index];
                    Array.Copy(source, index, part, 0, part.Length);
                    parts.Add(part);
                    index = I + separator.Length;
                    I += separator.Length - 1;
                }
            part = new byte[source.Length - index];
            Array.Copy(source, index, part, 0, part.Length);
            parts.Add(part);
            return parts.ToArray();
        }

        private static bool Equals(IReadOnlyList<byte> source, IEnumerable<byte> separator, int index)
        {
            return !separator.Where((t, i) => index + i >= source.Count || source[index + i] != t).Any();
        }
    }
}