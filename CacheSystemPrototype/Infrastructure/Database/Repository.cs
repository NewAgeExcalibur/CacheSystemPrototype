using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using CacheSystemPrototype.Infrastructure.Cache;

namespace CacheSystemPrototype.Infrastructure.Database
{
    /// <summary>
    /// this class is responsible to populate the data, Either using Cache or Database
    /// </summary>
    public class Repository : IDatabaseStore
    {
        #region Properties

        private readonly ICacheStore cacheStore;

        private readonly IDatabaseStore databaseStore;
        private readonly ILog log;

        #endregion

        #region Constructor

        public Repository(ICacheStore cacheStore, IDatabaseStore databaseStore, ILog log)
        {
            if (cacheStore == null) throw new ArgumentNullException("cacheStore");
            if (databaseStore == null) throw new ArgumentNullException("databaseStore");
            if (log == null) throw new ArgumentNullException("log");

            this.cacheStore = cacheStore;
            this.databaseStore = databaseStore;
            this.log = log;
        }

        #endregion

        #region Implementation of IDatabaseStore Interface

        public object GetValue(string key)
        {
            log.DebugFormat("Start Getting Key:{0}", key);

            var value = cacheStore.GetValue(key);

            if (value == null)
            {
                value = databaseStore.GetValue(key);
                
                log.DebugFormat("Read value from database, key:{0}", key);

                cacheStore.StoreValue(key, value);
            }
            else
            {
               log.DebugFormat("Read value from Cache, key:{0}", key);
                
            }

           log.DebugFormat("Finsih Getting Key:{0}", key);

           return value;
        }

       public void StoreValue(string key, object value)
       {
           databaseStore.StoreValue(key, value);
       }

        #endregion

    }
}
