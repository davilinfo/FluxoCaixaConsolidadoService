using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Persistence.Context.Raven
{
    public class RavenConsolidadoContext{
        IConfiguration _configuration;
        const string _connectionString = "RavenDefaultConnection";
        public IDocumentStore store {get;private set;}
        public bool IsRavenSet {get; private set;}
        public RavenConsolidadoContext(IConfiguration configuration){
            _configuration = configuration;
            IsRavenSet = _configuration.GetSection("RavenDB") == null ? false : (_configuration.GetSection("RavenDB").Value.Equals("true", StringComparison.CurrentCultureIgnoreCase) ? true : false);
            if (IsRavenSet){
                store = new DocumentStore{
                    Urls = new[]{
                        _configuration.GetConnectionString(_connectionString)
                    }
                ,
                Database = "FluxoConsolidado",
                Conventions = {}
                };
                store.Initialize();                             
            }
        }
    }
}