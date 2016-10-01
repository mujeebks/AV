using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AVD.MongoDataAccessLayer.Models;
using AVD.MongoDataAccessLayer.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDBUnitTest
{
    [TestClass]
    public class MongoDbTest
    {
        void InitializeAllDevelopmentInstances()
        {

            //intialize all Development tenants and automapper mappings and all
            //this is should be called when we want to perform test cases with respect to entityFramework

        }

        [TestMethod]
        public void Mongodbtest()
        {

            IConfigurationRepository repo = new ConfigurationRepository(new WebConfigConnectionStringRepository());
            IList<EntityMongoDao> loadtestsInPeriod = repo.GetEntities(1, 100);
            List<EntityMongoDao> toBeInserted = new List<EntityMongoDao>();
            List<EntityMongoDao> toBeUpdated = new List<EntityMongoDao>();

            List<AttributeDataMongoDao> attrDao = new List<AttributeDataMongoDao>();
            AttributeDataMongoDao attr1 = new AttributeDataMongoDao { DbObjectId = MongoDB.Bson.ObjectId.GenerateNewId(), AttributeID = 1, Caption = "Fiscal Year", Value = "1", ValueCaption = "2016" };
            AttributeDataMongoDao attr2 = new AttributeDataMongoDao { DbObjectId = MongoDB.Bson.ObjectId.GenerateNewId(), AttributeID = 2, Caption = "Country", Value = "2", ValueCaption = "India" };
            attrDao.Add(attr1);
            attrDao.Add(attr2);

            int entitySize = 10000;

            for (int i = 0; i <= entitySize - 1; i++)
            {

                EntityMongoDao ltNewOne = new EntityMongoDao
                {
                    EntityId = GetRandomNumber(1000, 1000000),
                    Name = GenRandomFirstName() + " " + GenRandomLastName(),
                    CreationDate = DateTime.Now,
                    AttributeData = attrDao,
                    TypeName = GenRandomEntityTypeName(),
                    TypeId = GenRandomEntityType()
                };
                EntityMongoDao ltNewTwo = new EntityMongoDao
                {
                    EntityId = GetRandomNumber(1000, 1000000),
                    Name = GenRandomFirstName() + " " + GenRandomLastName(),
                    CreationDate = DateTime.Now,
                    AttributeData = attrDao,
                    TypeName = GenRandomEntityTypeName(),
                    TypeId = GenRandomEntityType()
                };

                toBeInserted.Add(ltNewOne);
                toBeInserted.Add(ltNewTwo);

            }


            EntityMongoDao ltUpdOne = new EntityMongoDao { EntityId = toBeInserted[0].EntityId, Name = GenRandomFirstName() + " " + GenRandomLastName(), CreationDate = DateTime.Now.AddDays(1), AttributeData = attrDao };
            toBeUpdated.Add(ltUpdOne);
            repo.AddOrUpdateLoadEntites(toBeInserted, toBeUpdated);

            Assert.IsNotNull(toBeInserted[0].DbObjectId); // Test that record saved properly
        }

        [TestMethod]
        public void TestSelectAllNoFilter()
        {

            ERMMongoContext context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
            Task<List<EntityMongoDao>> dbEntitiesTask = context.Entities.Find(x => true).ToListAsync();
            Task.WaitAll(dbEntitiesTask);
            List<EntityMongoDao> dbEntites = dbEntitiesTask.Result;

            foreach (EntityMongoDao eng in dbEntites)
            {
                Debug.WriteLine(eng.Name);
            }

            Assert.AreNotEqual(0, dbEntites.Count); // Test that find worked properly or not
        }

        [TestMethod]
        public void TestWithMatchGroup()
        {

            ERMMongoContext context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
            Task<List<EntityMongoDao>> dbEntitiesTask = context.Entities.Find(x => true).ToListAsync();
            Task.WaitAll(dbEntitiesTask);

            var collection = context.MetadataVersion("version1");
            var aggregate = collection.Aggregate()
                .Match(new BsonDocument { { "VersionId", 1 } })
                .Unwind(x => x.EntityTypes)
                .Match(new BsonDocument { { "EntityTypes.EntityTypeId", new BsonDocument { { "$gt", 138 } } } })
                .Group(new BsonDocument { { "_id", "$_id" }, { "list", new BsonDocument { { "$push", "$EntityTypes" } } } });
            var results = aggregate.ToList();

            var aggregate1 = collection.Aggregate()
                .Match(new BsonDocument { { "VersionId", 1 } })
                .Unwind(x => x.EntityTypes)
                .Match(new BsonDocument { { "EntityTypes.EntityTypeId", new BsonDocument { { "$gt", 1 } } } })
                .Group(new BsonDocument { { "_id", "$_id" }, { "list", new BsonDocument { { "$push", "$EntityTypes" } } } }).FirstOrDefault();

            BsonValue dimVal1 = aggregate1["list"];
            List<EntityTypeMongoDao> d = BsonSerializer.Deserialize<List<EntityTypeMongoDao>>(dimVal1.ToJson());

            var aggregate2 = context.MetadataVersion("version1").Aggregate()
            .Match(new BsonDocument { { "VersionId", 1 } })
            .Unwind(x => x.EntityTypeAttributeRelation)
            .Match(new BsonDocument { { "EntityTypeAttributeRelation.EntityTypeID", new BsonDocument { { "$eq", 138 } } } })
            .Group(new BsonDocument { { "_id", "$_id" }, { "list", new BsonDocument { { "$push", "$EntityTypeAttributeRelation" } } } }).FirstOrDefault();
            BsonValue dimVal = aggregate2["list"];
            var result2 = BsonSerializer.Deserialize<List<EntityTypeAttributeRelationMongoDao>>(dimVal.ToJson());



        }

        [TestMethod]
        public void TestSelectWithWhereClause()
        {

            ERMMongoContext context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
            Task<List<EntityMongoDao>> planEntitiesTask = context.Entities.Find(a => a.TypeName == "Plan").ToListAsync();
            Task.WaitAll(planEntitiesTask);
            List<EntityMongoDao> planEntites = planEntitiesTask.Result;

            foreach (EntityMongoDao agent in planEntites)
            {
                Debug.WriteLine(agent.TypeName);
            }

            int entityID = 96;
            Task<EntityMongoDao> singleAgentByIdTask = context.Entities.Find(a => a.EntityId == entityID).SingleOrDefaultAsync();
            Task.WaitAll(singleAgentByIdTask);
            EntityMongoDao singleEntityById = singleAgentByIdTask.Result;
            if (singleEntityById != null)
                Debug.WriteLine(singleEntityById.TypeName);


            var findFluentCol = context.MetadataVersion("version1").Find(f => f.VersionId == 1 && f.EntityTypeAttributeRelation.Any(fb => fb.EntityTypeID == 63)).ToList();
            var findFluent = context.MetadataVersion("version1").Find(Builders<MetadataVersionMongoDao>.Filter.ElemMatch(foo => foo.EntityTypeAttributeRelation, foobar => foobar.EntityTypeID == 63)).ToList();
            var condition = Builders<MetadataVersionMongoDao>.Filter.Eq(p => p.VersionId, 1);
            var fields = Builders<MetadataVersionMongoDao>.Projection.Include(p => p.EntityTypeAttributeRelation);
            var result = context.MetadataVersion("version1").Find(condition).Project<EntityTypeAttributeRelationMongoDao>(fields).ToList();

            var dateQueryBuilder = Builders<EntityTypeAttributeRelationMongoDao>.Filter;
            var startDateBeforeSearchStartFilter = dateQueryBuilder.Eq<int>(l => l.EntityTypeID, 1);
            var dateQueryBuilder1 = Builders<MetadataVersionMongoDao>.Filter;
            var startDateBeforeSearchStartFilter1 = dateQueryBuilder1.ElemMatch(p => p.EntityTypeAttributeRelation, startDateBeforeSearchStartFilter);
            var mongoDbLoadtestsInSearchPeriod = context.MetadataVersion("version1").Find(startDateBeforeSearchStartFilter1)
            .ToList();

            Task<List<EntityMongoDao>> planEntitiesWithBuilderTask = context.Entities
                .Find(Builders<EntityMongoDao>.Filter.Eq<string>(a => a.TypeName, "Plan")).ToListAsync();
            Task.WaitAll(planEntitiesWithBuilderTask);
            List<EntityMongoDao> planEntitesWithBuilder = planEntitiesWithBuilderTask.Result;

            foreach (EntityMongoDao entity in planEntitesWithBuilder)
            {
                Debug.WriteLine(entity.TypeName);
            }

            Assert.AreNotEqual(0, planEntitesWithBuilder.Count); // Test that where clause worked properly or not
        }

        [TestMethod]
        public void TestEntityInsertion()
        {

            ERMMongoContext context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
            IConfigurationRepository repo = new ConfigurationRepository(new WebConfigConnectionStringRepository());
            List<AttributeDataMongoDao> attrDao = new List<AttributeDataMongoDao>();
            attrDao.Add(new AttributeDataMongoDao { AttributeID = 1, Caption = "Fiscal Year", Value = "1", ValueCaption = "2016" });
            attrDao.Add(new AttributeDataMongoDao { AttributeID = 2, Caption = "Country", Value = "2", ValueCaption = "India" });
            EntityMongoDao newEntity = new EntityMongoDao()
            {
                EntityId = GetRandomNumber(1, 100),
                Name = GenRandomFirstName() + " " + GenRandomLastName(),
                CreationDate = DateTime.Now,
                AttributeData = attrDao,
                TypeName = "Plan",
                TypeId = 1
            };
            Task insertionTask = context.Entities.InsertOneAsync(newEntity);
            Task.WaitAll(insertionTask);

            Assert.IsNotNull(newEntity.DbObjectId); // Test that insertion happened properly or not
        }

        [TestMethod]
        public void TestReplacement()
        {
            string beforeReplacement = "", afterReplacement = "";

            ERMMongoContext context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
            IConfigurationRepository repo = new ConfigurationRepository(new WebConfigConnectionStringRepository());
            int existingEntityId = 96;
            Task<EntityMongoDao> entityTask = context.Entities.Find(p => p.EntityId == existingEntityId).SingleOrDefaultAsync();
            Task.WaitAll(entityTask);
            EntityMongoDao entity = entityTask.Result;
            if (entity != null)
            {
                beforeReplacement = entity.Name;
                entity.Name = GenRandomFirstName() + " " + GenRandomLastName();
                entity.UniqueKey = Convert.ToString(GetRandomNumber(1, 100));
                Task<EntityMongoDao> replacementTask = context.Entities.FindOneAndReplaceAsync(p => p.DbObjectId == entity.DbObjectId, entity);
            }

            entityTask = context.Entities.Find(p => p.EntityId == existingEntityId).SingleOrDefaultAsync();
            entity = entityTask.Result;
            if (entityTask != null)
            {
                afterReplacement = entity.Name;
            }

            Assert.AreNotEqual(beforeReplacement, afterReplacement); // Test replacement worked properly or not
        }

        [TestMethod]
        public void TestUpdate()
        {

            ERMMongoContext context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
            int existingEntityID = 96;
            IConfigurationRepository repo = new ConfigurationRepository(new WebConfigConnectionStringRepository());
            UpdateDefinition<EntityMongoDao> entityUpdateDefinition = Builders<EntityMongoDao>.Update.Set<string>(a => a.UniqueKey, Convert.ToString(GetRandomNumber(1, 100)));
            Task<EntityMongoDao> replacementTask = context.Entities.FindOneAndUpdateAsync(a => a.EntityId == existingEntityID, entityUpdateDefinition);
            Task.WaitAll(replacementTask);

            EntityMongoDao replacementResult = replacementTask.Result;
            Debug.WriteLine(string.Concat(replacementResult.TypeName, ", ", replacementResult.EntityId));

            Assert.AreNotEqual(0, replacementResult.EntityId); // Test update worked properly or not
        }

        [TestMethod]
        public void Seed()
        {

            ERMMongoContext context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
            IConfigurationRepository repo = new ConfigurationRepository(new WebConfigConnectionStringRepository());

            List<EntityMongoDao> entites = new List<EntityMongoDao>();
            List<AttributeDataMongoDao> attrDao = new List<AttributeDataMongoDao>();
            attrDao.Add(new AttributeDataMongoDao { AttributeID = 1, Caption = "Fiscal Year", Value = "1", ValueCaption = "2016" });
            attrDao.Add(new AttributeDataMongoDao { AttributeID = 2, Caption = "Country", Value = "2", ValueCaption = "India" });

            EntityMongoDao newEntity = new EntityMongoDao()
            {
                DbObjectId = MongoDB.Bson.ObjectId.GenerateNewId(), //test to get new objectid
                EntityId = GetRandomNumber(1, 100),
                Name = GenRandomFirstName() + " " + GenRandomLastName(),
                CreationDate = DateTime.Now,
                AttributeData = attrDao,
                TypeName = "Plan",
                TypeId = 1
            };
            entites.Add(newEntity);

            newEntity = new EntityMongoDao()
            {
                DbObjectId = MongoDB.Bson.ObjectId.GenerateNewId(),
                EntityId = GetRandomNumber(1, 100),
                Name = GenRandomFirstName() + " " + GenRandomLastName(),
                CreationDate = DateTime.Now,
                AttributeData = attrDao,
                TypeName = "Plan",
                TypeId = 1
            };
            entites.Add(newEntity);

            Task addManyEntitiesTask = context.Entities.InsertManyAsync(entites);
            Task.WaitAll(addManyEntitiesTask);

            Assert.IsNotNull(entites[0].DbObjectId); // Test Seed worked properly or not
        }

        [TestMethod]
        public void MetadataSetup()
        {
            ERMMongoContext context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
            IConfigurationRepository repo = new ConfigurationRepository(new WebConfigConnectionStringRepository());

            InitializeAllDevelopmentInstances(); // initialize the persistance layer and nhibernate engine 

            MetadataVersionMongoDao newEntity = new MetadataVersionMongoDao()
            {
                VersionId = 1,
                VersionName = "Version1",
                CreatedDate = DateTime.Now,
                EntityTypeHierarchy = context.GetCollectionBsonDocument("EntityTypeHierarchy") as List<EntityTypeHierarchyMongoDao>,
                EntityTypeAttributeRelation = context.GetCollectionBsonDocument("EntityTypeAttributeRelation") as List<EntityTypeAttributeRelationMongoDao>,
                EntityFeatures = context.GetCollectionBsonDocument("EntityFeatures") as List<EntitytypeFeatureMongoDao>,
                Attributes = context.GetCollectionBsonDocument("Attributes") as List<AttributeMongoDao>,
                Features = context.GetCollectionBsonDocument("Features") as List<FeatureMongoDao>,
                Modules = context.GetCollectionBsonDocument("Modules") as List<ModuleMongoDao>,
                Options = context.GetCollectionBsonDocument("Options") as List<OptionMongoDao>,
                TreeLevels = context.GetCollectionBsonDocument("TreeLevels") as List<TreeLevelMongoDao>,
                TreeNodes = context.GetCollectionBsonDocument("TreeNodes") as List<TreeNodeMongoDao>

            };
            newEntity.EntityTypes = context.GetCollectionBsonDocument("EntityTypes") as List<EntityTypeMongoDao>;
            Task insertionTask = context.MetadataVersion("version1").InsertOneAsync(newEntity);
            Task.WaitAll(insertionTask);


        }

        [TestMethod]
        public void MetadataVersionDetails()
        {

            ERMMongoContext context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
            Task<MetadataVersionMongoDao> versionDetailTask = context.MetadataVersion("version1").Find(a => a.VersionId == 1).SingleOrDefaultAsync();
            Task.WaitAll(versionDetailTask);
            MetadataVersionMongoDao planEntites = versionDetailTask.Result;

        }

        [TestMethod]
        public void SeacrhWithSortPropertyName()
        {
            int pageNo = 1, pageSize = 20;
            int skipCount = (pageNo - 1) * pageSize;
            string propertyInfo = GetPropertyInfo(new EntityMongoDao(), u => u.EntityId).Name;

            var builder = Builders<EntityMongoDao>.Sort;
            var sort = builder.Ascending(propertyInfo);

            ERMMongoContext context = ERMMongoContext.Create(new WebConfigConnectionStringRepository());
            List<EntityMongoDao> mongoDbLoadEntites1 = context.Entities.Find(x => true).Sort(sort).SortBy(x => x.Name).ThenByDescending(x => x.EntityId).ThenByDescending(x => x.CreationDate).Skip(skipCount).Limit(pageSize)
                .ToList();
            var mongoDbLoadEntites = context.Entities.Find(x => true).Sort(sort).Skip(skipCount).Limit(pageSize)
               .ToList();
        }


        public PropertyInfo GetPropertyInfo<TSource, TProperty>(
    TSource source,
    Expression<Func<TSource, TProperty>> propertyLambda)
        {

            PropertyInfo propInfo = null;
            try
            {

                Type type = typeof(TSource);
                MemberExpression member = propertyLambda.Body as MemberExpression;
                if (member == null)
                {
                    throw new ArgumentException(
                        $"Expression '{propertyLambda.ToString()}' refers to a method, not a property.");
                }


                propInfo = member.Member as PropertyInfo;
                if (propInfo == null)
                {
                    throw new ArgumentException(
                        $"Expression '{propertyLambda.ToString()}' refers to a field, not a property.");
                }

                if (propInfo.ReflectedType != null && (type != propInfo.ReflectedType &&
                                                       !type.IsSubclassOf(propInfo.ReflectedType)))
                {
                    throw new ArgumentException(
                        $"Expresion '{propertyLambda.ToString()}' refers to a property that is not from type {type}.");
                }
            }
            catch
            {
                // ignored
            }

            return propInfo;
        }


        #region testUtilityMethods

        public static Random rnd = new Random();
        public string GenRandomLastName()
        {
            List<string> lst = new List<string>();
            string str = string.Empty;
            lst.Add("Smith");
            lst.Add("Johnson");
            lst.Add("Williams");
            lst.Add("Jones");
            lst.Add("Brown");
            lst.Add("Davis");
            lst.Add("Miller");
            lst.Add("Wilson");
            lst.Add("Moore");
            lst.Add("Taylor");
            lst.Add("Anderson");
            lst.Add("Thomas");
            lst.Add("Jackson");
            lst.Add("White");
            lst.Add("Harris");
            lst.Add("Martin");
            lst.Add("Thompson");
            lst.Add("Garcia");
            lst.Add("Martinez");
            lst.Add("Robinson");
            lst.Add("Clark");
            lst.Add("Rodriguez");
            lst.Add("Lewis");
            lst.Add("Lee");
            lst.Add("Walker");
            lst.Add("Hall");
            lst.Add("Allen");
            lst.Add("Young");
            lst.Add("Hernandez");
            lst.Add("King");
            lst.Add("Wright");
            lst.Add("Lopez");
            lst.Add("Hill");
            lst.Add("Scott");
            lst.Add("Green");
            lst.Add("Adams");
            lst.Add("Baker");
            lst.Add("Gonzalez");
            lst.Add("Nelson");
            lst.Add("Carter");
            lst.Add("Mitchell");
            lst.Add("Perez");
            lst.Add("Roberts");
            lst.Add("Turner");
            lst.Add("Phillips");
            lst.Add("Campbell");
            lst.Add("Parker");
            lst.Add("Evans");
            lst.Add("Edwards");
            lst.Add("Collins");
            lst.Add("Stewart");
            lst.Add("Sanchez");
            lst.Add("Morris");
            lst.Add("Rogers");
            lst.Add("Reed");
            lst.Add("Cook");
            lst.Add("Morgan");
            lst.Add("Bell");
            lst.Add("Murphy");
            lst.Add("Bailey");
            lst.Add("Rivera");
            lst.Add("Cooper");
            lst.Add("Richardson");
            lst.Add("Cox");
            lst.Add("Howard");
            lst.Add("Ward");
            lst.Add("Torres");
            lst.Add("Peterson");
            lst.Add("Gray");
            lst.Add("Ramirez");
            lst.Add("James");
            lst.Add("Watson");
            lst.Add("Brooks");
            lst.Add("Kelly");
            lst.Add("Sanders");
            lst.Add("Price");
            lst.Add("Bennett");
            lst.Add("Wood");
            lst.Add("Barnes");
            lst.Add("Ross");
            lst.Add("Henderson");
            lst.Add("Coleman");
            lst.Add("Jenkins");
            lst.Add("Perry");
            lst.Add("Powell");
            lst.Add("Long");
            lst.Add("Patterson");
            lst.Add("Hughes");
            lst.Add("Flores");
            lst.Add("Washington");
            lst.Add("Butler");
            lst.Add("Simmons");
            lst.Add("Foster");
            lst.Add("Gonzales");
            lst.Add("Bryant");
            lst.Add("Alexander");
            lst.Add("Russell");
            lst.Add("Griffin");
            lst.Add("Diaz");
            lst.Add("Hayes");

            str = lst.OrderBy(xx => rnd.Next()).First();
            return str;
        }
        public string GenRandomFirstName()
        {
            List<string> lst = new List<string>();
            string str = string.Empty;
            lst.Add("Aiden");
            lst.Add("Jackson");
            lst.Add("Mason");
            lst.Add("Liam");
            lst.Add("Jacob");
            lst.Add("Jayden");
            lst.Add("Ethan");
            lst.Add("Noah");
            lst.Add("Lucas");
            lst.Add("Logan");
            lst.Add("Caleb");
            lst.Add("Caden");
            lst.Add("Jack");
            lst.Add("Ryan");
            lst.Add("Connor");
            lst.Add("Michael");
            lst.Add("Elijah");
            lst.Add("Brayden");
            lst.Add("Benjamin");
            lst.Add("Nicholas");
            lst.Add("Alexander");
            lst.Add("William");
            lst.Add("Matthew");
            lst.Add("James");
            lst.Add("Landon");
            lst.Add("Nathan");
            lst.Add("Dylan");
            lst.Add("Evan");
            lst.Add("Luke");
            lst.Add("Andrew");
            lst.Add("Gabriel");
            lst.Add("Gavin");
            lst.Add("Joshua");
            lst.Add("Owen");
            lst.Add("Daniel");
            lst.Add("Carter");
            lst.Add("Tyler");
            lst.Add("Cameron");
            lst.Add("Christian");
            lst.Add("Wyatt");
            lst.Add("Henry");
            lst.Add("Eli");
            lst.Add("Joseph");
            lst.Add("Max");
            lst.Add("Isaac");
            lst.Add("Samuel");
            lst.Add("Anthony");
            lst.Add("Grayson");
            lst.Add("Zachary");
            lst.Add("David");
            lst.Add("Christopher");
            lst.Add("John");
            lst.Add("Isaiah");
            lst.Add("Levi");
            lst.Add("Jonathan");
            lst.Add("Oliver");
            lst.Add("Chase");
            lst.Add("Cooper");
            lst.Add("Tristan");
            lst.Add("Colton");
            lst.Add("Austin");
            lst.Add("Colin");
            lst.Add("Charlie");
            lst.Add("Dominic");
            lst.Add("Parker");
            lst.Add("Hunter");
            lst.Add("Thomas");
            lst.Add("Alex");
            lst.Add("Ian");
            lst.Add("Jordan");
            lst.Add("Cole");
            lst.Add("Julian");
            lst.Add("Aaron");
            lst.Add("Carson");
            lst.Add("Miles");
            lst.Add("Blake");
            lst.Add("Brody");
            lst.Add("Adam");
            lst.Add("Sebastian");
            lst.Add("Adrian");
            lst.Add("Nolan");
            lst.Add("Sean");
            lst.Add("Riley");
            lst.Add("Bentley");
            lst.Add("Xavier");
            lst.Add("Hayden");
            lst.Add("Jeremiah");
            lst.Add("Jason");
            lst.Add("Jake");
            lst.Add("Asher");
            lst.Add("Micah");
            lst.Add("Jace");
            lst.Add("Brandon");
            lst.Add("Josiah");
            lst.Add("Hudson");
            lst.Add("Nathaniel");
            lst.Add("Bryson");
            lst.Add("Ryder");
            lst.Add("Justin");
            lst.Add("Bryce");

            //—————female

            lst.Add("Sophia");
            lst.Add("Emma");
            lst.Add("Isabella");
            lst.Add("Olivia");
            lst.Add("Ava");
            lst.Add("Lily");
            lst.Add("Chloe");
            lst.Add("Madison");
            lst.Add("Emily");
            lst.Add("Abigail");
            lst.Add("Addison");
            lst.Add("Mia");
            lst.Add("Madelyn");
            lst.Add("Ella");
            lst.Add("Hailey");
            lst.Add("Kaylee");
            lst.Add("Avery");
            lst.Add("Kaitlyn");
            lst.Add("Riley");
            lst.Add("Aubrey");
            lst.Add("Brooklyn");
            lst.Add("Peyton");
            lst.Add("Layla");
            lst.Add("Hannah");
            lst.Add("Charlotte");
            lst.Add("Bella");
            lst.Add("Natalie");
            lst.Add("Sarah");
            lst.Add("Grace");
            lst.Add("Amelia");
            lst.Add("Kylie");
            lst.Add("Arianna");
            lst.Add("Anna");
            lst.Add("Elizabeth");
            lst.Add("Sophie");
            lst.Add("Claire");
            lst.Add("Lila");
            lst.Add("Aaliyah");
            lst.Add("Gabriella");
            lst.Add("Elise");
            lst.Add("Lillian");
            lst.Add("Samantha");
            lst.Add("Makayla");
            lst.Add("Audrey");
            lst.Add("Alyssa");
            lst.Add("Ellie");
            lst.Add("Alexis");
            lst.Add("Isabelle");
            lst.Add("Savannah");
            lst.Add("Evelyn");
            lst.Add("Leah");
            lst.Add("Keira");
            lst.Add("Allison");
            lst.Add("Maya");
            lst.Add("Lucy");
            lst.Add("Sydney");
            lst.Add("Taylor");
            lst.Add("Molly");
            lst.Add("Lauren");
            lst.Add("Harper");
            lst.Add("Scarlett");
            lst.Add("Brianna");
            lst.Add("Victoria");
            lst.Add("Liliana");
            lst.Add("Aria");
            lst.Add("Kayla");
            lst.Add("Annabelle");
            lst.Add("Gianna");
            lst.Add("Kennedy");
            lst.Add("Stella");
            lst.Add("Reagan");
            lst.Add("Julia");
            lst.Add("Bailey");
            lst.Add("Alexandra");
            lst.Add("Jordyn");
            lst.Add("Nora");
            lst.Add("Carolin");
            lst.Add("Mackenzie");
            lst.Add("Jasmine");
            lst.Add("Jocelyn");
            lst.Add("Kendall");
            lst.Add("Morgan");
            lst.Add("Nevaeh");
            lst.Add("Maria");
            lst.Add("Eva");
            lst.Add("Juliana");
            lst.Add("Abby");
            lst.Add("Alexa");
            lst.Add("Summer");
            lst.Add("Brooke");
            lst.Add("Penelope");
            lst.Add("Violet");
            lst.Add("Kate");
            lst.Add("Hadley");
            lst.Add("Ashlyn");
            lst.Add("Sadie");
            lst.Add("Paige");
            lst.Add("Katherine");
            lst.Add("Sienna");
            lst.Add("Piper");

            str = lst.OrderBy(xx => rnd.Next()).First();
            return str;
        }
        public string GenRandomEntityTypeName()
        {
            List<string> lst = new List<string>();
            string str = string.Empty;
            lst.Add("Plan");
            lst.Add("Milestone");
            lst.Add("Task");
            lst.Add("Asset");
            lst.Add("CostCenter");
            lst.Add("Objectives");
            lst.Add("Calander");
            str = lst.OrderBy(xx => rnd.Next()).First();
            return str;
        }
        public int GenRandomEntityType()
        {
            List<int> lst = new List<int>();
            int nmbr = 1;
            lst.Add(1);
            lst.Add(2);
            lst.Add(3);
            lst.Add(4);
            lst.Add(5);
            lst.Add(6);
            lst.Add(7);
            nmbr = lst.OrderBy(xx => rnd.Next()).First();
            return nmbr;
        }

        ///<summary>
        ///GetRandomNumber
        ///</summary>
        private static readonly Random Getrandom = new Random();
        private static readonly object SyncLock = new object();
        public int GetRandomNumber(int min, int max)
        {
            lock (SyncLock)
            { // synchronize
                return Getrandom.Next(min, max);
            }
        }

        #endregion
    }
}
