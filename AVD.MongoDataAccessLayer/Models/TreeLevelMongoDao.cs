using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class TreeLevelMongoDao : MongoDbObjectBase
    {
        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int TreeLevelId
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
        public string LevelName
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

        [BsonRepresentation(BsonType.Boolean)]
        public virtual bool IsPercentage
        {
            get;
            set;

        }


        #endregion
    }
}
