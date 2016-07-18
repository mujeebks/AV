using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class AttributeDataMongoDao : MongoDbObjectBase
    {
        [BsonRepresentation(BsonType.Int32)]
        public int AttributeID { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Int32)]
        public int TypeID { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public string Lable { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public string Caption { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public string Value { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public string ValueCaption { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public string SpecialValue { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Int32)]
        public int Level { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsSpecial { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsInheritFromParent { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsChooseFromParent { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsReadOnly { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Int32)]
        public int SortOrder { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public string tree { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public string options { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Int32)]
        public int MinValue { get; set; }
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.Int32)]
        public int MaxValue { get; set; }

    }
}
