using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace ByteEncryptor
{
    class AESEncrypter
    {
        private byte[] AESKey { get; set; }
        private Aes myAes = Aes.Create();
        //private byte[] iv = Encoding.ASCII.GetBytes("0000000000000000");
        //private byte[] sharedSecret = Encoding.ASCII.GetBytes("ABCDEFG123456789");
        public AESEncrypter(string key)
        {
            using (SHA1Managed mySHA1 = new SHA1Managed())
            {
                AESKey = mySHA1.ComputeHash(Encoding.UTF8.GetBytes(key)).Take(16).ToArray<byte>();
            }
        }

        public string EncryptString(string stringToEncrypt,byte[] sharedSecret, byte[] iv)
        {
            byte[] encrypted;
            //using (Aes myAes = Aes.Create())
            //{
            // Encrypt the string to an array of bytes.


            encrypted = EncryptStringToBytes_Aes(stringToEncrypt, sharedSecret, iv);
            //}
            return Convert.ToBase64String(encrypted);
        }

        public string DecryptString(string encryptedString,byte[] sharedSecret, byte[] iv)
        {
            string decrypted;
            byte[] encryptedBytes = Convert.FromBase64String(encryptedString);

            decrypted = DecryptStringFromBytes_Aes(encryptedBytes, sharedSecret, iv);
            return decrypted;
        }

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key,
            byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key
                    , aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt
                        , encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(
                            csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

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
                        }
                    }
                }

            }

            return plaintext;

        }


    }
}