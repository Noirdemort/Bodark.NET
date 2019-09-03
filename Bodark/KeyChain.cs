using System;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using Sodium;
using static Bodark.CryptUtils;
using System.Collections.Generic;

namespace Bodark
{
    public class KeyChain
    {
        private string secretKey;
        private string separator = ",#,";

        MongoClient client;
        IMongoDatabase database;
        IMongoCollection<BsonDocument> collection;

        public KeyChain(string key)
        {
            secretKey = key;
            client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("vault");
            collection = database.GetCollection<BsonDocument>("locker");
        }

        public void addKey(string website, string username, string password)
        {
            var nonce = SecretAeadAes.GenerateNonce();
            var totalString = username + separator + password;
            var encryptedData = SimpleAESEncryption(secretKey, totalString, nonce);

            //var bsonCredentials = new BsonDocument {
            //    { "credentials", encryptedData},
            //    { "nonce", Encoding.UTF8.GetString(nonce) }
            //};
            var doc = new BsonDocument
            {
                { "website", website },
                { "credentials", encryptedData },
                { "nonce", Encoding.UTF8.GetString(nonce) }
            };

            collection.InsertOne(doc);
        }

        public void deleteKey(string website)
        {
            var doc = new BsonDocument
            {
                { "website", website }
            };
            collection.DeleteMany(doc);
        }

        public void updateKey(string website, string username, string password)
        {
            deleteKey(website);
            addKey(website, username, password);
        }

        public Dictionary<string, string> findKey(string website)
        {
            var credentials = new Dictionary<string, string>();
            var document = collection.Find(new BsonDocument { { "website", website} }).FirstOrDefault();
            var decryptedData = CryptUtils.SimpleAESDecryption(secretKey, document["credentials"].ToString(), Encoding.UTF8.GetBytes(document["nonce"].ToString()));
            var decryptedCredentials = decryptedData.Split(new[] { separator }, StringSplitOptions.None);
            credentials["username"] = decryptedCredentials[0];
            credentials["password"] = decryptedCredentials[1];
            return credentials;
        }
    }
}
