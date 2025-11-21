using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;

namespace AllinOne.Utils.Helpers
{
    public static class AesGcmEncryptionHelper
    {
        private static readonly byte[] Key = LoadKeyFromEnvironment();

        //load the key once when the app starts
        private static byte[] LoadKeyFromEnvironment()
        {
            //read the ENCRYPTION_KEY (Base64)
            var b64 = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
            if (string.IsNullOrWhiteSpace(b64))
            {
                throw new InvalidOperationException("Missing ENCRYPTION_KEY environment variable (base64).");
            }

            var key = Convert.FromBase64String(b64);
            // must be exactly 32 bytes (AES-256)
            if (key.Length != 32)
            {
                throw new InvalidOperationException("Encryption key must be 32 bytes (base64).");
            }

            return key;
        }

        private const int NonceSize = 12; // Recommended from NIST: Nonce/GCM = 12 bytes (96 bits)
        private const int TagSize = 16;   // Authentication Tag default size 16 bytes (128bit)

        public static string Encrypt(string plainText)
        {
            if (plainText == null)
            {
                return null;
            }

            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            //Nonce: unique per encryption (Number Used Once) 
            //if you encrypt the same text twice, you should not get the same
            var nonce = new byte[NonceSize]; 
            RandomNumberGenerator.Fill(nonce); //crypto RNG

            var ciphertext = new byte[plainBytes.Length];
            //tag
            //proof that the encryption was done correctly
            //proof that no one has tampered with the data
            //proof that you are using the correct key
            var tag = new byte[TagSize];

            using (var aes = new AesGcm(Key))
            {
                //nonce->random 12 bytes
                //plainBytes->the original text
                //ciphertext->there will write the encrypted bytes
                //tag->there will write the authentication tag
                //associatedData: null->you do not add “headers” that need to be authenticated
                //encrypts plainBytes using: Key and nonce
                //and writes the result to the ciphertext
                //Creates Authentication Tag(integrity check) and writes it to the tag
                aes.Encrypt(nonce, plainBytes, ciphertext, tag, null);
            }

            var result = new byte[NonceSize + TagSize + ciphertext.Length];
            //put the 12 bytes of the nonce at the beginning of the result.
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            //after the nonce, put the tag
            Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
            //after nonce+tag, put the ENTIRE ciphertext
            Buffer.BlockCopy(ciphertext, 0, result, NonceSize + TagSize, ciphertext.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string combinedBase64)
        {
            if (combinedBase64 == null) return null;

            byte[] combined;
            try
            {
                combined = Convert.FromBase64String(combinedBase64);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Ciphertext is not valid Base64.", ex);
            }

            if (combined.Length < NonceSize + TagSize)
            {
                // NonceSize + TagSize -> must always be contained
                throw new InvalidOperationException("Ciphertext too short to contain nonce+tag.");
            }

            var nonce = new byte[NonceSize];    // 12-byte nonce
            var tag = new byte[TagSize];        // 16-byte authentication tag
            var ciphertextLength = combined.Length - NonceSize - TagSize;
            var ciphertext = new byte[ciphertextLength]; //ciphertext

            Buffer.BlockCopy(combined, 0, nonce, 0, NonceSize);                               // 1 nonce
            Buffer.BlockCopy(combined, NonceSize, tag, 0, TagSize);                           // 2 tag
            Buffer.BlockCopy(combined, NonceSize + TagSize, ciphertext, 0, ciphertextLength); // 3 ciphertext

            var plaintext = new byte[ciphertextLength];
            try
            {
                using (var aes = new AesGcm(Key))
                {
                    // AES-GCM Decrypt:
                    // nonce -> used to unlock
                    // ciphertext -> encrypted bytes
                    // tag -> authentication tag for integrity checking
                    // plaintext -> the decrypted result will be written here
                    // associatedData = null -> we do not have AAD
                    aes.Decrypt(nonce, ciphertext, tag, plaintext, null);
                }
            }
            catch (CryptographicException ex)
            {
                // if the tag does not match or the key is wrong
                throw new InvalidOperationException("Decryption failed — authentication tag mismatch or bad key.", ex);
            }

            return Encoding.UTF8.GetString(plaintext);
        }

        //you ONLY need it to create the first key
        public static string GenerateNewKeyBase64()
        {
            var key = new byte[32];
            RandomNumberGenerator.Fill(key);
            return Convert.ToBase64String(key);
        }
    }

}
