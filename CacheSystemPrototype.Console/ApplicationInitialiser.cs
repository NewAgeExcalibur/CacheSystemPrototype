using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using log4net;
using CacheSystemPrototype.Infrastructure.Database;


namespace CacheSystemPrototype.Console
{
    /// <summary>
    /// Initialising application
    /// Adding test data
    /// </summary>
    public class ApplicationInitialiser
    {
        private readonly IDatabaseStore databaseStoreInterceptore;

        private readonly ILog log;

        public ApplicationInitialiser(DatabaseStore databaseStoreInterceptore, ILog log)
        {
            if (databaseStoreInterceptore == null) throw new ArgumentNullException("databaseStoreInterceptore");
            if (log == null) throw new ArgumentNullException("log");

            this.databaseStoreInterceptore = databaseStoreInterceptore;
            this.log = log;
        }


        /// <summary>
        /// setting up required data 
        /// 
        /// </summary>
        //public async void InitialiseAsync()
        //{
        //    log.DebugFormat("Start initialising the data, dateTime:{0}", DateTime.UtcNow.ToString("fff"));

        //    Stopwatch stopWatch = new Stopwatch();

        //    stopWatch.Start();

        //    log.DebugFormat("StartTime :{0}", new TimeSpan(stopWatch.ElapsedTicks).TotalSeconds);

        //    List<Task> warmupTasks = new List<Task>();

        //    warmupTasks.Add(Task.Factory.StartNew(() => databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 0), string.Format("value_{0}", 0))));
        //    warmupTasks.Add(Task.Factory.StartNew(() => databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 1), string.Format("value_{0}", 1))));
        //    warmupTasks.Add(Task.Factory.StartNew(() => databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 2), string.Format("value_{0}", 2))));
        //    warmupTasks.Add(Task.Factory.StartNew(() => databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 3), string.Format("value_{0}", 3))));
        //    warmupTasks.Add(Task.Factory.StartNew(() => databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 4), string.Format("value_{0}", 4))));
        //    warmupTasks.Add(Task.Factory.StartNew(() => databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 5), string.Format("value_{0}", 5))));
        //    warmupTasks.Add(Task.Factory.StartNew(() => databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 6), string.Format("value_{0}", 6))));
        //    warmupTasks.Add(Task.Factory.StartNew(() => databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 7), string.Format("value_{0}", 7))));
        //    warmupTasks.Add(Task.Factory.StartNew(() => databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 8), string.Format("value_{0}", 8))));
        //    warmupTasks.Add(Task.Factory.StartNew(() => databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 9), string.Format("value_{0}", 9))));

        //    stopWatch.Stop();

        //    log.DebugFormat("EndTime :{0}", new TimeSpan(stopWatch.ElapsedTicks).TotalSeconds);

        //    //return but whenever all tasks finish their work it will move on to the next line and mark it finish
        //    // it will release the manual reset event so other request can carry on and access the data
        //    await Task.WhenAll(warmupTasks);

        //   log.DebugFormat("waiting finishing the initialiser, dateTime:{0}", DateTime.UtcNow.ToString("O"));

        //   //databaseStoreInterceptore.Complete();
        //}

        public void Initialise()
        {
            databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 0), string.Format("value_{0}", 0));
            databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 1), string.Format("value_{0}", 1));
            databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 2), string.Format("value_{0}", 2));
            databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 3), string.Format("value_{0}", 3));
            databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 4), string.Format("value_{0}", 4));
            databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 5), string.Format("value_{0}", 5));
            databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 6), string.Format("value_{0}", 6));
            databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 7), string.Format("value_{0}", 7));
            databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 8), string.Format("value_{0}", 8));
            databaseStoreInterceptore.StoreValue(string.Format("key_{0}", 9), string.Format("value_{0}", 9));
        }
    }
}