using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KawaiiList.Services.Security
{
    public class SecurityService : ISecurityService
    {
        // Используем DPAPI для простого шифрования
        public string Encrypt(string plainText)
        {
            byte[] entropy = Encoding.Unicode.GetBytes(Environment.MachineName);
            byte[] data = Encoding.Unicode.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string encryptedText)
        {
            try
            {
                byte[] entropy = Encoding.Unicode.GetBytes(Environment.MachineName);
                byte[] data = Convert.FromBase64String(encryptedText);
                byte[] decrypted = ProtectedData.Unprotect(data, entropy, DataProtectionScope.CurrentUser);
                return Encoding.Unicode.GetString(decrypted);
            }
            catch
            {
                return "";
            }
        }
    }
}
