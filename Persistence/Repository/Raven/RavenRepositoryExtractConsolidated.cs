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
            if (_context.IsRavenSet){
                using(IDocumentSession session = _context.store.OpenSession()){
                    session.Store(entidade);
                    session.SaveChanges();
                }
                return entidade.ExtractConsolidatedId;
            }
            return new Guid();
        }

        public IQueryable<RavenExtractConsolidated> All()
        {
            if (_context.IsRavenSet){
                IDocumentSession session = _context.store.OpenSession();
                return session.Query<RavenExtractConsolidated>().AsNoTracking();
            }
            return null;
        }

        public IRavenQueryable<RavenExtractConsolidated> AllRaven()
        {
            if (_context.IsRavenSet){                
                IDocumentSession session = _context.store.OpenSession();
                var query = session.Query<RavenExtractConsolidated>();
                var list = query.ToList();
                return query;
            }
            return null;
        }

        public Task<int> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<RavenExtractConsolidated> GetByDate(DateTime? date, int numberOfRows = 50, int page = 0)
        {
            if (_context.IsRavenSet){
                var searchDate = date != null ? date.Value.Date : DateTime.Today;
                using(IDocumentSession session = _context.store.OpenSession()){
                    return session
                    .Query<RavenExtractConsolidated>()
                    .Where(e=> e.Date == searchDate)
                    .Skip(page).Take(numberOfRows)
                    .ToList();
                }
            }
            return null;
        }

        public async Task<RavenExtractConsolidated> GetById(Guid id)
        {
            if (_context.IsRavenSet){
                using(IDocumentSession session = _context.store.OpenSession()){

                    var current = session
                    .Query<RavenExtractConsolidated>()
                    .FirstOrDefault(e=> e.ExtractConsolidatedId == id);
    #pragma warning disable CS8603 // Possible null reference return.
                    return current;
    #pragma warning restore CS8603 // Possible null reference return.
                }
            }
            return null;
        }

        public async Task<int> Update(RavenExtractConsolidated entidade)
        {
            var result=0;
            if (_context.IsRavenSet){
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
            }
            return result;
        }
    }

}