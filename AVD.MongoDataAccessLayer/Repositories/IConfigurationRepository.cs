using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVD.MongoDataAccessLayer.Models;

namespace AVD.MongoDataAccessLayer.Repositories
{
    public interface IConfigurationRepository
    {
        IList<EntityMongoDao> GetEntities();
        IList<EntityMongoDao> GetEntitiesForTimePeriod(DateTime searchStartDateUtc, DateTime searchEndDateUtc);
        IList<EntityMongoDao> GetEntities(int pageNo, int pageSize);
        MetadataVersionMongoDao MetadataVersion(string versionCollectionName);
        List<EntityTypeAttributeRelationMongoDao> GetEntityTypeRelationById(string collectionName, int entityTypeId, int versionID);
        void AddOrUpdateLoadEntites(List<EntityMongoDao> ToBeInserted = null, List<EntityMongoDao> ToBeUpdated = null);
        void DeleteById(int id);

        int SaveObject<T>(T saveObj, string versionName);
        bool SaveObject<T>(List<T> listObj, string versionName);

        bool DeleteObject<T>(T deleteObj, string versionName);
        bool DeleteObject<T>(List<T> deleteArr, string versionName);

    }


}
