using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.MongoDataAccessLayer.Models;
using AVD.MongoDataAccessLayer.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace AVD.MongoDataAccessLayer.Models
{
    public class ERMMongoContext
    {
        private IMongoClient Client { get; set; }
        private IMongoDatabase Database { get; set; }
        private const string DatabaseName = "MarketRobo";
        private static ERMMongoContext _loadTestingContext;

        private ERMMongoContext() { }

        public static ERMMongoContext Create(IMongoConnectionStringRepository connectionStringRepository)
        {
            if (_loadTestingContext == null)
            {
                _loadTestingContext = new ERMMongoContext();
                string connectionString = connectionStringRepository.ReadConnectionString("MongoDbERMMongoContext");
                _loadTestingContext.Client = new MongoClient(connectionString);
                _loadTestingContext.Database = _loadTestingContext.Client.GetDatabase(DatabaseName);
            }
           
            return _loadTestingContext;
        }

        public IMongoCollection<EntityMongoDao> Entities
        {
            get { return Database.GetCollection<EntityMongoDao>("Entities"); }
        }

        public IMongoCollection<MetadataVersionMongoDao> MetadataVersion(string collectionNames)
        {
            return Database.GetCollection<MetadataVersionMongoDao>(collectionNames);
        }

        public IMongoCollection<BsonDocument> GetCollectionBsonDocument(string collectionName)
        {
            return Database.GetCollection<BsonDocument>(collectionName);
        }

        public async void CollectionBsonDocument(string collectionName)
        {
            var collection = Database.GetCollection<BsonDocument>(collectionName);
            var filter = new BsonDocument();
            var count = 0;
            using (var cursor = await collection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {

                        // process document
                        count++;
                    }
                }
            }
        }
    }
}
