using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.Common.Caching;

namespace AVD.Common.Caching
{
    public class NoCacheCachingManager : ICachingManager
    {
        private CachingManager CachingManager;

        public NoCacheCachingManager()
        {
            CachingManager = new CachingManager();
        }
        public string GetHashCodeKey(params string[] inputs)
        {
            return new CachingManager().GetHashCodeKey(inputs);
        }

        public bool Add<T>(string key, T value, bool overwrite = false)
        {
            return true;
        }

        public bool Add<T>(string key, T value, int expirySeconds, bool overwrite = false)
        {
            return true;
        }

        public bool AddWithCallback<T>(string key, T value, int expirySeconds, Action<System.Runtime.Caching.CacheEntryRemovedArguments> callback, bool overwrite = false)
        {
            return true;
        }

        public bool AddWithSlidingExpiration<T>(string key, T value, TimeSpan slidingExpiry, bool overwrite = false)
        {
            return true;
        }

        public bool AddWithSlidingExpirationAndCallback<T>(string key, T value, TimeSpan slidingExpiry, Action<System.Runtime.Caching.CacheEntryRemovedArguments> callback, bool overwrite = false)
        {
            return true;
        }

        public T GetExistingOrAdd<T>(string key, Func<T> getValue, int expirySeconds, bool overwrite = false)
        {
            return CachingManager.GetExistingOrAdd(key, getValue, expirySeconds, overwrite);
        }

        public T Get<T>(string key) where T : class
        {
            return null;
        }

        public bool TryGet<T>(string key, out T value)
        {
            value = default(T);
            return false;
        }

        public bool Remove<T>(string key)
        {
            return true;
        }

        public T GetOrAddToCache<T>(string key, Func<T> getValue, int? expirySeconds = null, bool overwrite = false) where T : class
        {
            return CachingManager.GetOrAddToCache(key, getValue, expirySeconds, overwrite);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
