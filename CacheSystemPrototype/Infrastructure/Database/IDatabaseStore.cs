namespace CacheSystemPrototype.Infrastructure.Database
{
    /// <summary>
    /// retreive/store data using Database
    /// </summary>
    public interface IDatabaseStore
    {
        /// <summary>
        /// retrieve the object from database
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetValue(string key);

        ///// <summary>
        ///// Store data into database
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        void StoreValue(string key, object value);
    }
}