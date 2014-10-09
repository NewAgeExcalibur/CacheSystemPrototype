using System;
using log4net;

namespace CacheSystemPrototype.Infrastructure.Cache
{
    /// <summary>
    /// this class is responsible to provide a fast and smart way of caching
    /// to be able to do that it combines localcache with Distributed cache
    /// </summary>
    public class SmartCacheStore : ICacheStore
    {
        #region Properties

        private readonly ICacheStore distributedCacheStore;

        private readonly ICacheStore localCacheStore;

        private readonly ILog log;

        #endregion

        #region Constructor

        /// <summary>
        /// Creating an instance of SmartCacheStoreInterceptor
        /// </summary>
        /// <param name="distributedCacheStore">First Cache storage priority to Read/Write</param>
        /// <param name="localCacheStore">Second Cache storage priority to Read/Write</param>
        /// <param name="log"></param>
        public SmartCacheStore(ICacheStore distributedCacheStore, ICacheStore localCacheStore, ILog log)
        {
            if (distributedCacheStore == null) throw new ArgumentNullException("distributedCacheStore");
            if (localCacheStore == null) throw new ArgumentNullException("localCacheStore");
            if (log == null) throw new ArgumentNullException("log");

            this.distributedCacheStore = distributedCacheStore;
            this.localCacheStore = localCacheStore;
            this.log = log;
        }

        #endregion

        #region Implementation of ICacheStore Interface

        /// <summary>
        /// check local cache if object is high demand then it returns the value from local cache
        /// otherwise it returns it checkes distributed cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            //check if object exists on first priority of cache repository
            object value = localCacheStore.GetValue(key);

            if (value == null)
            {
                //Check if object exist on second priority of cache repository
                value = distributedCacheStore.GetValue(key);
                log.DebugFormat("Cache.Read from Distributed cache, key:{0}",key);
                
                //store the key, value into local cache asynchronously
                //if value might be null for the first time but since
                //reading from database in 10 times slower then second get will defenitly has the value
                // Its possible to make it async
                //the very first time value is null but since the first time we only caputre the access count so its fine
                localCacheStore.StoreValue(key, value);

            }
            else
            {
                log.DebugFormat("Cache.Read from Local cache, key:{0}", key);
            }

            return value;
        }

        /// <summary>
        /// Store Value of Object base on key on second priority of cache repository
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void StoreValue(string key, object value)
        {
            //this step can be Async to release the request to wait for storing value into the cache
             distributedCacheStore.StoreValue(key, value);
        }

        #endregion

    }
}