using System;
using System.Collections.Generic;
using System.Threading;
using log4net;

namespace CacheSystemPrototype.Infrastructure.Cache
{
    /// <summary>
    /// local cache is faster a bit because it does not have network over head
    /// so we can use it to store high demand object into it
    /// assumption: it takes 3 m s to read and write into local cache
    /// This is In Memory Cache
    /// </summary>
    public class LocalCacheStore : ICacheStore
    {
        #region Propeties

        protected readonly ICacheConfiguration cacheConfiguration;
        
        protected readonly ILog log;

        /// <summary>
        /// a dictionary includes all key and value of the cached object
        /// </summary>
        private Dictionary<string, object> values = new Dictionary<string, object>();

        ReaderWriterLockSlim slimLock = new ReaderWriterLockSlim();

        protected int lockTimeout = 0;

        #endregion

        #region Constructor

        public LocalCacheStore(ICacheConfiguration cacheConfiguration, ILog log)
        {
            // if (localCacheAccelerator == null) throw new ArgumentNullException("localCacheAccelerator");
            if (cacheConfiguration == null) throw new ArgumentNullException("cacheConfiguration");
            if (log == null) throw new ArgumentNullException("log");

            //this.localCacheAccelerator = localCacheAccelerator;
            this.cacheConfiguration = cacheConfiguration;
            this.log = log;

            lockTimeout = this.cacheConfiguration.LocalCacheTimeout;
        }

        #endregion

        #region Implementation of ICacheStore Interface

        /// <summary>
        /// Try to get the value from local cache 
        /// Plus notify local cache accelerator that a new request comes in for this key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual object GetValue(string key)
        {
            object value = null;

            log.DebugFormat("LocalCacheStore,Start GetValue({0})", key);
         
            if (slimLock.TryEnterReadLock(lockTimeout))
            {
                //simulates 3 ms roundtrip to the local cache
                Thread.Sleep(3);

                //try to get the value from local cache
                bool result= values.TryGetValue(key, out value);

                slimLock.ExitReadLock();

                log.DebugFormat("LocalCacheStore,Finish GetValue({0}), Result:{1}", key, result);
         
            }
            else
            {
                log.ErrorFormat("Cannot enter lock LocalCache.GetValue ,key:{0}", key);
            }
            
            return value;
        }

        /// <summary>
        /// store the value of the object into the local cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void StoreValue(string key, object value)
        {
            log.DebugFormat("LocalCacheStore,Start StoreValue({0}, {1})", key, value);

            if (slimLock.TryEnterWriteLock(lockTimeout))
            {
                //simulates 3 ms roundtrip to the local cache
                Thread.Sleep(3);

                values[key] = value;

                slimLock.ExitWriteLock();

                log.DebugFormat("LocalCacheStore,Finish StoreValue({0},{1})", key, value);
            }
            else
            {
                log.ErrorFormat("Cannot enter lock LocalCache.StoreValue ,key:{0}", key);
            }
         }

        #endregion
    }
}