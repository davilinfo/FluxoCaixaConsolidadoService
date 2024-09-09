using Domain.EF;
using Raven.Client.Documents.Linq;

namespace Domain.Contract
{
  public interface IRavenRepositoryExtractConsolidated : IRepository<RavenExtractConsolidated>
  {
    List<RavenExtractConsolidated> GetByDate(DateTime? date, int numberOfRows=50, int page=0);
    IRavenQueryable<RavenExtractConsolidated> AllRaven();
    bool IsRavenDbSet();
  }
}
