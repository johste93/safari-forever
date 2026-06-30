using System.Security.Cryptography;
using System.Text;
using System;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace SFServer.Security
{
    public class RSAEncrypt
    {
        private const int keyLength = 2048;

        private static string rsa_pk;
        public RSAEncrypt()
        {
            rsa_pk = Startup.Configuration["RSA_PRIVATE_KEY"];
        }

        public static byte[] Encrypt(string DataToEncrypt, bool DoOAEPPadding)
        {
            byte[] dataToEncrypt = new UnicodeEncoding().GetBytes(DataToEncrypt);
            return Encrypt(dataToEncrypt, DoOAEPPadding);
        }

        public static string Decrypt(string DataToDecrypt, bool DoOAEPPadding)
        {
            byte[] dataToDecrypt = Encoding.UTF8.GetBytes(DataToDecrypt);
            byte[] decryptedData = Decrypt(dataToDecrypt, DoOAEPPadding);
            return new UnicodeEncoding().GetString(decryptedData);
        }

        public static byte[] Encrypt(byte[] DataToEncrypt, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(keyLength))
                {
                    RSAParameters parameters = GetParametersFromXmlAsync();
                    RSA.ImportParameters(parameters);

                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
        }

        public static byte[] Decrypt(byte[] DataToDecrypt, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(keyLength))
                {
                    RSAParameters parameters = GetParametersFromXmlAsync();
                    RSA.ImportParameters(parameters);

                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
        }

        public static RSAParameters GetParametersFromXmlAsync()
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(rsa_pk);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Exponent": parameters.Exponent = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "P": parameters.P = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Q": parameters.Q = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DP": parameters.DP = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DQ": parameters.DQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "InverseQ": parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "D": parameters.D = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            return parameters;
        }
    }
}
