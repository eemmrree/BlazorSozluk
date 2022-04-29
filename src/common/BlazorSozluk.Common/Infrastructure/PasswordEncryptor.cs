using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Common.Infrastructure
{
    public class PasswordEncryptor
    {
        public static string Encryp(string password)
        {
            using var md5 = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(password);
            byte[] result = md5.ComputeHash(bytes);

            return Convert.ToHexString(result);
        }
    }
}
