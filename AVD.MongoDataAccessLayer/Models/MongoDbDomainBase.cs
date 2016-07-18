using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    class MongoDbDomainBase : MongoDbObjectBase
    {
        [BsonRepresentation(BsonType.String)]
        public Guid DomainId { get; set; }
    }
}
