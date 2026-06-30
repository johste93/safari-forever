using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SFServer.Utility
{
    public class RandomTokenGenerator
    {
        public static string Generate()
        {
            // generate a 1024-bit salt using a secure PRNG
            byte[] salt = new byte[1024 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }
    }
}
