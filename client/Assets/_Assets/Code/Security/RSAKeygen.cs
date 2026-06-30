using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public class RSAKeygen
{
    public static void GenerateNewKeys(int keyLength)
    {
        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(keyLength))
        {
            //Create XML
            string publicKey = RSA.ToXmlString(false);
            string privateKey = RSA.ToXmlString(true);

            //publicKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(publicKey));
            //privateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(privateKey));

            //Convert to byte[]

            //Convert to base64

            //Save to disk

            File.WriteAllText(Path.Combine(Application.persistentDataPath , "public.rsa"), publicKey);
            File.WriteAllText(Path.Combine(Application.persistentDataPath , "private.rsa"), privateKey);
        }
    }

    public static void LoadKeys(int keyLength)
    {
        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(keyLength))
        {
            //Load base64
            string publicKey = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(Path.Combine(Application.persistentDataPath, "public.rsa"))));
            string privateKey = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(Path.Combine(Application.persistentDataPath, "private.rsa"))));

            //Convert to byte[]

            //Convert to XML

            //Load XML

            RSA.FromXmlString(publicKey);
            RSA.FromXmlString(privateKey);
        }
    }
}
