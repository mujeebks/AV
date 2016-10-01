using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.Common;
using AVD.Common.Caching;
using AVD.Common.Logging;
using AVD.DataAccessLayer.Repositories;
using Newtonsoft.Json;
using AVD.Core.Auth;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Models;

namespace AVD.Core.Shared
{
    public class DbCacheManager : IPersistentCacheManager
    {  
        public string GetHashCodeKey(params string[] inputs)
        {
            return ServiceLocator.CachingManager.GetHashCodeKey(inputs);
        }

        public bool Add<T>(string key, T value) where T : class
        {
            return Add(key, value, CachingManager.DefaultTimeOut);
        }

        public bool Add<T>(string key, T value, int expirySeconds) where T : class
        {
            return Add(key, value, new TimeSpan(0, 0, 0, expirySeconds));
        }

        /// <summary>
        /// Adds an item to the cache with the given expiry
        /// </summary>
        /// <remarks>Note that the expiry has not been implemented yet.</remarks>

        public bool Add<T>(string key, T value, TimeSpan? expiry) where T : class
        {
            int? expiryMinutes = null;

            key = typeof (T).Name + ":" + key;

            if (expiry != null)
            {
                expiryMinutes = Convert.ToInt32(expiry.Value.TotalMinutes);
            }

            int? userId = null;

            if (AuthManager.IsCurrentUserAuthenticated())
            {
                userId = AuthManager.GetCurrentUserId();
            }

            var cachedItem = new CachedItem
                {
                    UserId = userId,
                    CacheKey = key,
                    ExpiryMinutes = expiryMinutes,
                    Value = JsonConvert.SerializeObject(value, new JsonSerializerSettings{ NullValueHandling = NullValueHandling.Ignore})
                };

            using (var cachedItemRepository = new Repository<CachedItem>())
            {
                var c = cachedItemRepository.SingleOrDefault(t => t.CacheKey == key);

                if (c != null)
                {
                    c.UserId = userId;
                    c.Value = cachedItem.Value;
                    c.ExpiryMinutes = expiryMinutes;
                    cachedItem = c;
                }

                if (cachedItem.CachedItemId == 0)
                {
                    cachedItemRepository.InsertAndSave(cachedItem); // perf fix
                }
                else
                {
                    cachedItemRepository.Update(cachedItem);
                }

                cachedItemRepository.SaveChanges();
            }

            return cachedItem.CachedItemId != 0;
        }

        public T Get<T>(string key) where T : class
        {
            key = typeof (T).Name + ":" + key;

            T item;

            using (var cachedItemRepository = new Repository<CachedItem>())
            {
                var cachedItem = cachedItemRepository.SingleOrDefault(t => t.CacheKey == key);

                if (cachedItem == null)
                    return null;

                item = JsonConvert.DeserializeObject<T>(cachedItem.Value);

                if (cachedItem.ElapsedMinutes > cachedItem.ExpiryMinutes)
                {
                    Logger.InstanceVerbose.Info(typeof (DbCacheManager).FullName, "Get",
                        "Removed cached key due to expiry = " + key + "- " + item);

                    // Remove from cache
                    Remove<T>(key);
                    item = null;
                }
                else
                {
                    Logger.InstanceVerbose.Info(typeof (DbCacheManager).FullName, "Get",
                        "Retreiving from the DbCache key=" + key + "- " + item);
                }
            }
            return item;
        }

        public bool Remove<T>(string key) where T : class
        {
            key = typeof (T).Name + ":" + key;

            using (var cachedItemRepository = new Repository<CachedItem>())
            {
                var cachedItem = cachedItemRepository.SingleOrDefault(t => t.CacheKey == key);
                if (cachedItem == null)
                    return false;

                cachedItemRepository.Delete(cachedItem);
                cachedItemRepository.SaveChanges();
            }

            Logger.InstanceVerbose.Info(typeof(DbCacheManager).FullName, "Remove", "Removing from the DbCache key=" + key);

            return true;
        }

        public T GetOrAddToCache<T>(string key, Func<T> getValue, int? expirySeconds) where T : class
        { 
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "GetOrAddToCache");
            // Try and retreive the object first
            var o = Get<T>(key);

            bool addedInCache = false;

            // If it can't be found, add it. Otherwise, return the value in the cache
            if (o == null)
            {
                // Get the item 
                o = getValue();

                // If expirySeconds was not set, call the Add method that does not require it.
                // Otherwise call the method that does.
                if (expirySeconds == null)
                {
                    addedInCache = Add(key, o);
                }
                else
                {
                    addedInCache = Add(key, o, new TimeSpan(0,0,0, expirySeconds.Value));
                }
            }

            // Log the outcome
            Logger.InstanceVerbose.Debug(GetType().Name, "GetOrAddToCache", "Item added to cache=" + addedInCache);

            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "GetOrAddToCache");

            // Return the object, either from the cache or from the getValue() execution
            return o;
        }

        public bool AddWithSlidingExpiration<T>(string key, T value, TimeSpan slidingExpiry, bool overwrite = false) where T : class
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "AddWithSlidingExpiration");
            bool result = Add(key, value, slidingExpiry);
            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "AddWithSlidingExpiration");
            return result;
        }

        public void RemoveExpiredItems()
        {
            using (var cachedItemRepository = new Repository<CachedItem>())
            {
                var cachedItems =
                    cachedItemRepository.GetAsQueryable()
                        .Where(t => t.ExpiryMinutes != null && t.ElapsedMinutes > t.ExpiryMinutes);

                foreach (var d in cachedItems)
                {
                    cachedItemRepository.Delete(d);
                }

                cachedItemRepository.SaveChanges();
            }

        }
    }
}
