using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using CacheSystemPrototype.Console;
using log4net;
using log4net.Config;
using CacheSystemPrototype.Infrastructure.Cache;
using CacheSystemPrototype.Infrastructure.Database;
using CacheSystemPrototype.Service;
using System.Diagnostics;

namespace myCacheApp.Console
{

    class Program
    {
        [DllImport("winmm.dll")]
        internal static extern uint timeBeginPeriod(uint period);
        [DllImport("winmm.dll")]
        internal static extern uint timeEndPeriod(uint period);

        /// <summary>
        /// To generate random requested key
        /// </summary>
        static Random random = new Random();

        static void Main(string[] args)
        {
            #region Display Header Info

            System.Console.ForegroundColor = ConsoleColor.DarkGreen;
            System.Console.WriteLine("---------------------------------------------------------------");
            System.Console.WriteLine("Welcome to NearMap Test ---------------------------------------");
            System.Console.WriteLine("------------------------ https://github.com/izevaka/nearmap-test");
            System.Console.WriteLine("Amir--Pour-----------------------------------------------------");
            System.Console.WriteLine("Mobile: 0414 888 931-------------------------------------------");
            System.Console.WriteLine("---------------------------------------------------------------");
            System.Console.ResetColor();

            #endregion

            #region Setting up log

            ILog log = LogManager.GetLogger(typeof(Program));

            FileInfo configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.xml"));

            XmlConfigurator.ConfigureAndWatch(configFile);

            #endregion

            #region Composition root- Any kind of Dependency injector can be used to set it up

            DatabaseStore databaseStore = new DatabaseStore(log);

            ApplicationInitialiser applicationInitialiser = new ApplicationInitialiser(databaseStore, log);

            applicationInitialiser.Initialise();

            //getting all configs required for application
            CacheConfiguration cacheConfiguration = new CacheConfiguration();

            ICacheStoreAccelerator cacheStoreAccelerator = new CacheStoreAccelerator(cacheConfiguration, log);

            //this class helps local cache work faster, it adds more functionality
            LocalCacheStoreAccelerator localCacheAccelerator = new LocalCacheStoreAccelerator(cacheStoreAccelerator, cacheConfiguration, log);

            //this is the distributed cache 
            DistributedCacheStore distributedCacheStore = new DistributedCacheStore(cacheConfiguration, log);

            //this class combines the localcache and distributed cache together  to provide a faster caching mechanism after a while
            //for more demanded objects, this class is the cache layer of the application
            SmartCacheStore fastCacheInterceptor = new SmartCacheStore(distributedCacheStore, localCacheAccelerator, log);

            //this class is responsible to retrieve the data either from cache layer or from database 
            Repository repository = new Repository(fastCacheInterceptor, databaseStore, log);

            //this is the service that is going to expose to all consumers
            MyService myService = new MyService(repository);

            #endregion

            System.Console.WriteLine("Start up finished");
            System.Console.WriteLine("---------------------------------------------------------------");

            for (int i = 0; i < 10; i++)
            {
                new Thread(() =>
                {
                    //Sets the resolution of the timer. This has to be done per thread to ensure that 5ms sleep is actually 5 ms.
                    //http://stackoverflow.com/a/522681
                    timeBeginPeriod(5);

                    for (int j = 0; j < 50; j++)
                    {
                        string key = string.Format("key_{0}", random.Next(0, 9));

                        Stopwatch myStopwatch = new Stopwatch();

                        myStopwatch.Reset();
                        myStopwatch.Start();

                        var value = myService.GetItem(key);

                        myStopwatch.Stop();

                        var duration = myStopwatch.ElapsedMilliseconds;

                        System.Console.WriteLine("[{0}] Request '{1}', response '{2}', time: {3}",
                            Thread.CurrentThread.ManagedThreadId,
                            key,
                            value,
                            duration.ToString("F2"));

                        System.Console.ResetColor();

                    }

                    timeEndPeriod(5);

                }).Start();
            }

            System.Console.WriteLine("----E--N--D---");

            System.Console.Read();

        }
     
    }
}
