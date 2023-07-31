using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace aes_server
{
    public class aes
    {
        static byte[] aesKey = { 23, 45, 56, 67, 67, 87, 98, 12, 32, 34, 45, 56, 67, 87, 65, 5 };
        static byte[] aesIv = { 123, 43, 46, 89, 29, 187, 58, 213, 78, 50, 19, 106, 205, 1, 5, 7 };

        public static string AesEncrypt(string originalText)
        {
            // add a null character at the end
            // this is required by Arduino C/C++ as a string terminator
            // prevent Arduino process to overflow to other memory's data
            originalText += "\0";

            // convert the string into bytes (byte array)
            byte[] data = System.Text.Encoding.UTF8.GetBytes(originalText);

            // initialize AES encryption
            using (Aes aes = Aes.Create())
            {
                // set the AES parameters
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = aesKey;
                aes.IV = aesIv;

                // Create an encryptor to encrypt some data
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // create a memory stream for AES to store the encrypted bytes
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // begin the encryption process
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();

                        // get the encrypted bytes
                        data = msEncrypt.ToArray();
                    }
                }
            }

            // convert the encrypted bytes into base64 string
            // sending this text to Arduino
            return Convert.ToBase64String(data);
        }

        public static string AesDecrypt(string base64str)
        {
            byte[] data = null;

            // the base64 string into bytes that's encrypted at Arduino
            byte[] encryptedData = Convert.FromBase64String(base64str);

            // initialize AES encryption
            using (Aes aes = Aes.Create())
            {
                // set the AES parameters
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = aesKey;
                aes.IV = aesIv;

                // Create a decryptor to decrypt the data
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // initialize memory stream to read data from the encrypted bytes
                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                {
                    // initialize the AES decryption engine
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // declare a memory stream for AES to save the decrypted data
                        using (MemoryStream originalMemoryStream = new MemoryStream())
                        {
                            byte[] buffer = new byte[1024];
                            int readBytes;
                            while ((readBytes = csDecrypt.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                originalMemoryStream.Write(buffer, 0, readBytes);
                            }

                            // extract the decrypted data from the memory stream
                            data = originalMemoryStream.ToArray();
                        }
                    }
                }
            }

            // Convert the bytes into string
            string text = System.Text.Encoding.UTF8.GetString(data);

            // remove the last null character (added by Arduino as line terminator)
            text = text.Remove(text.Length - 1, 1);

            return text;
        }
    }
}