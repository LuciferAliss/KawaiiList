using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KawaiiList.Services.Security
{
    public interface ISecurityService
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }
}
