using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Cesium.Ion.Revit
{
    public static class Utils
    {
        public static string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

        public static void OpenBrowser(this string url)
        {
            Process.Start(url);
        }

        public static byte[] SHA256(this string randomString)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            return crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
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
