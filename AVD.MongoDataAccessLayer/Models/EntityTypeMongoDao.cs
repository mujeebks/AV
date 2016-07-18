using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class EntityTypeMongoDao : MongoDbObjectBase
    {
        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int EntityTypeId
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

        [BsonRepresentation(BsonType.String)]
        public int ModuleID
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.String)]
        public string ModuleCaption
        {
            get;
            set;
        }


        [BsonRepresentation(BsonType.Int32)]
        public int Category
        {
            get;
            set;

        }


        [BsonRepresentation(BsonType.String)]
        public string ShortDescription
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

        [BsonRepresentation(BsonType.Boolean)]
        public virtual bool IsAssociate
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Boolean)]
        public virtual bool IsRootLevel
        {
            get;
            set;
        }


        #endregion
    }
}
