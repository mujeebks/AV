using System;

namespace AVD.Common
{
    public interface IPersistentCacheManager
    {
        string GetHashCodeKey(params string[] inputs);
        bool Add<T>(string key, T value) where T : class;

        /// <summary>
        /// Adds an item to the cache with the given expiry
        /// </summary>
        /// <remarks>Note that the expiry has not been implemented yet.</remarks>
        bool Add<T>(string key, T value, TimeSpan? expiry) where T : class;
        bool Add<T>(string key, T value, int expirySeconds) where T : class;

        T Get<T>(string key) where T : class;
        bool Remove<T>(string key) where T : class;

        T GetOrAddToCache<T>(string key, Func<T> value, int? seconds) where T : class;

        bool AddWithSlidingExpiration<T>(string key, T value, TimeSpan timeSpan, bool overwrite = false) where T : class;
    }
}