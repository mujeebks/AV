using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Common.Caching
{
    public class InMemoryAndPersistentCacheManager : IPersistentCacheManager
    {
        private ICachingManager inMemoryCache;
        private IPersistentCacheManager persistentCache;

        public InMemoryAndPersistentCacheManager(IPersistentCacheManager persistentCacheManager, ICachingManager cachingManager)
        {
            this.inMemoryCache = cachingManager;
            this.persistentCache = persistentCacheManager;
        }

        public string GetHashCodeKey(params string[] inputs)
        {
            return new CachingManager().GetHashCodeKey(inputs);
        }

        /// <summary>
        /// Adds an item with a default timeout.
        /// </summary>
        public bool Add<T>(string key, T value) where T : class
        {
            Task.Factory.StartNew(() =>
            {
                if (persistentCache != null)
                    persistentCache.Add(key, value);
            });

            return inMemoryCache.Add(key, value, true);
        }

        public bool Add<T>(string key, T value, int expirySeconds) where T : class
        {
            return Add(key, value, new TimeSpan(0, 0, 0, expirySeconds));
        }

        public bool Add<T>(string key, T value, TimeSpan? expiry) where T : class
        {
            Task.Factory.StartNew(() =>
            {
                if (persistentCache != null)
                    persistentCache.Add(key, value, expiry);
            });

            if(expiry != null)
                return inMemoryCache.Add(key, value, (int)expiry.Value.TotalSeconds, true);
            else
                return inMemoryCache.Add(key, value, true);
        }

        public T Get<T>(string key) where T : class
        {
            T obj = inMemoryCache.Get<T>(key);

            if (obj == null && persistentCache != null)
                obj = persistentCache.Get<T>(key);

            return obj;
        }

        public bool Remove<T>(string key) where T : class
        {
            var res = inMemoryCache.Remove<T>(key);

            if (persistentCache != null)
            {
                res = res && persistentCache.Remove<T>(key);
            }

            return res;
        }

        public T GetOrAddToCache<T>(string key, Func<T> value, int? seconds) where T : class
        {
            var obj = Get<T>(key);
            if (obj == null)
            {
                obj = value();
                if (seconds != null)
                {
                    Add(key, obj, new TimeSpan(0, 0, 0, seconds.Value));
                }
                else
                {
                    Add(key, obj);
                }
            }

            return obj;
        }

        public bool AddWithSlidingExpiration<T>(string key, T value, TimeSpan timeSpan, bool overwrite = false) where T : class
        {
            // TODO: IMPLEMENT
            return Add(key, value, timeSpan);
        }
    }
}
