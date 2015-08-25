using Ninject;
using NLog;
using Raven.Client;
using Raven.Json.Linq;
using RiotApi.Net.RestClient;
using System;
using System.Collections.Generic;

namespace LoLUniverse.Services
{
    public class RavenCacheManager : ICacheManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IDocumentStore _documentStore;

        public RavenCacheManager(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }


        public T Get<T>(string key, DateTime expiry, Func<T> getFromRiotFunc)
        {
            using (var session = _documentStore.OpenSession())
            {
                logger.Debug($"Getting {key} from cache");
                
                var entity = session.Load<T>(key);
                if (entity == null)
                {
                    logger.Debug($"Didn't find {key} in cache");
                    entity = getFromRiotFunc.Invoke();
                    if (entity != null)
                    {
                        session.Store(entity, id: key);
                        session.Advanced.GetMetadataFor(entity)["Raven-Expiration-Date"] = new RavenJValue(expiry);
                        session.SaveChanges();
                        logger.Debug($"Stored {key} in cache");
                    }
                }
                return entity;
            }
        }
    }
}