using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HidCerberus.Util
{
    public static class StringExtensions
    {
        public static string ToSha256(this string value)
        {
            using (var hash = SHA256.Create())
            {
                return string.Concat(hash
                    .ComputeHash(Encoding.UTF8.GetBytes(value))
                    .Select(item => item.ToString("x2")));
            }
        }
    }
}