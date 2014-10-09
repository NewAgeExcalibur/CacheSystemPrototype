namespace CacheSystemPrototype.Infrastructure.Cache
{
    /// <summary>
    /// all required config that required for cache system
    /// to mkae it simple for this example , all configs are in one file 
    /// Its possible each cache system can have its own config to make it more maintanable/managable 
    /// and avoid any conflics
    /// </summary>
    public class CacheConfiguration : ICacheConfiguration
    {
        /// <summary>
        /// Timeout for Local cache system
        /// </summary>
        public int LocalCacheTimeout {
            get { return 5 * 1000; }
        }

        /// <summary>
        /// Timeout for Local cache Accelerator system
        /// </summary>
        public int LocalCacheAcceleratorTimeout
        {
            get { return 5 * 1000; }
        }

        /// <summary>
        /// Timeout for distributed cache system
        /// </summary>
        public int DistributedCacheTimeout
        {
            get { return 5 * 1000; }
        }

        #region HighDemand Configs , can live in a separated config file

        /// <summary>
        /// default value
        /// </summary>
        private int maxNumberOfRequestToMarkObjectHighDemand = 30;
        /// <summary>
        /// max number of request to mark object high demand
        /// This config can be in a separated file 
        /// </summary>
        public int MaxNumberOfRequestToMarkObjectHighDemand
        {
            get { return maxNumberOfRequestToMarkObjectHighDemand; }
            set { maxNumberOfRequestToMarkObjectHighDemand = value; }
        }

        private int maxTimeInSecondsToKeepObjectHighDemand = 30;
        /// <summary>
        /// how long(In Seconds) high demand objects need to stay Highdemand
        /// this config can be in a separated file 
        /// </summary>
        public int MaxTimeInSecondsToKeepObjectHighDemand
        {
            get { return maxTimeInSecondsToKeepObjectHighDemand; }
            set { maxTimeInSecondsToKeepObjectHighDemand = value; }
        }

        #endregion

    }
}
