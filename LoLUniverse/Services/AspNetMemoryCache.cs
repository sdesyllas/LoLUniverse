using System;
using System.Web;
using System.Web.Caching;
using NLog;

namespace LoLUniverse.Services
{
    public class AspNetMemoryCache : IMemoryCache
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public T Get<T>(string key, DateTime expiry, Func<T> getFromRiotFunc)
        {
            T entity = (T)HttpContext.Current.Cache[key];

            if (entity == null)
            {
                logger.Debug($"Didn't find {key} in cache");
                entity = getFromRiotFunc.Invoke();
                if (entity != null)
                {
                    HttpContext.Current.Cache.Insert(key, entity, null, expiry, Cache.NoSlidingExpiration);
                    logger.Debug($"Stored {key} in cache");
                }
            }
            logger.Debug($"Getting {key} from cache");
            return entity;
        }
    }
}