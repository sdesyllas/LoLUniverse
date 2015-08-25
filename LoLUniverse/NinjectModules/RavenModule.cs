using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Web.Common;
using Raven.Client;
using Raven.Client.Document;

namespace LoLUniverse.NinjectModules
{
    public class RavenModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDocumentStore>()
            .ToMethod(InitDocStore)
            .InSingletonScope();

            Bind<IDocumentSession>()
                .ToMethod(c => c.Kernel.Get<IDocumentStore>().OpenSession())
                .InRequestScope();
        }

        private IDocumentStore InitDocStore(IContext context)
        {
            IDocumentStore store = new DocumentStore() { ConnectionStringName = "RavenDbNoSql" };
            store.Initialize();
            return store;
        }
    }
}