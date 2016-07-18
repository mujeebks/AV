using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AVD.MongoDataAccessLayer.Models
{
    public class EntityTypeAttributeRelationMongoDao : MongoDbObjectBase
    {
        #region Public Properties

        [BsonRepresentation(BsonType.Int32)]
        public int RelationID
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
        [BsonRepresentation(BsonType.String)]
        public string EntityTypeCaption
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
        public string AttributeCaption
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
        [BsonRepresentation(BsonType.String)]
        public string ValidationID
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
        [BsonRepresentation(BsonType.String)]
        public string DefaultValue
        {
            get;
            set;

        }
        [BsonRepresentation(BsonType.String)]
        public string PlaceHolderValue
        {
            get;
            set;

        }
        [BsonRepresentation(BsonType.Int32)]
        public int MinValue
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Int32)]
        public int MaxValue
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsHelptextEnabled
        {
            get;
            set;
        }

        [BsonRepresentation(BsonType.String)]
        public string HelptextDecsription
        {
            get;
            set;
        }

        [BsonRepresentation(BsonType.Boolean)]
        public bool InheritFromParent
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsReadOnly
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

        [BsonRepresentation(BsonType.Boolean)]
        public bool ChooseFromParentOnly
        {
            get;
            set;

        }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsValidationNeeded
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

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsSystemDefined
        {
            get;
            set;

        }

        #endregion
    }
}
