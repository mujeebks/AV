using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class TreeNodeMongoDao : MongoDbObjectBase
    {
        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int TreeNodeId
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int NodeID
        {
            get;
            set;

        }
        [BsonRepresentation(BsonType.Int32)]
        public int ParentNodeID
        {
            get;
            set;

        }
        [BsonRepresentation(BsonType.Int32)]
        public int Level
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.String)]
        public string KEY
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.String)]
        public string ColorCode
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int AttributeID
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.String)]
        public virtual string Caption
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
