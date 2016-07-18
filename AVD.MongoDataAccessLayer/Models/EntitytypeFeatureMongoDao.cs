using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class EntitytypeFeatureMongoDao : MongoDbObjectBase
    {
        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int TypeFeatureId
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int TypeID
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int FeatureID
        {
            get;
            set;
        }


        #endregion
    }
}
