using Domain.Contract;
using Domain.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context.Raven;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Persistence.Repository.Raven{
    public class RavenRepositoryExtractConsolidado : IRavenRepositoryExtractConsolidated
    {
        private readonly RavenConsolidadoContext _context; 
        
        public RavenRepositoryExtractConsolidado(IConfiguration configuration){
            _context = new RavenConsolidadoContext(configuration);
        }
        public async Task<Guid> Add(RavenExtractConsolidated entidade)
        {            
            using(IDocumentSession session = _context.store.OpenSession()){
                session.Store(entidade);
                session.SaveChanges();
            }
            return entidade.ExtractConsolidatedId;        
        }

        public IQueryable<RavenExtractConsolidated> All()
        {            
            IDocumentSession session = _context.store.OpenSession();
            return session.Query<RavenExtractConsolidated>().AsNoTracking();            
        }

        public IRavenQueryable<RavenExtractConsolidated> AllRaven()
        {                          
            IDocumentSession session = _context.store.OpenSession();
            var query = session.Query<RavenExtractConsolidated>();            
            return query;           
        }

        public Task<int> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<RavenExtractConsolidated> GetByDate(DateTime? date, int numberOfRows = 50, int page = 0)
        {            
            var searchDate = date != null ? date.Value.Date : DateTime.Today;
            using(IDocumentSession session = _context.store.OpenSession()){
                return session
                .Query<RavenExtractConsolidated>()
                .Where(e=> e.Date == searchDate)
                .Skip(page).Take(numberOfRows)
                .ToList();
            }        
        }

        public async Task<RavenExtractConsolidated> GetById(Guid id)
        {            
            using(IDocumentSession session = _context.store.OpenSession()){
                var current = session
                .Query<RavenExtractConsolidated>()
                .FirstOrDefault(e=> e.ExtractConsolidatedId == id);
#pragma warning disable CS8603 // Possible null reference return.
                return current;
#pragma warning restore CS8603 // Possible null reference return.
            }        
        }

        public bool IsRavenDbSet()
        {
            return _context.IsRavenSet;
        }

        public async Task<int> Update(RavenExtractConsolidated entidade)
        {
            var result=0;            
            using(IDocumentSession session = _context.store.OpenSession()){

                var current = session
                .Query<RavenExtractConsolidated>()
                .FirstOrDefault(e=> e.ExtractConsolidatedId == entidade.ExtractConsolidatedId);
                if (current != null){
                    current.Date = entidade.Date;
                    current.Extract = entidade.Extract;
                    current.AccountId = entidade.AccountId;

                    session.Store(current);
                    session.SaveChanges();
                    result = 1;
                }
            }            
            return result;
        }
    }

}