using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

public static class RsaHelper
{
    // ======= Sign a single message =======
    public static string Sign(string messageStr, string privateKeyXml)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKeyXml);
            byte[] data = Encoding.UTF8.GetBytes(messageStr);
            byte[] signMessage = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signMessage);
        }
    }

    // ======= Verify a single message =======
    public static bool Verify(string messageStr, string signature, string publicKeyXml)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(publicKeyXml);
            byte[] dataToVerify = Encoding.UTF8.GetBytes(messageStr);
            byte[] signatureData = Convert.FromBase64String(signature);
            return rsa.VerifyData(dataToVerify, signatureData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }

    // ======= Sign many messages =======
    public static List<string> SignMany(List<string> messages, string privateKeyXml)
    {
        var result = new List<string>();
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKeyXml);
            foreach (var msg in messages)
            {
                byte[] data = Encoding.UTF8.GetBytes(msg);
                byte[] signMessage = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                result.Add(Convert.ToBase64String(signMessage));
            }
        }
        return result;
    }

    // ======= Verify many messages (optimized) =======
    public static List<bool> VerifyMany(List<string> messages, List<string> signatures, string publicKeyXml)
    {
        var result = new List<bool>();
        int count = Math.Min(messages.Count, signatures.Count);
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(publicKeyXml);
            for (int i = 0; i < count; i++)
            {
                byte[] dataToVerify = Encoding.UTF8.GetBytes(messages[i]);
                byte[] signatureData = Convert.FromBase64String(signatures[i]);
                result.Add(rsa.VerifyData(dataToVerify, signatureData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            }
        }
        return result;
    }

    // ======= Encrypt a single message =======
    public static string Encrypt(string messageStr, string publicKeyXml)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(publicKeyXml);
            byte[] data = Encoding.UTF8.GetBytes(messageStr);
            byte[] encryptedData = rsa.Encrypt(data, false); // PKCS#1 v1.5 padding
            return Convert.ToBase64String(encryptedData);
        }
    }

    // ======= Encrypt many messages (optimized) =======
    public static List<string> EncryptMany(List<string> messages, string publicKeyXml)
    {
        var result = new List<string>();
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(publicKeyXml);
            foreach (var msg in messages)
            {
                byte[] data = Encoding.UTF8.GetBytes(msg);
                byte[] encryptedData = rsa.Encrypt(data, false);
                result.Add(Convert.ToBase64String(encryptedData));
            }
        }
        return result;
    }

    // ======= Decrypt a single message =======
    public static string Decrypt(string encryptedStr, string privateKeyXml)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKeyXml);
            byte[] data = Convert.FromBase64String(encryptedStr);
            byte[] decryptedData = rsa.Decrypt(data, false);
            return Encoding.UTF8.GetString(decryptedData);
        }
    }

    // ======= Decrypt many messages (optimized) =======
    public static List<string> DecryptMany(List<string> encryptedMessages, string privateKeyXml)
    {
        var result = new List<string>();
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKeyXml);
            foreach (var enc in encryptedMessages)
            {
                byte[] data = Convert.FromBase64String(enc);
                byte[] decryptedData = rsa.Decrypt(data, false);
                result.Add(Encoding.UTF8.GetString(decryptedData));
            }
        }
        return result;
    }
}