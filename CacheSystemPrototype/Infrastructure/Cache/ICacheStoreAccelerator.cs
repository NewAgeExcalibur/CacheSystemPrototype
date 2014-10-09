namespace CacheSystemPrototype.Infrastructure.Cache
{
    /// <summary>
    /// Includes required methods required to accelerate cache store
    /// </summary>
    public interface ICacheStoreAccelerator
    {
        /// <summary>
        /// identify that there is a request for the key
        /// </summary>
        /// <param name="key">unique key that used to store object into cache</param>
        void Notify(string key);

        /// <summary>
        /// check if key exist in the internal dictionary
        /// </summary>
        /// <param name="key">unique key that used to store object into cache</param>
        /// <returns></returns>
        bool ContainKey(string key);

        /// <summary>
        /// determind if this key is a highdemand
        /// </summary>
        /// <param name="key">unique key that used to store object into cache</param>
        /// <returns></returns>
        bool IsHighDemandObject(string key);
    }
}