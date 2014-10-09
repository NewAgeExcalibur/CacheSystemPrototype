namespace CacheSystemPrototype.Infrastructure.Cache
{
    /// <summary>
    /// the interface which includes all configs variable for cache
    /// </summary>
    public interface ICacheConfiguration
    {
        /// <summary>
        /// Timeout for Local cache system
        /// </summary>
        int LocalCacheTimeout { get; }

        /// <summary>
        /// Timeout for Local cache Accelerator system
        /// </summary>
        int LocalCacheAcceleratorTimeout { get; }

        /// <summary>
        /// Timeout for distributed cache system
        /// </summary>
        int DistributedCacheTimeout { get; }

        /// <summary>
        /// max number of request to mark object high demand
        /// This config can be in a separated file 
        /// </summary>
        int MaxNumberOfRequestToMarkObjectHighDemand { get; set; }

        /// <summary>
        /// how long(In Seconds) high demand objects need to stay Highdemand
        /// this config can be in a separated file 
        /// </summary>
        int MaxTimeInSecondsToKeepObjectHighDemand { get; set; }
    }
}