using Raven.Client;
using Raven.Client.Document;

namespace LoLUniverse.NoSqlContext
{
    public static class RavenContext
    {
        public static readonly IDocumentStore Db = new DocumentStore
        {
            ConnectionStringName = "RavenDbNoSql",
        }.Initialize();

        public static IDocumentSession CreateSession()
        {
            return Db.OpenSession();
        }
    }
}