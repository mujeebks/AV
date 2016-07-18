using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class EntityTypeHierarchyMongoDao : MongoDbObjectBase
    {
        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int HierarchyId
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int ParentActivityTypeID
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int ChildActivityTypeID
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int SortOrder
        {
            get;
            set;

        }


        #endregion
    }
}
