using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Moq;
using CacheSystemPrototype.Infrastructure.Cache;
using CacheSystemPrototype.Infrastructure.Database;
using Xunit;

namespace CacheSystemPrototype.UnitTest
{
    /// <summary>
    /// Includes all tests for Local Cache, Distributed Cache, Smart Cache,
    ///  Cache Accelerator, Object Access Rate
    /// </summary>
    public class CacheTests
    {
        #region Factory

        private ILog GetLog()
        {
            ILog log = Mock.Of<ILog>();
            return log;
        }

        private ICacheConfiguration GetConfiguration()
        {
            var cacheConfiguration = new CacheConfiguration();
            //To make tests faster and simpler , set these values with smaller number
            cacheConfiguration.MaxNumberOfRequestToMarkObjectHighDemand = 5;
            cacheConfiguration.MaxTimeInSecondsToKeepObjectHighDemand = 3;

            return cacheConfiguration;
        }

        private LocalCacheStore GetLocalCacheStore()
        {
            var log = GetLog();

            ICacheConfiguration cacheConfiguration = GetConfiguration();

            LocalCacheStore localCache = new LocalCacheStore(cacheConfiguration, log);

            return localCache;
        }

        private DistributedCacheStore GetDistributedCacheStore()
        {
            var log = GetLog();

            ICacheConfiguration cacheConfiguration = GetConfiguration();

            DistributedCacheStore localCache = new DistributedCacheStore(cacheConfiguration, log);

            return localCache;
        }

        private ICacheStoreAccelerator GetCacheStoreAccelerator()
        {
            var log = GetLog();

            ICacheConfiguration cacheConfiguration = GetConfiguration();

            ICacheStoreAccelerator cacheStoreAccelerator = new CacheStoreAccelerator(cacheConfiguration, log);
            
            return cacheStoreAccelerator;
        }

        private SmartCacheStore getSmartCacheStoreInterceptor()
        {
            var CacheStoreAccelerator = GetCacheStoreAccelerator();
            var cacheConfiguration = GetConfiguration();

            var log = GetLog();

            var LocalCacheAccelerator = new LocalCacheStoreAccelerator(CacheStoreAccelerator, cacheConfiguration, log);

            var distributedCacheStore = GetDistributedCacheStore();

            SmartCacheStore smartCacheStoreInterceptor = new SmartCacheStore(distributedCacheStore, LocalCacheAccelerator, log);

            return smartCacheStoreInterceptor;
        }

        #endregion

        #region Local Cache Store Tests
        [Fact]
        public void LocalCacheStore_GetValueByKeyforNonExisting_ValueShouldBeNull()
        {
            var cacheStore = GetLocalCacheStore();

            var value = cacheStore.GetValue("NonExistingKey");

            Assert.Null(value);
        }

        [Fact]
        public void LocalCacheStore_StoreOneItem_AddedItemHasToBeThere()
        {
            var cacheStore = GetLocalCacheStore();
            var key = "key_1";
            var value = "value_1";

            cacheStore.StoreValue(key,value);

            var result = cacheStore.GetValue(key);

            Assert.Equal(value, result);
        }

        [Fact]
        public void LocalCacheStore_StoreOneItem_ShouldTakeMoreThanSixMiliSecondsToRetrieveAddedItem()
        {
            var cacheStore = GetLocalCacheStore();
            var key = "key_1";
            var value = "value_1";

            cacheStore.StoreValue(key, value);
            Stopwatch stopwatch=new Stopwatch();

            stopwatch.Start();

            var result = cacheStore.GetValue(key);
            
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            
            Assert.NotNull(result);
            Assert.InRange(elapsedMilliseconds, 1,6);
        }

        [Fact]
        public void LocalCacheStore_StoreOneItemTwice_AddedItemHasToBeThere()
        {
            var cacheStore = GetLocalCacheStore();
            var key = "key_1";
            var value = "value_1";

            cacheStore.StoreValue(key, value);
            cacheStore.StoreValue(key, value);

            var result = cacheStore.GetValue(key);

            Assert.Equal(value, result);
        }
       
        [Fact]
        public void LocalCacheStore_StoreOneItemTwiceAtTheSameTime_AddItemHasToBeThere()
        {
            var cacheStore = GetLocalCacheStore();
            var key = "key_1";
            var value = "value_1";
            List<Task> tasks=new List<Task>();

            Task task1 = new Task(() => cacheStore.StoreValue(key, value));
            tasks.Add(task1);
            Task task2 = new Task(() => cacheStore.StoreValue(key, value));
            tasks.Add(task2);
            task1.Start();
            task2.Start();

            Thread.Sleep(3);

            var result = cacheStore.GetValue(key);

            Assert.Equal(value, result);
        }
        #endregion

