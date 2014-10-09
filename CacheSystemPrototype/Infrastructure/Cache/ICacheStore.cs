

namespace CacheSystemPrototype.Infrastructure.Cache
{
    /// <summary>
    /// an interface to define general required cache operation
    /// </summary>
    public interface ICacheStore
    {
        /// <summary>
        /// Store an entity into cache
        /// Store can happen Asyc to make it processing data faster, for now to keep it simple we are doing it sync
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        void StoreValue(string key, object entity);

        /// <summary>
        /// get object from cache based on cache key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetValue(string key);
    }
}