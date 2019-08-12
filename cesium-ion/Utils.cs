using System;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Threading.Tasks;

namespace Cesium.Ion
{
    static class Utils
    {
        public static string RandomString(int Length, string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
        {
            StringBuilder res = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (Length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(Alphabet[(int)(num % (uint)Alphabet.Length)]);
                }
            }

            return res.ToString();
        }

        public static byte[] SHA256(this string RandomString)
        {
            using (var crypt = System.Security.Cryptography.SHA256.Create())
            {
                var hash = new StringBuilder();
                return crypt.ComputeHash(Encoding.UTF8.GetBytes(RandomString));
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

       public static async Task<T> WithSecurityProtocol<T>(this Task<T> task, SecurityProtocolType Protocol)
        {
            var oldProtocol = ServicePointManager.SecurityProtocol;
            ServicePointManager.SecurityProtocol = Protocol;
            try
            {
                return await task.ConfigureAwait(false); 
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }


    }
}
