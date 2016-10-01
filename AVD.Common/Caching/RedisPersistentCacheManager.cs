using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVD.Common;
using AVD.Common.Caching;
using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.DataTypes;

namespace AVD.Common.Caching
{
    public interface IDistributedDictionaryProvider
    {
        IDictionary<TK, TV> GetDictionary<TK,TV>(string key);
    }

    public class RedisPersistentCacheManager : IPersistentCacheManager, IDistributedDictionaryProvider
    {

        private static volatile RedisPersistentCacheManager instance;
        private static object syncRoot = new Object();

        private volatile ConnectionMultiplexer connectionMultiplexer;

        private IDatabase GetDatabase()
        {
            return connectionMultiplexer.GetDatabase();
        }

        public static RedisPersistentCacheManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new RedisPersistentCacheManager();
                    }
                }

                return instance;
            }
        }

        private RedisPersistentCacheManager()
        {
            connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
        }

        public string GetHashCodeKey(params string[] inputs)
        {
            return  String.Join(",", inputs);
        }
        
        public bool Add<T>(string key, T value) where T: class
        {
            return Add(key, value, CachingManager.DefaultTimeOut);
        }

        /// <summary>
        /// Adds an item to the cache with the given expiry
        /// </summary>
        /// <remarks>Note that the expiry has not been implemented yet.</remarks>
        public bool Add<T>(string key, T value, TimeSpan? expiry) where T: class
        {
            key = typeof(T) + ":" + key;
            var sVal = ToStringForCache(value);
            return GetDatabase().StringSet(key, sVal);
        }

        public bool Add<T>(string key, T value, int expirySeconds) where T : class
        {
            return Add(key, value, new TimeSpan(0, 0, 0, expirySeconds));
        }

        public T GetOrAddToCache<T>(string key, Func<T> value, int? seconds) where T : class
        {
            if (GetDatabase().KeyExists(typeof (T) + ":" + key))
                return Get<T>(key);

            var val = value();
            bool result;
            if(seconds != null)
                result = Add(key, val, seconds.Value);
            else
                result = Add(key, val);

            if (!result)
            {
            }
            // log warn

            return val;
        }

        public bool AddWithSlidingExpiration<T>(string key, T value, TimeSpan timeSpan, bool overwrite = false) where T : class
        {
            key = typeof(T) + ":" + key;
            // todo: overwrite
            var sVal = ToStringForCache(value);
            return GetDatabase().StringSet(key, sVal, timeSpan);
        }

        private static string ToStringForCache(object value)
        {
            var sVal = JsonConvert.SerializeObject(value,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            return sVal;
        }

        public T Get<T>(string key) where T: class
        {
            key = typeof(T) + ":" + key;

            IDatabase db = connectionMultiplexer.GetDatabase();
            RedisValue cachedItem = db.StringGet(key);

            if (!cachedItem.HasValue)
                return null;

            var item = JsonConvert.DeserializeObject<T>(cachedItem);

            return item;
        }

        public bool Remove<T>(string key) where T: class
        {
            key = typeof (T) + ":" + key;

            return GetDatabase().KeyDelete(key);
        }

        public IDictionary<TK, TV> GetDictionary<TK,TV>(string key)
        {
            var redisTypeFactory = new RedisTypeFactory(connectionMultiplexer);

            var redisDictionary = redisTypeFactory.GetDictionary<TK, TV>(key);

            return redisDictionary;
        }

        public bool IsConnected()
        {
            return connectionMultiplexer.IsConnected;
        }
    }
}
