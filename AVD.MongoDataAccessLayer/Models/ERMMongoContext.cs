using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.MongoDataAccessLayer.Models;
using AVD.MongoDataAccessLayer.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AVD.MongoDataAccessLayer.Models
{
    public class ERMMongoContext
    {
        private IMongoClient Client { get; set; }
        private IMongoDatabase Database { get; set; }
        private const string _databaseName = "MarketRobo";
        private static ERMMongoContext _loadTestingContext;

        private ERMMongoContext() { }

        public static ERMMongoContext Create(IMongoConnectionStringRepository connectionStringRepository)
        {
            if (_loadTestingContext == null)
            {
                _loadTestingContext = new ERMMongoContext();
                string connectionString = connectionStringRepository.ReadConnectionString("MongoDbMarketRoboContext");
                _loadTestingContext.Client = new MongoClient(connectionString);
                _loadTestingContext.Database = _loadTestingContext.Client.GetDatabase(_databaseName);
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
