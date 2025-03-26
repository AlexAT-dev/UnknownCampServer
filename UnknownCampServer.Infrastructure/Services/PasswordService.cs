using System;
using System.Security.Cryptography;
using UnknownCampServer.Core.Services;

namespace UnknownCampServer.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        public byte[] GenerateSalt()
        {
            var salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public string HashPasswordPBKDF2(string password, byte[] salt)
        {
            const int iterations = 10000; 
            const int hashLength = 32;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] hash = pbkdf2.GetBytes(hashLength);
                byte[] hashBytes = new byte[salt.Length + hash.Length];

                Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);

                return Convert.ToBase64String(hashBytes); 
            }
        }

        public bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            var hashBytes = Convert.FromBase64String(storedPasswordHash);
            var salt = new byte[16]; 
            Buffer.BlockCopy(hashBytes, 0, salt, 0, salt.Length);

            var storedHash = new byte[hashBytes.Length - salt.Length];
            Buffer.BlockCopy(hashBytes, salt.Length, storedHash, 0, storedHash.Length);

            var enteredPasswordHash = HashPasswordPBKDF2(enteredPassword, salt);

            return enteredPasswordHash == Convert.ToBase64String(hashBytes);
        }
    }
}
