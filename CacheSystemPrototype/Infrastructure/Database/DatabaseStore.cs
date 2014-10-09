using System;
using System.Collections.Generic;
using System.Threading;
using log4net;

namespace CacheSystemPrototype.Infrastructure.Database
{
    /// <summary>
    /// This class represent database storage and 
    /// implement available operations
    /// </summary>
    public class DatabaseStore : IDatabaseStore
    {
        private readonly ILog log;

        public DatabaseStore(ILog log)
        {
            if (log == null) throw new ArgumentNullException("log");

            this.log = log;
        }

        public object GetValue(string key)
        {
            log.DebugFormat("DatabaseStore,Start GetValue({0})", key);
            //simulates 50 ms roundtrip to the database
            Thread.Sleep(50);

            object value=null;

           bool result= values.TryGetValue(key, out value);

           log.DebugFormat("DatabaseStore, Finish GetValue({0}), result:{1}", key, result);
            
            return value;
        }

        public void StoreValue(string key, object value)
        {
            //simulates 50 ms roundtrip to the database
            Thread.Sleep(50);
            values[key] = value;
        }

        /// <summary>
        /// This dictionary represent database
        /// </summary>
        Dictionary<string, object> values = new Dictionary<string, object>();
    }
}