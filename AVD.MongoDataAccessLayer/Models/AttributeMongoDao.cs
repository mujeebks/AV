using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{

    public class AttributeMongoDao : MongoDbObjectBase
    {

        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int AttributeId
        {
            get;
            set;

        }
        [BsonRepresentation(BsonType.String)]
        public string Caption
        {
            get;
            set;
        }
        [BsonRepresentation(BsonType.String)]
        public string Type
        {
            get;
            set;
        }

        [BsonRepresentation(BsonType.String)]
        public string Description
        {
            get;
            set;
        }

        [BsonRepresentation(BsonType.Int32)]
        public int AttributeTypeID
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsSystemDefined
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsSpecial
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

        #endregion


    }
}
