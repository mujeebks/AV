using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class ModuleMongoDao : MongoDbObjectBase
    {
        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int ModuleId
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
        public string Description
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsEnable
        {
            get;
            set;

        }

        #endregion
    }
}
