
/*
 * Copyright (c) 2016 MARKETROBO-AVD
 * Distributed under the MIT license - http://opensource.org/licenses/MIT
 *
 * Written with CSharpDriver-2.2.3
 * Documentation: http://api.mongodb.org/csharp/
 * A C# class connecting to a MongoDB database given a MongoDB Connection URI.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using AVD.MongoDataAccessLayer.Models;
using AVD.MongoDataAccessLayer.Repositories;
using System.Threading.Tasks;
using AVD.DataAccessLayer.Models;
using System.Collections;
using MongoDB.Bson.Serialization;
using AVD.DataAccessLayer.Repositories;

namespace AVD.MongoDataAccessLayer.Repositories
{

    public class ConfigurationRepository : GenericRepository, IConfigurationRepository
    {
        public ConfigurationRepository(IMongoConnectionStringRepository connectionStringRepository)
             : base(connectionStringRepository)
        { }

        ///<summary>
        ///Get all entites without pagination.
        ///</summary>
        public IList<EntityMongoDao> GetEntities()
        {

            var context = ERMMongoContext.Create(base.ConnectionStringRepository);
            var mongoDbLoadEntitesInSearchPeriod = context.Entities.Find(x => true)
                .ToList();

            return mongoDbLoadEntitesInSearchPeriod;
        }

        ///<summary>
        ///Get entites with pagination.
        ///</summary>
        public IList<EntityMongoDao> GetEntities(int pageNo, int pageSize)
        {
            var skipCount = (pageNo - 1) * 20;

            var context = ERMMongoContext.Create(base.ConnectionStringRepository);
            var mongoDbLoadEntitesInSearchPeriod = context.Entities.Find(x => true).Skip(skipCount).Limit(pageSize)
                .ToList();

            return mongoDbLoadEntitesInSearchPeriod;
        }

        ///<summary>
        ///Get entites by order caluse (Asc: {TypeName}, Desc:{CreationDate}).
        ///</summary>
        public IList<EntityMongoDao> GetEntities(int pageNo, int pageSize, bool sortby)
        {
            var skipCount = (pageNo - 1) * pageSize;

            var builder = Builders<EntityMongoDao>.Sort;
            var sort = builder.Ascending(x => x.EntityId).Descending(x => x.CreationDate);

            var context = ERMMongoContext.Create(base.ConnectionStringRepository);
            var mongoDbLoadEntites = context.Entities.Find(x => true).Sort(sort).SortBy(x => x.Name).ThenByDescending(x => x.EntityId).ThenByDescending(x => x.CreationDate).Skip(skipCount).Limit(pageSize)
                .ToList();
            return mongoDbLoadEntites;
        }

        ///<summary>
        ///Get all entites between creation dates.
        ///</summary>
        public IList<EntityMongoDao> GetEntitiesForTimePeriod(DateTime searchStartDateUtc, DateTime searchEndDateUtc)
        {
            var context = ERMMongoContext.Create(base.ConnectionStringRepository);

            var dateQueryBuilder = Builders<EntityMongoDao>.Filter;
            var startDateBeforeSearchStartFilter = dateQueryBuilder.Lte<DateTime>(l => l.CreationDate, searchStartDateUtc);
            var endDateAfterSearchStartFilter = dateQueryBuilder.Gte<DateTime>(l => l.CreationDate, searchStartDateUtc);
            var firstPartialDateQuery = dateQueryBuilder.And(new List<FilterDefinition<EntityMongoDao>>() { startDateBeforeSearchStartFilter, endDateAfterSearchStartFilter });

            var startDateBeforeSearchEndFilter = dateQueryBuilder.Lte<DateTime>(l => l.CreationDate, searchEndDateUtc);
            var endDateAfterSearchEndFilter = dateQueryBuilder.Gte<DateTime>(l => l.CreationDate, searchEndDateUtc);
            var secondPartialDateQuery = dateQueryBuilder.And(new List<FilterDefinition<EntityMongoDao>>() { startDateBeforeSearchEndFilter, endDateAfterSearchEndFilter });

            var thirdPartialDateQuery = dateQueryBuilder.And(new List<FilterDefinition<EntityMongoDao>>() { startDateBeforeSearchStartFilter, endDateAfterSearchEndFilter });

            var startDateAfterSearchStartFilter = dateQueryBuilder.Gte<DateTime>(l => l.CreationDate, searchStartDateUtc);
            var endDateBeforeSearchEndFilter = dateQueryBuilder.Lte<DateTime>(l => l.CreationDate, searchEndDateUtc);
            var fourthPartialQuery = dateQueryBuilder.And(new List<FilterDefinition<EntityMongoDao>>() { startDateAfterSearchStartFilter, endDateBeforeSearchEndFilter });

            var ultimateQuery = dateQueryBuilder.Or(new List<FilterDefinition<EntityMongoDao>>() { firstPartialDateQuery, secondPartialDateQuery, thirdPartialDateQuery, fourthPartialQuery });

            var mongoDbLoadtestsInSearchPeriod = context.Entities.Find(ultimateQuery)
                .ToList();

            return mongoDbLoadtestsInSearchPeriod;
        }

        /// <summary>
        /// Add or update entities
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void AddOrUpdateLoadEntites(List<EntityMongoDao> toBeInserted = null, List<EntityMongoDao> toBeUpdated = null)
        {
            var context = ERMMongoContext.Create(base.ConnectionStringRepository);

            if (toBeInserted != null && toBeInserted.Any())
            {

                context.Entities.InsertMany(toBeInserted);
            }

            if (toBeUpdated == null) return;
            foreach (var entity in toBeUpdated)
            {
                var existingEntityId = entity.EntityId;
                var loadentityInDbQuery = context.Entities.Find<EntityMongoDao>(lt => lt.EntityId == existingEntityId);
                var loadentityInDb = loadentityInDbQuery.FirstOrDefault();
                if (loadentityInDb == null) continue;
                {
                    loadentityInDb.Name = entity.Name;
                    loadentityInDb.CreationDate = entity.CreationDate;
                    context.Entities.FindOneAndReplace<EntityMongoDao>(lt => lt.DbObjectId == loadentityInDb.DbObjectId, loadentityInDb);
                }
            }
        }

        ///<summary>
        ///Delete entity by id
        ///</summary>
        public void DeleteById(int id)
        {
            var context = ERMMongoContext.Create(base.ConnectionStringRepository);
            context.Entities.FindOneAndDelete<EntityMongoDao>(lt => lt.EntityId == id);
        }

        ///<summary>
        ///Get metadata version details.
        ///</summary>
        public MetadataVersionMongoDao MetadataVersion(string versionCollectionName)
        {

            var context = ERMMongoContext.Create(base.ConnectionStringRepository);
            var mongoDbVersion = context.MetadataVersion(versionCollectionName).Find(x => true).SingleOrDefault();
            return mongoDbVersion;
        }

        /// <summary>
        /// Get MetaData Objects
        /// </summary>
        public List<T> GetObject<T>(string collectionName)
        {
            List<T> obj = null;
            try
            {

                var context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
                var versionDetailTask = context.MetadataVersion("version1").Find(x => true).SingleOrDefaultAsync();
                Task.WaitAll(versionDetailTask);
                var versionDetail = versionDetailTask.Result;

                if (typeof(T).Name == "EntityTypeMongoDao")
                {
                    obj = versionDetail.EntityTypes.Cast<T>().ToList();

                }
                if (typeof(T).Name == "EntityTypeAttributeRelationMongoDao")
                {
                    obj = versionDetail.EntityTypeAttributeRelation.Cast<T>().ToList();

                }
            }
            catch
            {
                // ignored
            }

            return obj;
        }

        /// <summary>
        /// Get EntityType Relation based on EntitytypeId
        /// </summary>
        public List<EntityTypeAttributeRelationMongoDao> GetEntityTypeRelationById(string collectionName, int entityTypeId, int versionId)
        {
            var result = new List<EntityTypeAttributeRelationMongoDao>();
            try
            {
                var context = ERMMongoContext.Create(base.ConnectionStringRepository);
                var versionWithBuilderTask = context.MetadataVersion(collectionName)
                 .Find(Builders<MetadataVersionMongoDao>.Filter.Eq<int>(a => a.VersionId, versionId)).FirstOrDefault();
                var planEntitesWithBuilder = versionWithBuilderTask;
                result = planEntitesWithBuilder.EntityTypeAttributeRelation.Where(a => a.EntityTypeID == entityTypeId).ToList();

                //EntityTypeMongoDao entityTypeObj = new EntityTypeMongoDao();
                //entityTypeObj.DbObjectId = ObjectId.GenerateNewId();
                //entityTypeObj.EntityTypeId = PersistenceManager.Instance.MetadataRepository.GetMaxId<EntityTypeMongoDao>();
                //entityTypeObj.Caption = GenRandomFirstName() + " " + GenRandomLastName();
                //entityTypeObj.ShortDescription = "Short " + GenRandomFirstName();
                //List<EntityTypeMongoDao> lst = new List<EntityTypeMongoDao>();
                //lst.Add(entityTypeObj);
                ////int lastSavedID = SaveObject<EntityTypeMongoDao>(entityTypeObj, "version1");
                ////bool isListSaved = SaveObject<EntityTypeMongoDao>(lst, "version1");

            }
            catch
            {
                // ignored
            }

            return result;

        }

        /// <summary>
        /// Get current metadata version details
        /// </summary>
        /// <returns>MetadataVersionMongoDao</returns>
        public MetadataVersionMongoDao GetCurrentVersion()
        {
            var currentVersion = new MetadataVersionMongoDao();
            try
            {
                var context = ERMMongoContext.Create(base.ConnectionStringRepository);
                currentVersion = context.MetadataVersion("version1")
                .Find(Builders<MetadataVersionMongoDao>.Filter.Eq<int>(a => a.VersionId, 1)).FirstOrDefault();
            }
            catch
            {
                // ignored
            }
            return currentVersion;
        }

        /// <summary>
        /// Save metadata configuration in mongodb
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="saveObj"></param>
        /// <param name="versionName"></param>
        /// <returns>int</returns>
        public int SaveObject<T>(T saveObj, string versionName)
        {
            var lastId = 0;

            try
            {

                var context = ERMMongoContext.Create(base.ConnectionStringRepository); var metadataVersion = GetCurrentVersion();

                if (typeof(T).Name == "EntityTypeMongoDao" && saveObj != null)
                {
                    var newObj = saveObj as EntityTypeMongoDao; var existType = metadataVersion.EntityTypes.FirstOrDefault(a => newObj != null && a.EntityTypeId == newObj.EntityTypeId);
                    if (existType == null)
                    {
                        var filter = Builders<MetadataVersionMongoDao>.Filter.Eq(p => p.VersionId, 1); var updateset = Update.AddToSet("EntityTypes", newObj.ToBsonDocument());
                        var result1 = context.MetadataVersion("version1").UpdateOne(filter, updateset.ToBsonDocument());
                    }
                    else
                    {
                        var filter = Builders<MetadataVersionMongoDao>.Filter.Eq(p => p.VersionId, 1);
                        {
                            var type = metadataVersion.EntityTypes.FirstOrDefault(f => f.EntityTypeId == existType.EntityTypeId);
                            if (newObj != null)
                            {
                                if (type != null)
                                {
                                    type.ShortDescription = newObj.ShortDescription; type.Category = newObj.Category; type.ColorCode = newObj.ColorCode; type.Caption = newObj.Caption; type.Description = newObj.Description; type.IsAssociate = newObj.IsAssociate; type.IsRootLevel = newObj.IsRootLevel; type.ModuleCaption = newObj.ModuleCaption; type.ModuleID = newObj.ModuleID;
                                }
                            }
                        }
                        context.MetadataVersion("version1").ReplaceOne(filter, metadataVersion);
                    }

                    if (newObj != null) lastId = newObj.EntityTypeId;
                }
            }
            catch
            {
                // ignored
            }

            return lastId;
        }

        /// <summary>
        /// Save metadata configuration in mongodb from List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listObj"></param>
        /// <param name="versionName"></param>
        public bool SaveObject<T>(List<T> listObj, string versionName)
        {
            try
            {
                //Loop through and save use SaveObject<T>(T saveObj, string versionName)
                foreach (var obj in listObj)
                {
                    SaveObject<T>(obj, versionName);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete single MongoDb Object from the Sub document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deleteObj"></param>
        /// <param name="versionName"></param>
        /// <returns></returns>
        public bool DeleteObject<T>(T deleteObj, string versionName)
        {
            try
            {

                var context = ERMMongoContext.Create(base.ConnectionStringRepository); var metadataVersion = GetCurrentVersion();
                if (typeof(T).Name != "EntityTypeMongoDao" || deleteObj == null) return true;
                var mongoObj = deleteObj as EntityTypeMongoDao;
                var existType = metadataVersion.EntityTypes.FirstOrDefault(a => mongoObj != null && a.EntityTypeId == mongoObj.EntityTypeId);
                var filter = Builders<MetadataVersionMongoDao>.Filter.Eq(p => p.VersionId, 1);
                {
                    var type = metadataVersion.EntityTypes.FirstOrDefault(f => existType != null && f.EntityTypeId == existType.EntityTypeId);
                    //Removing from the collection and save it again
                    metadataVersion.EntityTypes.Remove(type);
                }
                context.MetadataVersion("version1").ReplaceOne(filter, metadataVersion);

                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Delete single MongoDb Object from the Sub document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deleteArr"></param>
        /// <param name="versionName"></param>
        /// <returns></returns>
        public bool DeleteObject<T>(List<T> deleteArr, string versionName)
        {
            try
            {
                //Loop through and delete use DeleteObject<T>(T deleteObj, string versionName)
                foreach (var obj in deleteArr)
                {
                    DeleteObject<T>(obj, versionName);
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Get EntityType Relation based on EntitytypeId
        /// </summary>
        public List<EntityTypeAttributeRelationMongoDao> GetEntityTypeRelationByIdMatch(string collectionName, int entityTypeId, int versionId)
        {
            var result = new List<EntityTypeAttributeRelationMongoDao>();
            try
            {
                var context = ERMMongoContext.Create(base.ConnectionStringRepository);
                var aggregate1 = context.MetadataVersion(collectionName).Aggregate()
               .Match(new BsonDocument { { "VersionId", versionId } })
               .Unwind(x => x.EntityTypeAttributeRelation)
               .Match(new BsonDocument { { "EntityTypeAttributeRelation.EntityTypeID", new BsonDocument { { "$eq", entityTypeId } } } })
               .Group(new BsonDocument { { "_id", "$_id" }, { "list", new BsonDocument { { "$push", "$EntityTypeAttributeRelation" } } } }).FirstOrDefault();
                var dimVal = aggregate1["list"];
                result = BsonSerializer.Deserialize<List<EntityTypeAttributeRelationMongoDao>>(dimVal.ToJson());
            }
            catch
            {
                // ignored
            }

            return result;

        }


    }
}
