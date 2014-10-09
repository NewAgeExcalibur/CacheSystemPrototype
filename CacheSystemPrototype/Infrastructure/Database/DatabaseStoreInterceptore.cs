using System;
using System.Threading;
using log4net;

namespace CacheSystemPrototype.Infrastructure.Database
{
    /// <summary>
    /// The main responsibility of this class is controlling write and read asynchronously
    /// It intercept the implementation of the DatabaseStore and do more things
    /// Usually synchronising the request is not part of data access
    /// NOTE: This class not been used
    /// </summary>
    public class DatabaseStoreInterceptore : IDatabaseStore
    {
        #region Properties

        private readonly IDatabaseStore databaseStore;
        private readonly ILog log;

        /// <summary>
        /// to control read and store operation
        /// by default is clsoed(non of thread can pass read operation
        /// after Calling Complete method on this object then it opens the gate and let all waiting 
        /// threads pass read operation
        /// </summary>
        private static ManualResetEvent waitHandle;
      
        static Object mylock = new object();

        #endregion

        #region Constructor

        public DatabaseStoreInterceptore(IDatabaseStore databaseStore, ILog log)
        {
            if (databaseStore == null) throw new ArgumentNullException("databaseStore");
            if (log == null) throw new ArgumentNullException("log");
            this.databaseStore = databaseStore;
            this.log = log;

            //by default is closed
            waitHandle = new ManualResetEvent(false);
        }

        #endregion

        #region Added Instance method- Complete

        /// <summary>
        /// After database data has been initialised completly then this methos gets called to release 
        /// all waiting thread access data
        /// </summary>
        public void Complete()
        {
            waitHandle.Set();

           log.DebugFormat("data initialising is completed, dateTime:{0}", DateTime.UtcNow.ToString("fff"));
        }

        #endregion

        #region Implementation of IDatabaseStore Interface

        /// <summary>
        /// Getting value from Database
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            //make all waiting threads stay here and not pass this point
            waitHandle.WaitOne();

            log.DebugFormat("databaseStore.GetValue({0}), dateTime:{1}", key, DateTime.UtcNow.ToString("fff"));

            var result = databaseStore.GetValue(key);

            return result;
        }
        
        /// <summary>
        /// This operation is alwayes allowed
        /// </summary>
        /// <param name="key">primary key of the object</param>
        /// <param name="value">object that is going to save</param>
        public void StoreValue(string key, object value)
        {
            lock (mylock)
            {
                databaseStore.StoreValue(key, value);
            }

        }
        
        #endregion
    }
}