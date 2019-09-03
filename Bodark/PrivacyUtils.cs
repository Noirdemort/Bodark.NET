using System;
using static Bodark.NetUtils;
using static Bodark.CryptUtils;
using static Bodark.KeyChain;
using Sodium;
using System.Text;
using EasyHttp;
using EasyHttp.Http;
using System.Collections.Generic;

namespace Bodark
{
    public class PrivacyUtils
    {
        public PrivacyUtils()
        {
        }
    }

    public class DoubleRatchet
    {
        List<KeyPair> preKeys;
        KeyPair ephemeralKey;
        KeyPair identityKeys;
        byte[] signedKey;
        private string url;
        private string encryptionKey;

        public DoubleRatchet(string serverURL, KeyPair longTermKeys)
        {
            url = serverURL;
        }

        public void preKeyGeneration()
        {
            // Generate 16 prekeys
            for(int i = 0; i < 16; i++)
            {
                preKeys.Add(PublicKeyBox.GenerateSeededKeyPair(PublicKeyBox.GenerateNonce()));
            }
            
        }

        public void generateEmphemeralKey()
        {
            ephemeralKey = PublicKeyBox.GenerateKeyPair();
        }

        public void signKey()
        {

        }

        public void accountCreation()
        {
            preKeyGeneration();
            generateEmphemeralKey();
            signKey();
            // upload keys
            var publicKey = Encoding.UTF8.GetString(identityKeys.PublicKey);
            var signedPublicKey = Encoding.UTF8.GetString(signedPublicKey);
            int status = Post(url, HttpContentTypes.ApplicationXWwwFormUrlEncoded, new { pk = publicKey, spk =  signedPublicKey, prekeys= preKeys});

        }

        public void firstContact()
        {
            // perform triple diffie hellman

            var http = new HttpClient();
            http.Request.Accept = HttpContentTypes.ApplicationJson;
            var preKeyBundle = http.Get(url, null).DynamicBody;

            var df1 = new DiffieHellman();
            var ek1 = df1.computeFromPreKnownKey((string)preKeyBundle["signedKey"], identityKeys.PrivateKey);
            var ek2 = df1.computeFromPreKnownKey((string)preKeyBundle["signedKey"], ephemeralKey.PrivateKey);
            var ek3 = df1.computeFromPreKnownKey((string)preKeyBundle["identityKey"], ephemeralKey.PrivateKey);
            var ek4 = df1.computeFromPreKnownKey((string)preKeyBundle["prekey"], ephemeralKey.PrivateKey);
            var exportKeys = new[] { ek1, ek2, ek3, ek4};
            int status = Post(url, HttpContentTypes.ApplicationXWwwFormUrlEncoded, new { keySet= exportKeys});
            encryptionKey = PasswordHash.ScryptHashString(ek1 + ek2 + ek3 + ek4);

          
        }

        public void turnDiffieHellmanRatchet()
        {

        }

        public void turnSendingRatchet()
        {

        }

        public void turnReceivingRatchet()
        {

        }

        public void sendMessage(string message)
        {
            turnDiffieHellmanRatchet();
            turnSendingRatchet();
            var cipherText = CryptUtils.SimpleAESEncryption(outputChain.latest.value, message, outputChain.latest.nonce);
            Post(server, HttpContentTypes.TextPlain, new { message=cipherText });
            sendDiffieHellmanParameters();
            receiveDiffieHellmanParameters();
        }

        public string receiveMessage(string cipherText)
        {
            turnDiffieHellmanRatchet();
            turnReceivingRatchet();
            var plainText = CryptUtils.SimpleAESDecryption(outputChain.latest.value, cipherText, outputChain.latest.nonce);
            receiveDiffieHellmanParameters();
            sendDiffieHellmanParameters();
            return plainText
        }

        public void sendDiffieHellmanParameters()
        {

        }

        public void receiveDiffieHellmanParameters()
        {
            
        }

    }

    public class Signal
    {
        public Signal()
        {

        }
    }

    public class DeniableAuth
    {
        public DeniableAuth()
        {

        }
    }

    public class ZeroKnowledgeProof
    {
        public ZeroKnowledgeProof()
        {

        }
    }

    // File and message Signature
    public class Signature
    {
        public Signature()
        {

        }
    }

    public class HomomorphicMachine
    {
        public HomomorphicMachine()
        {

        }
    }

    public class PerfectForwardSecrectDrive
    {
        public PerfectForwardSecrectDrive()
        {

        }
    }

}

