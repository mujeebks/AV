using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using AVD.Common.Logging;

namespace AVD.Common.Caching
{
    /// <summary>
    ///     this class encapsulates all the caching code for use for it's clients. It provides three basic methods - one to add
    ///     a
    ///     an item to a cache, second to retrieve the object, and the third to expire the item. Further, it provide a handy
    ///     method that
    ///     creates a hashcode given an array of inputs that can be used as a key for the cache. As long as the order and
    ///     actual strings passed remain the same
    ///     the method always returns the same hashcode.
    ///     Adding to cache can be done in two major ways - absolute expiry for the item, and a sliding expiry for the item. In each
    ///     of these cases, a callback method may or maynot be registered. this call back happens when the object is evicted from
    ///     cache. 
    ///     Further, there are some templated versions of the mthods also available.
    ///     Currently, the implementation uses the .Net caching service. In future, implementation may be switched out to
    ///      use some distributed caching service.
    ///     
    /// </summary>
    public sealed class CachingManager : ICachingManager
    {
        //TODO: rewire to use a distributed caching service.

        private static readonly MD5 md5Hash = MD5.Create();

        private static volatile ObjectCache cache;
        private static object syncRoot = new Object();

        private static object syncHash = new Object();

        public static TimeSpan DefaultTimeOut = new TimeSpan(0, 4, 0, 0);
        private const int DefaultTimeOutSeconds = 4 * 60 * 60;

        /// <summary>
        /// publi constructor to implement a thread safe caching manager singleton pattern
        /// to account for 
        /// http://social.msdn.microsoft.com/Forums/vstudio/en-US/1233ffb3-6480-431b-94ca-1190f96cf5f6/memorycache-get-in-disposed-state-magically?forum=netfxbcl
        /// bug in .Net 4.0 memory cache implementation
        /// </summary>
        public CachingManager ()
        {
            if (cache == null)
            {

                lock (syncRoot)
                {
                    if (cache == null)
                    {
                        using (ExecutionContext.SuppressFlow())
                        {
                            cache = MemoryCache.Default;
                        }
                    }
                }
            }
        }

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
        public String GetHashCodeKey(params string[] inputs)
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "GetHashCodeKey");
            //concat the strings
            string input = String.Join(",", inputs.Select(t => t.ToLower().Trim()));
            
