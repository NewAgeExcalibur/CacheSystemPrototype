using System;
using System.Collections.Generic;
using System.Threading;
using log4net;

namespace CacheSystemPrototype.Infrastructure.Cache
{
    /// <summary>
    /// Implementation of a distributed cache store
    /// To store/retreive cache data from a distributed cache
    /// </summary>
    public class DistributedCacheStore: ICacheStore
    {
        #region Properties

        private readonly ICacheConfiguration cacheConfiguration;
        
        private readonly ILog log;

        private Dictionary<string, object> values = new Dictionary<string, object>();

        private static ReaderWriterLockSlim slimLock = new ReaderWriterLockSlim();

        private int lockTimeout = 0;

        #endregion

        #region Constructor

        public DistributedCacheStore(ICacheConfiguration cacheConfiguration, ILog log)
        {
            if (cacheConfiguration == null) throw new ArgumentNullException("cacheConfiguration");
            if (log == null) throw new ArgumentNullException("log");
            this.cacheConfiguration = cacheConfiguration;
            this.log = log;

            lockTimeout = cacheConfiguration.DistributedCacheTimeout;
        }

        #endregion

        #region Implementation of ICacheStore interface

        public object GetValue(string key)
        {
            log.DebugFormat("DistributedCacheStore,Start GetValue({0})", key);
          
            object value = null;

            if (slimLock.TryEnterReadLock(lockTimeout))
            {
                //simulates 5 ms roundtrip to the distributed cache
                Thread.Sleep(5);

                bool result=values.TryGetValue(key, out value);

                slimLock.ExitReadLock();

                log.DebugFormat("DistributedCacheStore,Finish GetValue({0}), Result:{1}", key, result);
            }
            else
            {
                log.ErrorFormat("Cannot enter lock DistributedCache.GetValue ,key:{0}", key);
            }
            
            return value;
        }

        public void StoreValue(string key, object value)
        {
            if (slimLock.TryEnterWriteLock(lockTimeout))
            {
                //simulates 5 ms roundtrip to the distributed cache
                Thread.Sleep(5);

                values[key] = value;

                slimLock.ExitWriteLock();
            }
            else
            {
                log.ErrorFormat("Cannot enter lock DistributedCache.StoreValue ,key:{0}", key);
            }
            


        }

        #endregion
    }
}