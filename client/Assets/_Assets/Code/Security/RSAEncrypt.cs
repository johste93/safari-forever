using System.Security.Cryptography;
using System.Text;
using System;

using UnityEngine;

public class RSAEncrypt
{
    private const int keyLength = 2048;
    private static string pathToPublicKey = "public";
    private static string pathToPrivateKey = "private";

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
                TextAsset keyTextAsset = Resources.Load<TextAsset>(pathToPublicKey);
                string xml = keyTextAsset.text;
                RSA.FromXmlString(xml);

                encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
            }
            return encryptedData;
        }
        catch (CryptographicException e)
        {
            Debug.LogError(e.Message);

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
                TextAsset keyTextAsset = Resources.Load<TextAsset>(pathToPrivateKey);
                string xml = keyTextAsset.text;
                RSA.FromXmlString(xml);

                decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
            }
            return decryptedData;
        }
        catch (CryptographicException e)
        {
            Debug.LogError(e.ToString());

            return null;
        }
    }
}
