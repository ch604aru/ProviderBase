using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProviderBase.Data.Entities
{
    public static class Encryption
    {
        private static int KeySize = 256;
        private static int BlockSize = 128;
        private static int Iterations = 10000;
        private static int SaltLength = 8;
        private static int HashLength = 20;

        public static string EncryptString(string text)
        {
            string password = Utility.GetAppSetting("EncryptSecret", "");

            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("No encryption password found");
            }
            else
            {
                return EncryptString(text, password);
            }
        }

        public static string EncryptString(string text, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            // Hash the password with SHA256
            byte[] passwordHashBytes = SHA256Managed.Create().ComputeHash(passwordBytes);
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] saltBytes = GetRandomBytes();
            byte[] encryptedBytes = new byte[saltBytes.Length + textBytes.Length];

            // Combine Salt + Text
            for (int i = 0; i < saltBytes.Length; i++)
            {
                encryptedBytes[i] = saltBytes[i];
            }

            for (int i = 0; i < textBytes.Length; i++)
            {
                encryptedBytes[i + saltBytes.Length] = textBytes[i];
            }

            encryptedBytes = AESEncrypt(encryptedBytes, passwordHashBytes);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string DecryptString(string text)
        {
            string password = Utility.GetAppSetting("EncryptSecret", "");

            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("No encryption password found");
            }
            else
            {
                return DecryptString(text, password);
            }
        }

        public static string DecryptString(string text, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            // Hash the password with SHA256
            byte[] passwordHashBytes = SHA256Managed.Create().ComputeHash(passwordBytes);
            byte[] textBytes = Convert.FromBase64String(text);
            byte[] decryptedBytes = AESDecrypt(textBytes, passwordHashBytes);
            // Remove salt
            byte[] resultBytes = new byte[decryptedBytes.Length - SaltLength];

            for (int i = 0; i < resultBytes.Length; i++)
            {
                resultBytes[i] = decryptedBytes[i + SaltLength];
            }

            return Encoding.UTF8.GetString(resultBytes);
        }

        private static byte[] AESEncrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    Rfc2898DeriveBytes key = null;
                    string saltSecret = Utility.GetAppSetting("SaltSecret", "");
                    byte[] saltBytes = saltSecret.Split(new char[] { ',' }).Select(x => Convert.ToByte(x, 16)).ToArray();

                    key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, Iterations);

                    AES.KeySize = KeySize;
                    AES.BlockSize = BlockSize;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] AESDecrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    Rfc2898DeriveBytes key = null;
                    string saltSecret = Utility.GetAppSetting("SaltSecret", "");
                    byte[] saltBytes = saltSecret.Split(new char[] { ',' }).Select(x => Convert.ToByte(x, 16)).ToArray();

                    key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, Iterations);

                    AES.KeySize = KeySize;
                    AES.BlockSize = BlockSize;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        private static byte[] GetRandomBytes()
        {
            byte[] saltBytes = new byte[SaltLength];
            RNGCryptoServiceProvider.Create().GetBytes(saltBytes);
            return saltBytes;
        }

        public static string HashHMAC(string message)
        {
            string password = Utility.GetAppSetting("HMACSecret", "");

            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("No HMAC key found");
            }
            else
            {
                return HashHMAC(message, password);
            }
        }

        public static string HashHMAC(string message, string key)
        {
            HMACSHA256 hmacHash = null;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] hashhex = null;
            byte[] hash = null;
            byte[] hashBuffer = null;
            string hmac = "";

            hashhex = Encoding.UTF8.GetBytes(key);
            hmacHash = new HMACSHA256(hashhex);
            hashBuffer = encoding.GetBytes(message);
            hash = hmacHash.ComputeHash(hashBuffer);

            hmac = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return hmac;
        }

        public static string Hash(string password)
        {
            byte[] salt = null;
            byte[] hash = null;
            byte[] hashBytes = null;
            Rfc2898DeriveBytes pbkdf2 = null;
            RNGCryptoServiceProvider cryptoProvider = null;
            string passwordHash = "";

            salt = new byte[SaltLength];

            cryptoProvider = new RNGCryptoServiceProvider();
            cryptoProvider.GetBytes(salt);

            pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            hash = pbkdf2.GetBytes(HashLength);

            hashBytes = new byte[HashLength + SaltLength];
            Array.Copy(salt, 0, hashBytes, 0, SaltLength);
            Array.Copy(hash, 0, hashBytes, SaltLength, HashLength);

            passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;
        }

        public static bool HashCheck(string password, string hashedPassword)
        {
            byte[] salt = null;
            byte[] hash = null;
            byte[] hashBytes = null;
            Rfc2898DeriveBytes pbkdf2 = null;

            salt = new byte[SaltLength];

            hashBytes = Convert.FromBase64String(hashedPassword);

            Array.Copy(hashBytes, 0, salt, 0, SaltLength);

            pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            hash = pbkdf2.GetBytes(HashLength);

            for (int i = 0; i < HashLength; i++)
            {
                if (hashBytes[i + SaltLength] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