        #region Distributed Cache Store Tests
        [Fact]
        public void DistributedCacheStore_GetValueByKeyforNonExisting_ValueShouldBeNull()
        {
            var cacheStore = GetDistributedCacheStore();

            var value = cacheStore.GetValue("NonExistingKey");

            Assert.Null(value);
        }

        [Fact]
        public void DistributedCacheStore_StoreOneItem_AddedItemHasToBeThere()
        {
            var cacheStore = GetDistributedCacheStore();
            var key = "key_1";
            var value = "value_1";
            cacheStore.StoreValue(key, value);

            var result = cacheStore.GetValue(key);

            Assert.Equal(value, result);
        }

        [Fact]
        public void DistributedCacheStore_StoreOneItem_ShouldTakeMoreThanTenMiliSecondsToRetrieveAddedItem()
        {
            var cacheStore = GetDistributedCacheStore();
            var key = "key_1";
            var value = "value_1";

            cacheStore.StoreValue(key, value);
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            var result = cacheStore.GetValue(key);

            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            Assert.NotNull(result);
            Assert.InRange(elapsedMilliseconds, 1, 10);
        }

        [Fact]
        public void DistributedCacheStore_StoreOneItemTwice_AddedItemHasToBeThere()
        {
            //Arrange
            var cacheStore = GetDistributedCacheStore();
            var key = "key_1";
            var value = "value_1";

            //Act
            cacheStore.StoreValue(key, value);
            cacheStore.StoreValue(key, value);
            
            var result = cacheStore.GetValue(key);
            
            //Assert
            Assert.Equal(value, result);
        }

        #endregion

        #region Cache Accelerator Tests

        [Fact]
        public void CacheAccelerator_CallNotifyForOneItemOnce_ItemIsNotHighDemand()
        {
            var localCacheStoreAccelerator = GetCacheStoreAccelerator();
           
            string key = "key_1";

            localCacheStoreAccelerator.Notify(key);
            
            bool isHighDemandObject = localCacheStoreAccelerator.IsHighDemandObject(key);

            Assert.False(isHighDemandObject);
        }

        [Fact]
        public void CacheAccelerator_CallNotifyFiveTimesForOneItem_ItemIsHighDemand()
        {
            var cacheStoreAccelerator = GetCacheStoreAccelerator();

            string key = "key_1";

            for (int i = 0; i < 5; i++)
            {
                cacheStoreAccelerator.Notify(key);
            }

            var isHighDemandObject = cacheStoreAccelerator.IsHighDemandObject(key);

            Assert.True(isHighDemandObject);
        }

        [Fact]
        public void CacheAccelerator_CallNotifyFiveTimesForOneItemWaitForOneSecond_ItemIsNotHighDemand()
        {
            var cacheStoreAccelerator = GetCacheStoreAccelerator();

            var cacheConfiguration = GetConfiguration();

            string key = "key_1";

            for (int i = 0; i < 5; i++)
            {
                cacheStoreAccelerator.Notify(key);
            }

            //wait till item is not high demand any more
            Thread.Sleep(cacheConfiguration.MaxTimeInSecondsToKeepObjectHighDemand * 1000);

            var isHighDemandObject = cacheStoreAccelerator.IsHighDemandObject(key);

            Assert.False(isHighDemandObject);
        }


