using System;
using System.Text;
using Sodium;

namespace Bodark
{
    public class CryptUtils
    {
        public CryptUtils()
        {
            
        }

        public static string SimpleAESEncryption(string key, string plainText, byte[] nonce)
        {
            var encodedKey = Encoding.UTF8.GetBytes(key);
            var cipherText = SecretAead.Encrypt(Encoding.UTF8.GetBytes(plainText), nonce, encodedKey, null);
            return Encoding.UTF8.GetString(cipherText);
        }

        public static string SimpleAESDecryption(string key, string cipherText, byte[] nonce)
        {
            var cipherTextInternal = Encoding.UTF8.GetBytes(cipherText);
            var encodedKey = Encoding.UTF8.GetBytes(key);
            var plainText = SecretAead.Decrypt(cipherTextInternal, nonce, encodedKey, null);
            return Encoding.UTF8.GetString(plainText);
        }

        public static string[] AESEncryption(string key, string plainText) 
        {
            var nonce = SecretAead.GenerateNonce();
            var encodedKey = Encoding.UTF8.GetBytes(key);
            var encodedData = SodiumCore.GetRandomBytes(SodiumCore.GetRandomNumber(1147483647));
            var cipherText = SecretAead.Encrypt(Encoding.UTF8.GetBytes(plainText), nonce, encodedKey, encodedData);
            return new[] { Encoding.UTF8.GetString(cipherText), Encoding.UTF8.GetString(nonce), Encoding.UTF8.GetString(encodedData) };
        }

        public static string AESDecryption(string key, string cipherText, string nonce, string additionalData)
        {
            var cipherTextInternal = Encoding.UTF8.GetBytes(cipherText);
            var privateNonce = Encoding.UTF8.GetBytes(nonce);
            var encodedKey = Encoding.UTF8.GetBytes(key);
            var encodedData = Encoding.UTF8.GetBytes(additionalData);
            var plainText = SecretAead.Decrypt(cipherTextInternal, privateNonce, encodedKey, encodedData);
            return Encoding.UTF8.GetString(plainText);
        }

        public static string[] RSAEncryption(string publicKey, string privateKey, string plainText)
        {
            var encodedPrivateKey = Encoding.UTF8.GetBytes(privateKey);
            var encodedPublicKey = Encoding.UTF8.GetBytes(publicKey);
            var nonce = PublicKeyBox.GenerateNonce();
            var cipherText = PublicKeyBox.Create(plainText, nonce, encodedPrivateKey, encodedPublicKey);
            return new[] { Encoding.UTF8.GetString(cipherText), Encoding.UTF8.GetString(nonce) };

        }

        public static string RSADecryption(string privateKey, string cipherText, string nonce, string publicKey)
        {
            var encodedPrivateKey = Encoding.UTF8.GetBytes(privateKey);
            var encodedPublicKey = Encoding.UTF8.GetBytes(publicKey);
            var cipher = Encoding.UTF8.GetBytes(cipherText);
            var privateNonce = Encoding.UTF8.GetBytes(nonce);
            var plainText = PublicKeyBox.Open(cipher, privateNonce, encodedPrivateKey, encodedPublicKey);
            return Encoding.UTF8.GetString(plainText);
        }

        public static string AnonymousRSAEncryption(string publicKey, string plainText)
        {
            var encodedPublicKey = Encoding.UTF8.GetBytes(publicKey);
            var cipherText = SealedPublicKeyBox.Create(plainText, encodedPublicKey);
            return Encoding.UTF8.GetString(cipherText);
        }

        public static string AnonymousRSADecryption(string privateKey, string cipherText)
        {
            var encodedPrivateKey = Encoding.UTF8.GetBytes(privateKey);
            var keyPair = PublicKeyBox.GenerateKeyPair(encodedPrivateKey);
            var plainText = SealedPublicKeyBox.Open(cipherText, keyPair);
            return Encoding.UTF8.GetString(plainText);
        }

        public static string sign(string privateKey, string cipherText)
        {
            var encodedKey = Encoding.UTF8.GetBytes(privateKey);
            byte[] signature = SecretKeyAuth.SignHmacSha512(cipherText, encodedKey);
            return Encoding.UTF8.GetString(signature);
        }

        public static string hash512(string message)
        {
            var hash = CryptoHash.Sha512(message);
            return Encoding.UTF8.GetString(hash);
        }

        public static bool verify(string message, string signature, string key)
        {
            var encodedSignature = Encoding.UTF8.GetBytes(signature);
            var encodedKey = Encoding.UTF8.GetBytes(key);
            return SecretKeyAuth.VerifyHmacSha512(message, encodedSignature, encodedKey);
        }

        public static string randomString()
        {
            var randomBytes = SodiumCore.GetRandomBytes(SodiumCore.GetRandomNumber(1147483647));
            return Encoding.UTF8.GetString(randomBytes);
        }

        public static string passwordHashing(string key)
        {
            var hash = PasswordHash.ScryptHashString(key, PasswordHash.Strength.MediumSlow);
            return hash;
        }
    }

    public class DiffieHellman
    {
        private byte[] secretKey;
        private byte[] receivedPublicKey;
        private byte[] sharedSecret;

        public DiffieHellman()
        {
            secretKey = SodiumCore.GetRandomBytes(SodiumCore.GetRandomNumber(1147483647));
        }

        public bool computeSharedSecret(string foreignSecret)
        {
            try
            {
                var encodedForeignSecret = Encoding.UTF8.GetBytes(foreignSecret);
                receivedPublicKey = encodedForeignSecret;
                sharedSecret = ScalarMult.Mult(secretKey, encodedForeignSecret);
                return true;
            } catch {
                return false;
            }
        }

        public string exportPublicKey()
        {
            byte[] export = ScalarMult.Base(secretKey);
            return Encoding.UTF8.GetString(export);
        }

        public string getEncryptionKey()
        {
            if (sharedSecret != null)
            {
                return Encoding.UTF8.GetString(sharedSecret);
            }
            return "-1";
        }

    }
}
