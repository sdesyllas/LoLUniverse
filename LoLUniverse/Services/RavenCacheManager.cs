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
        private IRiotClient _riotClient;

        public RavenCacheManager(IDocumentStore documentStore, IRiotClient riotClient)
        {
            _documentStore = documentStore;
            _riotClient = riotClient;
        }


        public T Get<T>(string key, DateTime expiry, Func<T> GetFromRiotFunc)
        {
            using (var session = _documentStore.OpenSession())
            {
                logger.Debug($"Getting {key} from cache");
                
                var entity = session.Load<T>(key);
                if (entity == null)
                {
                    logger.Debug($"Didn't find {key} in cache");
                    entity = GetFromRiotFunc.Invoke();
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