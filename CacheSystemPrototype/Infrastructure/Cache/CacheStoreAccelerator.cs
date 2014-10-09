using System;
using System.Collections.Generic;
using System.Threading;
using log4net;

namespace CacheSystemPrototype.Infrastructure.Cache
{
    /// <summary>
    /// This class is responsible to keep the access rate information of object and use it on localcache store
    /// </summary>
    public class CacheStoreAccelerator :  ICacheStoreAccelerator
    {
        #region Properties

        /// <summary>
        /// this timeout should be greater than localcachestore timeout, to keep it simple , it should be twice as localcachestore
        /// </summary>
        private int lockTimeoutAccelerator
        {
            get { return  cacheConfiguration.LocalCacheAcceleratorTimeout; }
        }

        /// <summary>
        /// a dictionary to store access rate information of object based on cache key
        /// </summary>
        private Dictionary<string, ObjectAccessRate> values = new Dictionary<string, ObjectAccessRate>();

        /// <summary>
        /// internal lock , when new item is adding into the values(dictionary) then working thread needs to wait till
        /// writing is completly finished. then it can continue
        /// </summary>
        private static ReaderWriterLockSlim slimLock = new ReaderWriterLockSlim();

        private readonly ICacheConfiguration cacheConfiguration;

        private readonly ILog log;

        #endregion

        #region Constructor

        public CacheStoreAccelerator(ICacheConfiguration cacheConfiguration, ILog log)
       {
            if (cacheConfiguration == null) throw new ArgumentNullException("cacheConfiguration");
            if (log == null) throw new ArgumentNullException("log");

            this.cacheConfiguration = cacheConfiguration;
            this.log = log;
       }

        #endregion
       
        #region Instance methods- Notify, ContainKey, IsHighDemandObject

        /// <summary>
        /// identify that there is a request for the key
        /// </summary>
        /// <param name="key">unique key that used to store object into cache</param>
        public void Notify(string key)
        {
            log.DebugFormat("LocalCacheStoreAccelerator.Notify, key:{0}, before TryEnterReadLock.", key);

            if (slimLock.TryEnterWriteLock(lockTimeoutAccelerator))
            {
                log.DebugFormat("LocalCacheStoreAccelerator.Notify, key:{0}, after TryEnterReadLock.", key);

                ObjectAccessRate objectAccessRate;

                if (values.ContainsKey(key))
                {
                    var accessInfo = values[key];
                    accessInfo.IncreaseDemand();
                }
                else
                {
                    objectAccessRate = new ObjectAccessRate(cacheConfiguration.MaxTimeInSecondsToKeepObjectHighDemand
                        , cacheConfiguration.MaxNumberOfRequestToMarkObjectHighDemand);

                    values.Add(key, objectAccessRate);
                }

                slimLock.ExitWriteLock();

                log.DebugFormat("LocalCacheStoreAccelerator.Notify, key:{0},Exit lock.", key);

            }
            else
            {
                log.ErrorFormat("Cannot enter lock LocalCacheAccelerator.Notify ,key:{0}", key);
            }
        }

        /// <summary>
        /// check if key exist in the internal dictionary
        /// </summary>
        /// <param name="key">unique key that used to store object into cache</param>
        /// <returns></returns>
        public bool ContainKey(string key)
        {
            Boolean result = false;

            log.DebugFormat("LocalCacheStoreAccelerator.ContainKey, key:{0}, before TryEnterReadLock.", key);

            if (slimLock.TryEnterReadLock(lockTimeoutAccelerator))
            {
                log.DebugFormat("LocalCacheStoreAccelerator.ContainKey, key:{0}, after TryEnterReadLock.", key);

                result = values.ContainsKey(key);

                log.DebugFormat("LocalCacheStoreAccelerator.ContainKey, key:{0},Exit lock.", key);

                slimLock.ExitReadLock();
            }
            else
            {
                log.ErrorFormat("Cannot enter lock LocalCacheAccelerator.ContainKey ,key:{0}", key);
            }

            return result;
        }

        /// <summary>
        /// determind if this key is a highdemand
        /// </summary>
        /// <param name="key">unique key that used to store object into cache</param>
        /// <returns></returns>
        public bool IsHighDemandObject(string key)
        {
            log.DebugFormat("LocalCacheStoreAccelerator.IsHighDemandObject, key:{0}, before TryEnterReadLock.", key);

            bool hasHighDemand = false;

            if (slimLock.TryEnterReadLock(lockTimeoutAccelerator))
            {
                log.DebugFormat("LocalCacheStoreAccelerator.IsHighDemandObject, key:{0}, after TryEnterReadLock.", key);

                ObjectAccessRate objectAccessRate;

                if (values.TryGetValue(key, out objectAccessRate))
                {
                    hasHighDemand = objectAccessRate.HasHighDemand;
                }

                slimLock.ExitReadLock();

                log.DebugFormat("LocalCacheStoreAccelerator.IsHighDemandObject, key:{0},Exit lock, HasHighDemand:{1}", key, hasHighDemand);

                return hasHighDemand;
            }
            else
            {
                log.ErrorFormat("Cannot enter lock IsHighDemandObject,key:{0}",key);
            }

            return false;
        }

        #endregion
    }
}