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

        private T Store<T>(IDocumentSession session, string key, DateTime expiry, Func<T> getFromRiotFunc)
        {
            return Store<T>(session, default(T), key, expiry, getFromRiotFunc);
        }

        private T Store<T>(IDocumentSession session, T entity, string key, DateTime expiry, Func<T> getFromRiotFunc)
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
            return entity;
        }

        private T Update<T>(IDocumentSession session, T entity, string key, DateTime expiry, Func<T> getFromRiotFunc)
        {
            logger.Debug($"Didn't find {key} in cache");
            session.Delete(entity);
            session.SaveChanges();
            entity = getFromRiotFunc.Invoke();
            if (entity != null)
            {
                session.Store(entity, id: key);
                session.Advanced.GetMetadataFor(entity)["Raven-Expiration-Date"] = new RavenJValue(expiry);
                session.SaveChanges();
                logger.Debug($"Updated {key} in cache");
            }
            return entity;
        }

        public T Get<T>(string key, DateTime expiry, Func<T> getFromRiotFunc)
        {
            using (var session = _documentStore.OpenSession())
            {
                logger.Debug($"Getting {key} from cache");
                
                var entity = session.Load<T>(key);
                
                if (entity != null)
                {
                    RavenJObject metadata = session.Advanced.GetMetadataFor(entity);

                    // Get the last modified time stamp, which is known to be of type DateTime
                    DateTime expirationDate = metadata.Value<DateTime>("Raven-Expiration-Date");
                    if (expirationDate < DateTime.UtcNow)
                    {
                        entity = Update(session, entity, key, expiry, getFromRiotFunc);
                    }
                }
                else
                {
                    entity = Store(session, key, expiry, getFromRiotFunc);
                    
                }
                return entity;
            }
        }
    }
}