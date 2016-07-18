using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class OptionMongoDao : MongoDbObjectBase
    {
        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int OptionId
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

        [BsonRepresentation(BsonType.Int32)]
        public int AttributeID
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

        [BsonRepresentation(BsonType.Int32)]
        public int SortOrder
        {
            get;
            set;

        }




        #endregion
    }
}
