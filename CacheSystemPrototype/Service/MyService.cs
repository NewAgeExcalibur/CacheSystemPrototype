using System;
using CacheSystemPrototype.Infrastructure.Database;

namespace CacheSystemPrototype.Service
{
    /// <summary>
    /// this service is responsible to retreive the requested data from repository
    /// </summary>
    public class MyService : IMyService
    {
        private readonly Repository repository;
   
        public MyService(Repository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
          
            this.repository = repository;
            
        }

        /// <summary>
        /// Get data based on key
        /// </summary>
        /// <param name="key">unique identifier</param>
        /// <returns></returns>
        public string GetItem(string key)
        {
            var value = repository.GetValue(key);

            if(value != null)
            {
                return value.ToString();
            }

            return null;
        }
    }
}