using System;
using System.Text;
using System.Security.Cryptography;

namespace Server.Services
{
    public class HashingService
    {
        private Encoding Encoding { get; set; }
        private HashAlgorithm Hasher { get; set; }

        public HashingService(HashAlgorithm hasher, Encoding encoding)
        {
            Hasher = hasher;
            Encoding = encoding;
        }

        public string GetHash(string source)
        {
            if (source == null)
                return null;

            byte[] seedBytes = Encoding.GetBytes(source);
            byte[] hash = Hasher.ComputeHash(seedBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}