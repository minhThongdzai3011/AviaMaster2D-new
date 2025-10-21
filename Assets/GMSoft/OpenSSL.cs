using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets.GMDev.Utilities
{
    /// <summary>
    /// Mã hóa dữ liệu md5 - ssl
    /// </summary>
    public static class OpenSSL
    {
        public static byte GenB64CharByte()
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var _rand = new Random();
            var b = _rand.Next(255);
            switch (_rand.Next(2))
            {
                // In each case use mod to project byte b to the correct range
                case 0:
                    return Convert.ToByte(lower[b % lower.Length]);
                case 1:
                    return Convert.ToByte(upper[b % upper.Length]);
            }
            return (byte)b;
        }

        public static byte GenB64Byte()
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            var _rand = new Random();
            var b = _rand.Next(255);
            switch (_rand.Next(3))
            {
                // In each case use mod to project byte b to the correct range
                case 0:
                    return Convert.ToByte(lower[b % lower.Length]);
                case 1:
                    return Convert.ToByte(upper[b % upper.Length]);
                case 2:
                    return Convert.ToByte(number[b % number.Length]);
            }
            return (byte)b;
        }

        public static string GenB64Pass(int length = 24)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";

            // Get cryptographically random sequence of bytes
            var bytes = new byte[length];
            new RNGCryptoServiceProvider().GetBytes(bytes);

            // Build up a string using random bytes and character classes
            var res = new StringBuilder();
            var _rand = new Random();
            foreach (byte b in bytes)
            {
                // Randomly select a character class for each byte
                switch (_rand.Next(3))
                {
                    // In each case use mod to project byte b to the correct range
                    case 0:
                        res.Append(lower[b % lower.Length]);
                        break;
                    case 1:
                        res.Append(upper[b % upper.Length]);
                        break;
                    case 2:
                        res.Append(number[b % number.Length]);
                        break;
                }
            }
            return res.ToString();
        }

        public static string GenPassword(int length = 24)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            const string special = "!@#$%^&*_-=+";

            // Get cryptographically random sequence of bytes
            var bytes = new byte[length];
            new RNGCryptoServiceProvider().GetBytes(bytes);

            // Build up a string using random bytes and character classes
            var res = new StringBuilder();
            var _rand = new Random();
            foreach (byte b in bytes)
            {
                // Randomly select a character class for each byte
                switch (_rand.Next(4))
                {
                    // In each case use mod to project byte b to the correct range
                    case 0:
                        res.Append(lower[b % lower.Length]);
                        break;
                    case 1:
                        res.Append(upper[b % upper.Length]);
                        break;
                    case 2:
                        res.Append(number[b % number.Length]);
                        break;
                    case 3:
                        res.Append(special[b % special.Length]);
                        break;
                }
            }
            return res.ToString();
        }

        public static string EncodeWithPass(string input)
        {
            int passLength = UnityEngine.Random.Range(10, 24);
            int l1 = UnityEngine.Random.Range(3, passLength / 2);
            int l2 = passLength - l1;
            string pass = OpenSSL.GenB64Pass(passLength);
            string encodeText = OpenSSL.OpenSSLEncrypt(input, pass);
            List<byte> bytes = new List<byte>();
            bytes.Add(OpenSSL.GenB64Byte()); //noise
            bytes.Add((byte)(System.Convert.ToByte('G') + l1)); //length 1
            bytes.Add(OpenSSL.GenB64Byte()); //noise
            bytes.Add((byte)(System.Convert.ToByte('m') + l2)); //length 2
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(pass)); // add password
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(encodeText)); // add endcode data
            return System.Convert.ToBase64String(bytes.ToArray()); // endcoded text
        }

        public static string DecodeWithPass(string input)
        {
            if (IsValidBase64(input))
            {
                var data = System.Convert.FromBase64String(input);
                int passLength = data[1] - System.Convert.ToByte('G') + data[3] - System.Convert.ToByte('m');
                string pass = System.Text.Encoding.UTF8.GetString(SubArr(data, 4, passLength));
                string encodeText = System.Text.Encoding.UTF8.GetString(SubArr(data, 4 + passLength));
                return OpenSSLDecrypt(encodeText, pass);
            }
            return null;
        }

        public static bool IsValidBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return false;
            }

            if (base64.Length % 4 != 0)
            {
                return false;
            }
            string pattern = @"^[a-zA-Z0-9\+/]*={0,2}$";
            return Regex.IsMatch(base64, pattern);
        }

        private static byte[] SubArr(byte[] arr, int index, int length = -1)
        {
            if (length == -1)
            {
                length = arr.Length - index;
            }
            byte[] res = new byte[length];
            for (int i = 0; i < length; i++)
            {
                res[i] = arr[index + i];
            }
            return res;
        }

        /// <summary>
        /// Mã hóa dữ liệu text
        /// </summary>
        /// <param name="plainText">Dữ liệu text</param>
        /// <param name="passphrase">Mật khẩu mã hóa</param>
        /// <returns>Chuỗi dữ liệu sau khi mã hóa</returns>
        public static string OpenSSLEncrypt(string plainText, string passphrase)
        {
            // generate salt
            byte[] key, iv;
            byte[] salt = new byte[8];// {82, 170, 215, 131, 129, 98, 241, 107 };
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(salt);
            //for (int i = 0; i < salt.Length; i++) salt[i] = (byte)(i + 1);
            //Console.WriteLine($"salt: {string.Join(" ", salt)}");
            DeriveKeyAndIV(passphrase, salt, out key, out iv);
            //Console.WriteLine($"key: {BytesToString(key)} iv: {BytesToString(iv)}");
            // encrypt bytes
            byte[] encryptedBytes = EncryptStringToBytesAes(plainText, key, iv);
            // add salt as first 8 bytes
            byte[] encryptedBytesWithSalt = new byte[salt.Length + encryptedBytes.Length + 8];
            Buffer.BlockCopy(StringToBytes("Salted__"), 0, encryptedBytesWithSalt, 0, 8);
            Buffer.BlockCopy(salt, 0, encryptedBytesWithSalt, 8, salt.Length);
            Buffer.BlockCopy(encryptedBytes, 0, encryptedBytesWithSalt, salt.Length + 8, encryptedBytes.Length);
            // base64 encode
            return Convert.ToBase64String(encryptedBytesWithSalt);
        }

        /// <summary>
        /// Giải mã dữ liệu text
        /// </summary>
        /// <param name="encrypted">Dữ liệu mã hóa</param>
        /// <param name="passphrase">Mật khẩu mã hóa</param>
        /// <returns>Dữ liệu sau khi giải mã</returns>
        public static string OpenSSLDecrypt(string encrypted, string passphrase)
        {
            // base 64 decode
            byte[] encryptedBytesWithSalt = Convert.FromBase64String(encrypted);
            // extract salt (first 8 bytes of encrypted)
            byte[] salt = new byte[8];
            byte[] encryptedBytes = new byte[encryptedBytesWithSalt.Length - salt.Length - 8];
            Buffer.BlockCopy(encryptedBytesWithSalt, 8, salt, 0, salt.Length);
            Buffer.BlockCopy(encryptedBytesWithSalt, salt.Length + 8, encryptedBytes, 0, encryptedBytes.Length);
            // get key and iv
            byte[] key, iv;
            DeriveKeyAndIV(passphrase, salt, out key, out iv);
            //Console.WriteLine($"key: {BytesToString(key)} iv: {BytesToString(iv)} encryptedBytes:{BytesToString(encryptedBytes)}");
            return DecryptStringFromBytesAes(encryptedBytes, key, iv);
        }

        static byte[] StringToBytes(string str_val)
        {
            var bytes = new byte[str_val.Length];
            for (int i = 0; i < str_val.Length; i++)
            {
                bytes[i] = Convert.ToByte(str_val[i]);
            }
            return bytes;
        }

        static string BytesToString(byte[] bytes)
        {
            var str = "";
            foreach (var b in bytes)
            {
                str += Convert.ToChar(b);
            }
            return str;
        }

        static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = StringToBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        private static void DeriveKeyAndIV(string passphrase, byte[] salt, out byte[] key, out byte[] iv)
        {
            // generate key and iv
            List<byte> concatenatedHashes = new List<byte>(48);

            byte[] password = StringToBytes(passphrase);
            byte[] currentHash = new byte[0];
            MD5 md5 = MD5.Create();
            bool enoughBytesForKey = false;
            // See http://www.openssl.org/docs/crypto/EVP_BytesToKey.html#KEY_DERIVATION_ALGORITHM
            while (!enoughBytesForKey)
            {
                int preHashLength = currentHash.Length + password.Length + salt.Length;
                byte[] preHash = new byte[preHashLength];

                Buffer.BlockCopy(currentHash, 0, preHash, 0, currentHash.Length);
                Buffer.BlockCopy(password, 0, preHash, currentHash.Length, password.Length);
                Buffer.BlockCopy(salt, 0, preHash, currentHash.Length + password.Length, salt.Length);
                //Console.WriteLine($"prehash: {GetMD5Hash(BytesToString(preHash))} t: {BytesToString(preHash)}");
                currentHash = StringToBytes(GetMD5Hash(BytesToString(preHash))); //md5.ComputeHash(preHash);
                concatenatedHashes.AddRange(currentHash);

                if (concatenatedHashes.Count >= 48)
                    enoughBytesForKey = true;
            }

            key = new byte[32];
            iv = new byte[16];
            concatenatedHashes.CopyTo(0, key, 0, 32);
            concatenatedHashes.CopyTo(32, iv, 0, 16);

            md5.Clear();
            md5 = null;
        }

        static byte[] EncryptStringToBytesAes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            // Declare the stream used to encrypt to an in memory
            // array of bytes.
            MemoryStream msEncrypt;

            // Declare the RijndaelManaged object
            // used to encrypt the data.
            RijndaelManaged aesAlg = null;

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged { Mode = CipherMode.CBC, KeySize = 256, BlockSize = 128, Key = key, IV = iv };

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
                    csEncrypt.FlushFinalBlock();
                    msEncrypt.Close();
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            string base64MsEncrypt = Convert.ToBase64String(msEncrypt.ToArray());
            //Console.WriteLine($"base 64: {base64MsEncrypt}");
            // Return the encrypted bytes from the memory stream.
            return StringToBytes(base64MsEncrypt); //msEncrypt.ToArray();
        }

        static string DecryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            //Console.WriteLine($"cipher_text = {BytesToString(cipherText)}");
            cipherText = Convert.FromBase64String(BytesToString(cipherText));
            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged { Mode = CipherMode.CBC, KeySize = 256, BlockSize = 128, Key = key, IV = iv };

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                            srDecrypt.Close();
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }
    }

}
