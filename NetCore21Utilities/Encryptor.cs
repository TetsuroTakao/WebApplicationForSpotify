using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetCore21Utilities
{
    /// <summary>
    /// Crypt utility
    /// This class use ConfigurationBuilder class and settings.json
    /// </summary>
    public class Encryptor
    {
        Aes aes { get; set; }
        //To create AES key, input your prefer key value, then might added Zero backward until byte length 16 at constructor.
        static string key = "processtune";
        byte[] bufferPassword = null;
        public static string EncryptedString { get; set; }

        public Encryptor()
        {
            aes = AesCryptoServiceProvider.Create();
            aes.Mode = CipherMode.ECB;
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            createAESKey();
        }

        public Encryptor(CipherMode mode = CipherMode.ECB)
        {
            aes.Mode = mode;
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            createAESKey();
        }

        void createAESKey()
        {
            byte[] bufferKey = new byte[16];
            bufferPassword = Encoding.UTF8.GetBytes(key);
            aes.Key = bufferPassword.Concat(bufferKey).Take(16).ToArray();
        }

        public string Crypt(string text)
        {
            byte[] encrypted = null;
            byte[] text_bytes = Encoding.UTF8.GetBytes(text);
            if (aes.Key.Length == 0) createAESKey();
            var conf = new Configuration();
            var key = conf.Get("AesCryptoServiceProvider Key");
            var iv = conf.Get("AesCryptoServiceProvider IV");
            byte[] Key = Convert.FromBase64String(key);
            byte[] IV = Convert.FromBase64String(iv);
            aes.Key = Key;
            aes.IV = IV;
            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptStream.Write(text_bytes, 0, text_bytes.Length);
                        cryptStream.FlushFinalBlock();
                        encrypted = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string encryptedText)
        {
            byte[] encrypted = System.Convert.FromBase64String(encryptedText);
            byte[] planeText = new byte[encrypted.Length];
            var conf = new Configuration();
            var key = conf.Get("AesCryptoServiceProvider Key");
            var iv = conf.Get("AesCryptoServiceProvider IV");
            byte[] Key = Convert.FromBase64String(key);
            byte[] IV = Convert.FromBase64String(iv);
            aes.Key = Key;
            aes.IV = IV;
            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            {
                using (MemoryStream memoryStream = new MemoryStream(encrypted))
                {
                    using (CryptoStream cryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        cryptStream.Read(planeText, 0, planeText.Length);
                    }
                }
            }
            string result = Encoding.UTF8.GetString(planeText);
            return result.Replace("\0","");
        }

        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            EncryptedString = Convert.ToBase64String(encrypted);
            return encrypted;
        }

        public static string DecryptStringFromBytes_Aes(string original)
        {
            byte[] cipherText = new byte[0];
            string plaintext = null;
            var conf = new Configuration();
            var key = conf.Get("AesCryptoServiceProvider Key");
            var iv = conf.Get("AesCryptoServiceProvider IV");
            byte[] Key = Convert.FromBase64String(key);
            byte[] IV = Convert.FromBase64String(iv);
            using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider() { KeySize = 128,Padding = PaddingMode.PKCS7 })
            {
                cipherText = Convert.FromBase64String(original);
                if (cipherText == null || cipherText.Length <= 0)
                    throw new ArgumentNullException("cipherText");
                if (Key == null || Key.Length <= 0)
                    throw new ArgumentNullException("Key");
                if (IV == null || IV.Length <= 0)
                    throw new ArgumentNullException("IV");
                myAes.Key = Key;
                myAes.IV = IV;
                ICryptoTransform decryptor = myAes.CreateDecryptor(myAes.Key, myAes.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}