using System.Security.Cryptography;
using System.Text;
using Budget.Users.Domain.Services;

namespace Budget.Users.Cryptography.Services
{
    public class Sha512CryptService : ICryptService
    {
        public Sha512CryptService()
        {
            CryptAlgorithm = new SHA512Managed();
        }

        private SHA512Managed CryptAlgorithm { get; }

        public string Crypt(string content)
        {
            byte[] contentBytes = Encoding.Default.GetBytes(content);
            byte[] cryptedBytes = CryptAlgorithm.ComputeHash(contentBytes);
            
            return Encoding.Default.GetString(cryptedBytes);
        }
    }
}
