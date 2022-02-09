using Raven.Client.Documents;
using Raven.Embedded;

namespace AniGate.RavenDataAccess
{
    public static class RavenDb
    {
        public static IDocumentStore InitializeStore(string dataDirectory)
        {
            var ravenOptions = new ServerOptions
            {
                DataDirectory = dataDirectory,
                ServerUrl = "http://localhost:10000"
            };
            EmbeddedServer.Instance.StartServer(ravenOptions);
            return EmbeddedServer.Instance.GetDocumentStore("Embedded");
        }
    }
}
