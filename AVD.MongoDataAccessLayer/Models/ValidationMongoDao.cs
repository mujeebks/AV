using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class ValidationMongoDao : MongoDbObjectBase
    {
        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int ValidationId
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int EntityTypeID
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int RelationShipID
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.String)]
        public string Name
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.String)]
        public string ValueType
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.String)]
        public string ErrorMessage
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.String)]
        public string Value
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

        #endregion
    }
}
