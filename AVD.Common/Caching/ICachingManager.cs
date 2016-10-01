using System;
using System.Runtime.Caching;

namespace AVD.Common.Caching
{
    public interface ICachingManager
    {
        /// <summary>
        ///     given an input array of strings, it creates a hashcode by concatenating the strings together and
        ///     passes them through a hashcode generator engine. As long as the strings as the same and 
        ///     sent in the same order, the
        ///     hashcode
        ///     should be the same.
        ///     this method trims the strings and converts them to lower case as well.
        ///     This method uses a MD5 algo to create a hash.
        /// </summary>
        /// <param name="inputs">An array of strings to be used to convert into a hashcode</param>
        /// <returns>a hashcode string</returns>
        String GetHashCodeKey(params string[] inputs);

        /// <summary>
        ///     this method takes in a key and an object as it's vaue and sticks it into the cache.
        ///     If overwrite is specified as false, then it will check the cache for the existence of the key/value pair
        ///     and returns false if the object is in cache. If overwrite is true or the object is not in the cache
        ///     then it is added into cache. By default, objects are added for 4 hours into the cache.
        /// </summary>
        /// <param name="key">
        ///     A key used to uniquely identify this object. Clients of this can use the getHashCode method
        ///     to generate a unique key from multiple piece of data if needed.
        /// </param>
        /// <param name="value">the object to be stored in cache.</param>
        /// <param name="overwrite">specifies if key/vaue pair in cache should be overwritten or not. default is false.</param>
        /// <returns>returns true if everything was fine. Returns false if object could not be added, or already exists in cache.</returns>
        bool Add<T>(string key, T value, bool overwrite = false);

        /// <summary>
        ///     this method takes in a key and an object as it's vaue and sticks it into the cache.
        ///     If overwrite is specified as false, then it will check the cache for the existence of the key/value pair
        ///     and returns false if the object is in cache. If overwrite is true or the object is not in the cache
        ///     then it is added into cache.
        ///     It also takes in a parameter that identifies how long the object should in the cache in seconds.
        /// </summary>
        /// <param name="key">
        ///     A key used to uniquely identify this object. Clients of this can use the getHashCode method
        ///     to generate a unique key from multiple piece of data if needed.
        /// </param>
        /// <param name="value">the object to be stored in cache.</param>
        /// <param name="expirySeconds">
        ///     no of seconds the object will live in the cache. It is mandatory to be supplied. a value of
        ///     -1 indicates forever.
        /// </param>
        /// <param name="overwrite">specifies if key/vaue pair in cache should be overwritten or not. default is false.</param>
        /// <returns>returns true if everything was fine. Returns false if object could not be added, or already exists in cache.</returns>
        bool Add<T>(string key, T value, int expirySeconds, bool overwrite = false);

        /// <summary>
        ///     This method works the same as an add method with an addtional feature - it can call a method of your choosing when
        ///     the item you
        ///     placed in the cache has expired or evicted out of the cache for any reason. to use, this, please define a method in
        ///     your class
        ///     that has a return type of void and takes a single argument of type
        ///     System.Runtime.Cachine.CacheEntryRemovedArguments .
        ///     upon eviction of your object from the cache, this method will be called and you can take action like re-inserting,
        ///     releasing a session with Amadeus or others.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirySeconds"></param>
        /// <param name="callback">The call back method that needs to be called when the object is evicted from cache</param>
        /// <param name="overwrite"></param>
        /// <returns>true if adding to cache succeeded.</returns>
        bool AddWithCallback<T>(string key, T value, int expirySeconds,
            Action<CacheEntryRemovedArguments> callback, bool overwrite = false);

        /// <summary>
        ///     This will cache an object under the given key with a sliding expiry of the given timespan.
        ///     If overwrite is specified as false, then it will check the cache for the existence of the key/value pair
        ///     and returns false if the object is in cache. If overwrite is true or the object is not in the cache
        ///     then it is added into cache.
        /// </summary>
        /// <param name="key">
        ///     A key used to uniquely identify this object. Clients of this can use the getHashCode method
        ///     to generate a unique key from multiple piece of data if needed.
        /// </param>
        /// <param name="value">the object to be stored in cache.</param>
        /// <param name="slidingExpiry">The timespan the item will live in the cache for.</param>
        /// <param name="overwrite">specifies if key/vaue pair in cache should be overwritten or not. default is false.</param>
        /// <returns>returns true if everything was fine. Returns false if object could not be added, or already exists in cache.</returns>
        bool AddWithSlidingExpiration<T>(string key, T value, TimeSpan slidingExpiry, bool overwrite = false);

        /// <summary>
        /// This method puts an object into the cache, sets a sliding expiry and also allows you to
        /// register a callback method to be called when the timer expires. Please note that callbacks
        /// are not guaranteed to be called immediately upon expiry of the item in cache. Usually,
        /// callback happens at some point after the item in cache. In test, if the item was to expire in 
        /// 1 second, the callback usually come back after 10 seconds.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="slidingExpiry"></param>
        /// <param name="callback">The method to be called upon expiry of this item. Please neote
        /// that the signature of this method should have return type void and should take a single
        /// argument of type system.runtim.caching.CacheEntryRemovedArguments</param>
        /// <param name="overwrite"></param>
        /// <returns>true on success.</returns>
        bool AddWithSlidingExpirationAndCallback<T>(string key, T value, TimeSpan slidingExpiry,
            Action<CacheEntryRemovedArguments> callback,
            bool overwrite = false);

        /// <summary>
        ///     If the given key exists, the cached item will be returned. Otherwise, the getValue will be
        ///     evaluated and added to the cache with the key.
        /// </summary>
        T GetExistingOrAdd<T>(string key, Func<T> getValue, int expirySeconds, bool overwrite = false);
         
        /// <summary>
        ///     Returns the object with the key specified. If the object does not exists, it returns a null.
        ///     If the object has expired, an exception is thrown.
        /// </summary>
        /// <param name="key">the key to be looked up in the cache.</param>
        /// <returns>An object, if one is found matching the key, null if not found, an exception is thrown if object is stale.
        /// unless it is NULL in which case it will return null</returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">the key to be looked up in the cache.</param>
        /// <param name="value">An object, if one is found matching the key, null if not found, an exception is thrown if object is stale.
        /// unless it is NULL in which case it will return null</param>
        /// <returns>Will return true if the item is in the cache.</returns>
        bool TryGet<T>(string key, out T value);

        /// <summary>
        ///     Removes an item from the cache with the key specified.
        /// </summary>
        /// <param name="key">key to be used to locate item</param>
        /// <returns>true if item was found and removed, false otherwise</returns>
        bool Remove<T>(string key);

        /// <summary>
        /// This method is essentially a helper method that will get the object backed by the key from the cache
        /// but if it doesn't exist, it will execute getValue(), add that result to the cache with the given key
        /// and return.
        /// 
        /// If the item could not be added to the cache, it will also return the given object however 
        /// a warning will be placed in the logs.
        /// 
        /// Remember - this is not a callback method. getValue() will be executed once iif the object is not in the
        /// cache when this method is called and the output placed in the cache.
        /// </summary>
        /// <remarks>This calls the same methods in this class so preconditions for Get and Add
        /// need to be met for this to be successful.</remarks>
        T GetOrAddToCache<T>(string key, Func<T> getValue, int? expirySeconds = null, bool overwrite = false) where T: class;

        void Clear();
    }
}