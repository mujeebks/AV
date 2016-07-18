using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class MetadataVersionMongoDao : MongoDbObjectBase
    {
        [BsonRepresentation(BsonType.Int32)]
        public int VersionId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string VersionName { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedDate { get; set; }
        public List<EntityTypeMongoDao> EntityTypes { get; set; }
        public List<AttributeMongoDao> Attributes { get; set; }
        public List<EntityTypeAttributeRelationMongoDao> EntityTypeAttributeRelation { get; set; }
        public List<OptionMongoDao> Options { get; set; }
        public List<ModuleMongoDao> Modules { get; set; }
        public List<FeatureMongoDao> Features { get; set; }
        public List<EntitytypeFeatureMongoDao> EntityFeatures { get; set; }
        public List<EntityTypeHierarchyMongoDao> EntityTypeHierarchy { get; set; }
        public List<TreeLevelMongoDao> TreeLevels { get; set; }
        public List<TreeNodeMongoDao> TreeNodes { get; set; }

    }
}