            Logger.InstanceVerbose.Debug(GetType().Name, "GetHashCodeKey", "Input String: " + input);
            byte[] data = null;
            // now compute the hash
            // Convert the input string to a byte array and compute the hash. 
            lock (syncHash)
            {
                 data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            Logger.InstanceVerbose.Debug(GetType().Name, "GetHashCodeKey", "HashCode : " + sBuilder);

            // Return the hexadecimal string. 
            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "GetHashCodeKey");
            return sBuilder.ToString();
        }

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
        public bool Add<T>(string key, T value, bool overwrite = false)
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "Add_WithoutExpiry");
            bool retValue = false;
            if (!CheckAddInputs(key, value, ref overwrite))
                return false;
            // create a new cache item policy for 24h and put in cache
            retValue = Add(key, value, DefaultTimeOutSeconds, overwrite);

            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "Add_WithoutExpiry");
            return retValue;
        }

        /// <summary>
        ///     checks to see if the parameters specified into add are valid.
        ///     key and value should not be null, further, if overwrite is specified, then item
        ///     should not exist in cache
        /// </summary>
        /// <param name="key">key of ob to be added</param>
        /// <param name="value">object to be added to cache</param>
        /// <param name="overwrite">should th eobject be overwritten if it exists</param>
        /// <param name="retValue"></param>
        /// <returns>TRUE is checks pass</returns>
        private bool CheckAddInputs<T>(string key, T value, ref bool overwrite)
        {
            // check if key, or object is null
           
            if (String.IsNullOrWhiteSpace(key))
            {
                // log and return
                Logger.InstanceVerbose.Debug(GetType().Name, "CheckAddInputs", "Input key is null or whitespace");
                return false;
            }

            if (value == null)
            {
                // log and return
                Logger.InstanceVerbose.Debug(GetType().Name, "CheckAddInputs", "Value is null");
                return false;
            }
            
            string localKey = typeof(T) + ":" +  key.Trim();

            // check if the object exists and overwrite is false
            if ((cache.Get(localKey) != null) && (overwrite == false))
            {
                Logger.InstanceVerbose.Debug(GetType().Name, "CheckAddInputs",
                    "Object already exists in cache and cannot overwrite");
                return false;
            }
            return true;
        }

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
        public bool Add<T>(string key, T value, int expirySeconds, bool overwrite = false)
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "Add_WithExpiry");
            // check inputs
            if (!CheckAddInputs(key, value, ref overwrite))
                return false;
            //create a cache expiration policy
            var policy = new CacheItemPolicy();

            if (expirySeconds == -1)
            {
                policy.AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration;
            }
            else
            {
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expirySeconds);
            }
            bool result = AddToCache(key, value, overwrite, policy);
            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "Add_WithExpiry");
            return result;
        }

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
        public bool AddWithCallback<T>(string key, T value, int expirySeconds,
            Action<CacheEntryRemovedArguments> callback, bool overwrite = false)
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "AddWithCallback");
            // check inputs
            if (!CheckAddInputs(key, value, ref overwrite))
                return false;
            //create a cache expiration policy
            var policy = new CacheItemPolicy();

            if (expirySeconds == -1)
            {
                policy.AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration;
            }
            else
            {
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expirySeconds);
            }
            policy.RemovedCallback = new CacheEntryRemovedCallback(callback);
            bool result = AddToCache(key, value, overwrite, policy);
            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "AddWithCallback");
            return result;
        }

        /// <summary>
        ///     adds the item to cache once a policy is defined. Refactored method to reduce duplication in code.
        /// </summary>
        /// <param name="key">key for the cache</param>
        /// <param name="value">onject that needs to be places in the cache</param>
        /// <param name="overwrite">should this value be overwritten</param>
        /// <param name="policy">Policy used with the cached item</param>
        /// <returns>true if addition was successful, false otherwise</returns>
        private bool AddToCache<T>(string key, T value, bool overwrite, CacheItemPolicy policy)
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "AddToCache");

            key = typeof (T) + ":" + key.Trim();

            // check if item exists, if so, then evict it
            if (overwrite)
            {
                if (cache.Contains(key))
                {
                    Logger.InstanceVerbose.Info(GetType().Name, "AddToCache", "found the object, evicting it");
                    cache.Remove(key);
                }
            }
            //add to cache and return
            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "AddToCache");
            return cache.Add(key, value, policy);
        }

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
        public bool AddWithSlidingExpiration<T>(string key, T value, TimeSpan slidingExpiry, bool overwrite = false)
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "AddWithSlidingExpiration");
            if (!CheckAddInputs(key, value, ref overwrite))
                return false;
            var policy = new CacheItemPolicy();

            policy.SlidingExpiration = slidingExpiry;
            bool result = AddToCache(key, value, overwrite, policy);
            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "AddWithSlidingExpiration");
            return result;
        }

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
        public bool AddWithSlidingExpirationAndCallback<T>(string key, T value, TimeSpan slidingExpiry,
            Action<CacheEntryRemovedArguments> callback,
            bool overwrite = false)
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "AddwithSlidingExpirationAndCallback");
            if (!CheckAddInputs(key, value, ref overwrite))
                return false;
            var policy = new CacheItemPolicy();

            policy.SlidingExpiration = slidingExpiry;
            policy.RemovedCallback = new CacheEntryRemovedCallback(callback);
            bool result = AddToCache(key, value, overwrite, policy);
            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "AddwithSlidingExpirationAndCallback");
            return result;
        }

        /// <summary>
        ///     If the given key exists, the cached item will be returned. Otherwise, the getValue will be
        ///     evaluated and added to the cache with the key.
        /// </summary>
        public T GetExistingOrAdd<T>(string key, Func<T> getValue, int expirySeconds, bool overwrite = false)
        {
            T val;

            if(!TryGet(key, out val)){
                val = getValue();
                Add(key, val, expirySeconds, overwrite);
            }

            return val;
        }

        /// <summary>
        ///     A generic version of Get, this will cast the returned object to T, unless it is NULL in which case it will
        ///     return default(T)
        /// </summary>
        [Obsolete("Use Get(key, out T value) instead")]
        public T Get<T>(string key) where T : class
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "Get<" + typeof(T).Name + ">", key);

            T obj;

            TryGet(key, out obj);

            return obj;
        }

        public bool TryGet<T>(string key, out T value)
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "Get<" + typeof(T).Name + ">", key);

            key = typeof(T) + ":" + key.Trim();

            object o = cache.Get(key);

            if (o == null)
            {
                Logger.InstanceVerbose.Debug(GetType().Name, "Get<" + typeof(T).Name + ">", "Could not get " + key + " from cache, returning");
                value = default(T);
                return false;
            }

            if (!(o is T))
                throw new InvalidCastException(
                    String.Format("The cached type {0} is not the same as the requested type {1} for key {2}",
                        o.GetType().Name, typeof(T).Name, key));

            value = (T)o;
            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "Get<" + typeof(T).Name + ">");
            return true;
        }

        /// <summary>
        ///     Removes an item from the cache with the key specified.
        /// </summary>
        /// <param name="key">key to be used to locate item</param>
        /// <returns>true if item was found and removed, false otherwise</returns>
        public bool Remove<T>(string key)
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "Remove", key);
            key = typeof(T) + ":" + key.Trim();

            if (cache.Remove(key) == null)
            {
                return false;
            }
            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "Remove");
            return true;
        }

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
        public T GetOrAddToCache<T>(string key, Func<T> getValue, int? expirySeconds = null, bool overwrite = false) where T: class
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
                if(expirySeconds == null)
                {
                    addedInCache = Add(key, o, overwrite);
                }
                else
                {
                    addedInCache = Add(key, o, expirySeconds.Value, overwrite);
                }
            }

            // Log the outcome
            Logger.InstanceVerbose.Debug(GetType().Name, "GetOrAddToCache", "Item added to cache=" + addedInCache);

            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "GetOrAddToCache");

            // Return the object, either from the cache or from the getValue() execution
            return o;
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                List<KeyValuePair<String, Object>> cacheItems = (from n in cache.AsParallel() select n).ToList();

                foreach (KeyValuePair<String, Object> a in cacheItems)
                    cache.Remove(a.Key);
            }
        }
    }
}