using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{

    public class AttributeTypeMongoDao : MongoDbObjectBase
    {

        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int AttributeTypeId
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
        public string ClassName
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsSelectable
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.String)]
        public string DataType
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.String)]
        public string SqlType
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int Length
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsNullable
        {
            get;
            set;

        }


        #endregion


    }
}
