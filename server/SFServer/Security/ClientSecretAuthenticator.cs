using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace SFServer.Security
{
    public class ClientSecretAuthenticator
    {
        private string salt;
        public ClientSecretAuthenticator()
        {
            salt = Startup.Configuration["CLIENT_SECRET_SALT"];
        }

        public bool ValidateSecret(string clientSecret, string clientId)
        {
            byte[] decryptedBuffer = RSAEncrypt.Decrypt(Convert.FromBase64String(clientSecret), false);
            if (decryptedBuffer == null)
            {
                Console.WriteLine("clientSecret decryption failed");
                return false;
            }
               

            string decryptedSecret = Encoding.Unicode.GetString(decryptedBuffer);

            string[] segments = decryptedSecret.Split(":");

            if (segments.Length != 7)
            {
                Console.WriteLine("Not enough segments");
                return false;
            }
               
            //Check if clientId matches secretClientId
            if (segments[0] != clientId)
            {
                Console.WriteLine(segments[0] + " != " + clientId);
                Console.WriteLine("ClientId mismatch");
                return false;
            }
               
            if (segments[1] != salt)
            {
                Console.WriteLine("Salt mismatch");
                return false;
            }
            

            bool timestampValid = 
                    segments[2] == DateTimeOffset.Now.ToUniversalTime().Year.ToString() &&
                    segments[3] == DateTimeOffset.Now.ToUniversalTime().Month.ToString() &&
                    segments[4] == DateTimeOffset.Now.ToUniversalTime().Day.ToString() &&
                    segments[5] == DateTimeOffset.Now.ToUniversalTime().Hour.ToString() &&
                    (segments[6] == DateTimeOffset.Now.ToUniversalTime().Minute.ToString() || segments[6] == DateTimeOffset.Now.ToUniversalTime().AddMinutes(1).Minute.ToString() || segments[6] == DateTimeOffset.Now.ToUniversalTime().AddMinutes(2).Minute.ToString() || segments[6] == DateTimeOffset.Now.ToUniversalTime().AddMinutes(-1).Minute.ToString());

            if(!timestampValid)
            {
                string expectedResult = clientId + ":" + salt + ":" + DateTimeOffset.Now.ToUniversalTime().Year.ToString() + ":" + DateTimeOffset.Now.ToUniversalTime().Month.ToString() + ":" + DateTimeOffset.Now.ToUniversalTime().Day.ToString() + ":" + DateTimeOffset.Now.ToUniversalTime().Hour.ToString() + ":" + DateTimeOffset.Now.ToUniversalTime().Minute.ToString();
                //Console.WriteLine("clientSecret: " + decryptedSecret);
                //Console.WriteLine("expectedSecret: " + expectedResult);
                //Console.WriteLine("Timestamp not valid");
            }
                

            return timestampValid;
        }
    }
}
