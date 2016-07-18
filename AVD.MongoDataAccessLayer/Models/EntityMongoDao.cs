using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class EntityMongoDao : MongoDbObjectBase
    {
        [BsonElementAttribute("EntityId")]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Int32)]
        public int EntityId { get; set; }
        [BsonElementAttribute("Name")]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public string Name { get; set; }
        [BsonElementAttribute("CreationDate")]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreationDate { get; set; }
        [BsonElementAttribute("UniqueKey")]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public string UniqueKey { get; set; }
        [BsonElementAttribute("AttributeData")]
        [BsonIgnoreIfNull]
        public List<AttributeDataMongoDao> AttributeData { get; set; }
        [BsonElementAttribute("TypeName")]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public string TypeName { get; set; }
        [BsonElementAttribute("TypeId")]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Int32)]
        public int TypeId { get; set; }

    }
}
