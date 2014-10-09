using System;
using log4net;

namespace CacheSystemPrototype.Infrastructure.Cache
{
    /// <summary>
    /// This class is responsible to keep the access rate information of object and use it on localcache store
    /// </summary>
    public class LocalCacheStoreAccelerator : LocalCacheStore
    {
        private readonly ICacheStoreAccelerator cacheStoreAccelerator;

        #region Constructor

        public LocalCacheStoreAccelerator(ICacheStoreAccelerator cacheStoreAccelerator, ICacheConfiguration cacheConfiguration, ILog log)
            : base(cacheConfiguration, log)
        {
            if (cacheStoreAccelerator == null) throw new ArgumentNullException("cacheStoreAccelerator");

            this.cacheStoreAccelerator = cacheStoreAccelerator;
        }

        #endregion

        #region Overriding the implementation of GetValue and StoreValue of LocalCache class 
        
        /// <summary>
        /// get  value from local cache and also send notification to capture the access rate of the 
        /// object base on the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object GetValue(string key)
        {
            object value = null;
            
            //check if key is available is localcacheaccelerator dictionary
            if (cacheStoreAccelerator.ContainKey(key))
            {
               value = base.GetValue(key);
            }

            cacheStoreAccelerator.Notify(key);

            return value;
        }

        /// <summary>
        /// Store the object in local cache if access rate is high
        /// </summary>
        /// <param name="key">unique key that used to store object into cache</param>
        /// <param name="value">value of the object</param>
        public override void StoreValue(string key, object value)
        {
            if (cacheStoreAccelerator.IsHighDemandObject(key))
            {
                base.StoreValue(key, value);
            }
        }

        #endregion

       
    }
}