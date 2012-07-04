using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace MSW.Utilities
{
    public static class Cryptography
    {
        //THESE VALUES ***MUST NOT*** BE CHANGED
        private const string m_sKey = "P0rt3r1$@w3s0m3$@uc3!!";
        private const string m_sIV = "y0rm0m@p0urz$@Lt";
        private const int m_iKeySize = 256;
        //END VALUES

        /// <summary>
        /// This override provides a constant private key for you, allowing for easier/weaker encryption.
        /// </summary>
        /// <param name="sPlainText"></param>
        /// <returns></returns>
        public static string EncryptString(string sPlainText)
        {
            return EncryptString(sPlainText, m_sKey, m_sIV);
        }

        /// <summary>
        /// This override provides an Initial Vector for you, allowing you to pass in a key that you can determine
        /// from business rules.
        /// </summary>
        /// <param name="sPlainText"></param>
        /// <param name="sEncryptionKey"></param>
        /// <returns></returns>
        public static string EncryptString(string sPlainText, string sEncryptionKey)
        {
            return EncryptString(sPlainText, sEncryptionKey, m_sIV);
        }

        public static string EncryptString(string sPlainText, string sEncryptionKey, string sInitialVector)
        {
            if (!IsIVValid(sInitialVector))
            {
                throw new Exception("Invalid IV Value. Must be 16 characters long.");
            }

            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            // encoding.
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(sInitialVector);
            byte[] saltValueBytes = Encoding.UTF8.GetBytes(sEncryptionKey.Length.ToString());

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(sPlainText);

            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(sEncryptionKey, saltValueBytes);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(m_iKeySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                            keyBytes,
                                                            initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                        encryptor,
                                                        CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipherText;
        }

        /// <summary>
        /// This override provides a constant private key for you, allowing for easier/weaker decryption.
        /// </summary>
        /// <param name="sEncryptedText"></param>
        /// <returns></returns>
        public static string DecryptString(string sEncryptedText)
        {
            return DecryptString(sEncryptedText, m_sKey, m_sIV);
        }

        /// <summary>
        /// This override provides an Initial Vector for you, allowing you to pass in a key that you can determine
        /// from business rules.
        /// </summary>
        /// <param name="sEncryptedText"></param>
        /// <param name="sEncryptionKey"></param>
        /// <returns></returns>
        public static string DecryptString(string sEncryptedText, string sEncryptionKey)
        {
            return DecryptString(sEncryptedText, sEncryptionKey, m_sIV);
        }

        public static string DecryptString(string sEncryptedText, string sEncryptionKey, string sInitialVector)
        {
            if (!IsIVValid(sInitialVector))
            {
                throw new Exception("Invalid IV Value. Must be 16 characters long.");
            }

            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(sInitialVector);
            byte[] saltValueBytes = Encoding.UTF8.GetBytes(sEncryptionKey.Length.ToString());

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(sEncryptedText);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(sEncryptionKey, saltValueBytes);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(m_iKeySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                            keyBytes,
                                                            initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         decryptor,
                                                         CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                      0,
                                                      plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                      0,
                                                      decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }

        private static bool IsIVValid(string sIV)
        {
            if (sIV.Length != 16)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

