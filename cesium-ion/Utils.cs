using System;
using System.Text;
using System.Security.Cryptography;

namespace Cesium.Ion
{
    static class Utils
    {
        public static string RandomString(int length, string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
        {
            StringBuilder res = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(alphabet[(int)(num % (uint)alphabet.Length)]);
                }
            }

            return res.ToString();
        }

        public static byte[] SHA256(this string randomString)
        {
            using (var crypt = System.Security.Cryptography.SHA256.Create())
            {
                var hash = new StringBuilder();
                return crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            }
        }

        public static string Base64Encode(this byte[] bytes, bool URLEncoded = false)
        {
            var converted = Convert.ToBase64String(bytes);
            if (URLEncoded)
            {
                return converted
                    .TrimEnd('=')
                    .Replace("+", "-")
                    .Replace("/", "_");
            }
            return converted;
        }
    }
}
