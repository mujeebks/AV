using System;
using System.Configuration;
using System.Linq;
using System.Text;
using AVD.Common;
using AVD.Common.Logging;
using AVD.Common;
using AVD.Common.Caching;
using AVD.Common.Caching;
using AVD.Common.Logging;
using AVD.Core.Shared;

namespace AVD.Core
{
    public sealed class ServiceLocator
    {
        private IDistributedDictionaryProvider distributedDictionaryProvider;
        private volatile IPersistentCacheManager persistentCache;
        private volatile ICachingManager cachingManager;

        private static volatile ServiceLocator instance;
        private static object syncRoot = new Object();
         
        public static ServiceLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ServiceLocator();
                    }
                }

                return instance;
            }
        }

        private ServiceLocator()
        {
            switch(ConfigurationManager.AppSettings["AVD.Common.Caching.Distributed"])
            {
                case "Redis":
                    var redis = RedisPersistentCacheManager.Instance;

                    if (!redis.IsConnected())
                    {
                        Logger.Instance.Error(GetType().Name, "ServiceLocator", "Could not load up cache - reverting to Db");
                        persistentCache = new DbCacheManager();
                    }

                    persistentCache = redis;
                    distributedDictionaryProvider = redis;

                    break;
                case "DBCache":
                default:
                    persistentCache = new DbCacheManager();
                    distributedDictionaryProvider = null; // not available.
                    break;
            }

            cachingManager = new CachingManager();
        }

        public static IPersistentCacheManager PersistentCache
        {
            get { return Instance.persistentCache; }
        }

        public static ICachingManager CachingManager
        {
            get { return Instance.cachingManager; }
        }

        /// <summary>
        /// Used for session management in a distributed/loadbalanced environment
        /// </summary>
        public static IDistributedDictionaryProvider DistributedDictionaryProvider
        {
            get { return Instance.distributedDictionaryProvider; }
        }
    }
}
