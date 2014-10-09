using System;

namespace CacheSystemPrototype.Infrastructure.Cache
{
    /// <summary>
    /// To identify access rate information of object 
    /// </summary>
    public class ObjectAccessRate
    {
        #region Properties

        /// <summary>
        /// capture the latest datetime
        /// </summary>
        private DateTime LastAccesDateTime;

        private int accessCount;

        /// <summary>
        /// determind the number of times application tries to access 
        /// </summary>
        public int AccessCount
        {
            get { return accessCount; }
        }

        /// <summary>
        /// if object access within 30 Seconds and access rate is greater than 30 then its highdemand
        /// otherwise its normal 
        /// These configs can come from configuration as well
        /// This logic can be more complicated like we could consider a time frame and
        /// within each time frame defining the high demand and cleaning it up, to Follow KISS principle I just consider it as simple as possible :)
        /// </summary>
        public bool HasHighDemand
        {
            get
            {
                bool hasHighDemandAccessCount = AccessCount >= maxNumberOfRequestToMarkObjectHighDemand;

                bool hasBeenAccessedRecently = LastAccesDateTime >= DateTime.UtcNow.AddSeconds(maxTimeInSecondsToKeepObjectHighDemand * -1);
                
                if (hasHighDemandAccessCount && hasBeenAccessedRecently)
                {
                    return true;
                }

                return false;
            }
        }

        private readonly int maxNumberOfRequestToMarkObjectHighDemand;

        private readonly int maxTimeInSecondsToKeepObjectHighDemand;
       
        #endregion

        #region Constructor

        public ObjectAccessRate(int maxTimeInSecondsToKeepObjectHighDemand, int maxNumberOfRequestToMarkObjectHighDemand)
        {
            this.maxTimeInSecondsToKeepObjectHighDemand = maxTimeInSecondsToKeepObjectHighDemand;
            this.maxNumberOfRequestToMarkObjectHighDemand = maxNumberOfRequestToMarkObjectHighDemand;
            
            LastAccesDateTime = DateTime.UtcNow;
            accessCount = 1;
        }

        #endregion

        #region Instance method, IncreaseDemand
        
        /// <summary>
        /// Increase demand of object 
        /// Increase access count by one and update last access date time
        /// </summary>
        public void IncreaseDemand()
        {
            LastAccesDateTime = DateTime.UtcNow;
            accessCount++;
        }

        #endregion
    }
}