        [Fact]
        public void LocaLCacheAccelerator_TryToStoreHighDemandItem_StoredItemShouldExist()
        {
            //Arrange
            var cacheStoreAccelerator = new Mock<ICacheStoreAccelerator>();

            LocalCacheStoreAccelerator localCacheStoreAccelerator = new LocalCacheStoreAccelerator(cacheStoreAccelerator.Object, GetConfiguration(), GetLog());

            cacheStoreAccelerator.Setup(x => x.IsHighDemandObject(It.IsAny<string>())).Returns(true);
            cacheStoreAccelerator.Setup(x => x.ContainKey(It.IsAny<string>())).Returns(true);

            string key = "key_1";
            string value = "value_1";
            localCacheStoreAccelerator.StoreValue(key, value);

            //Act
            var result = localCacheStoreAccelerator.GetValue(key);

            //Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void LocalCacheAccelerator_TryToStoreNotHighDemandItemAndCallingGetValue_ReturnValueShouldBeNull()
        {
            //Arrange
            var cacheStoreAccelerator = new Mock<ICacheStoreAccelerator>();

            LocalCacheStoreAccelerator localCacheStoreAccelerator = new LocalCacheStoreAccelerator(cacheStoreAccelerator.Object, GetConfiguration(), GetLog());

            cacheStoreAccelerator.Setup(x => x.IsHighDemandObject(It.IsAny<string>())).Returns(false);
            cacheStoreAccelerator.Setup(x => x.ContainKey(It.IsAny<string>())).Returns(false);

            string key = "key_1";
            string value = "value_1";
            localCacheStoreAccelerator.StoreValue(key, value);

            //Act
            var result = localCacheStoreAccelerator.GetValue(key);

            //Assert
            Assert.Null(result);
        }

        #endregion

        #region Smart Cache and also repository Tests

        [Fact]
        public void SmartCacheStore_ItemNotExistInCache_ReturnsNullValue()
        {
            var smartCacheStoreInterceptor = getSmartCacheStoreInterceptor();

            string key = "key_1";

            var result=smartCacheStoreInterceptor.GetValue(key);

            Assert.Null(result);
        }

        [Fact]
        public void SmartCacheStore_StoringHighDemandItem_ShouldntTakeMoreThanSixMiliSecondsToRetrieveAddedItem()
        {
            var cacheStore = GetDistributedCacheStore();
        
            var key = "key_1";
            var value = "value_1";

            cacheStore.StoreValue(key, value);

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            var result = cacheStore.GetValue(key);

            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            Assert.NotNull(result);

            Assert.InRange(elapsedMilliseconds, 1, 10);
        }

        [Fact]
        public void Repository_ItemNotExistInCache_StoreValueGetsCalled()
        {
            ICacheStore cacheStore = Mock.Of<ICacheStore>();

            IDatabaseStore databaseStore = Mock.Of<IDatabaseStore>();

            Repository repository = new Repository(cacheStore, databaseStore, GetLog());

            string key = "key_1";

            repository.GetValue(key);
            
            Mock.Get(cacheStore).Verify(x=> x.StoreValue(key, It.IsAny<string>()));
        }

        [Fact]
        public void SmartCacheStore_GetValueGetsCalled_LocalGetValueShouldBeCalled()
        {
            //Arrange
            ICacheStore localCacheStore = Mock.Of<ICacheStore>();
            ICacheStore distributedcacheStore = Mock.Of<ICacheStore>();
            SmartCacheStore smartCacheStoreInterceptor = new SmartCacheStore(distributedcacheStore, localCacheStore, GetLog());

            string key = "Key_1";

            //Act
            smartCacheStoreInterceptor.GetValue(key);

            //Assert
            Mock.Get(localCacheStore).Verify(x => x.GetValue(key));
        }

        [Fact]
        public void SmartCacheStore_ItemNotExistInLocalCache_LocalGetValueShouldBeCalled()
        {
            //Arrange
            var localCacheStore = new Mock<ICacheStore>();
            ICacheStore distributedcacheStore = Mock.Of<ICacheStore>();
            SmartCacheStore smartCacheStoreInterceptor = new SmartCacheStore(distributedcacheStore, localCacheStore.Object, GetLog());
            
            localCacheStore.Setup(x => x.GetValue(It.IsAny<string>())).Returns(null);
            //Act
            string key = "key_1";
            smartCacheStoreInterceptor.GetValue(key);

            //Assert
            localCacheStore.Verify(x => x.GetValue(key));
         }

        #endregion

        #region Object Access Rate Tests
        [Fact]
        public void ObjectAccessRate_Initialise_AccessCountShouldBeOne()
        {
            ObjectAccessRate objectAccessRate=new ObjectAccessRate(0,0);

            Assert.Equal(1, objectAccessRate.AccessCount);
        }

        [Fact]
        public void ObjectAccessRate_IncreaseDemandGetsCalled_AccessCountShouldIncrease()
        {
            ObjectAccessRate objectAccessRate = new ObjectAccessRate(0, 0);
            objectAccessRate.IncreaseDemand();

            Assert.Equal(2, objectAccessRate.AccessCount);
        }

        [Fact]
        public void ObjectAccessRate_IncreaseDemandGetsCalledFiveTimes_HasHighDemandShouldBeTrue()
        {
            ObjectAccessRate objectAccessRate = new ObjectAccessRate(0, 0);
            
            //increasing demand of the object 5 times
            objectAccessRate.IncreaseDemand();
            objectAccessRate.IncreaseDemand();
            objectAccessRate.IncreaseDemand();
            objectAccessRate.IncreaseDemand();
            objectAccessRate.IncreaseDemand();

            Assert.True(objectAccessRate.HasHighDemand);

        }

        #endregion

    }
}
