using Domain.Contract;
using Domain.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context;
using System.Diagnostics.CodeAnalysis;

namespace Persistence.Repository
{
  [ExcludeFromCodeCoverage]
  public class RepositoryExtractConsolidated : IRepositoryExtractConsolidated
  {
    private readonly ConsolidadoContext _context;
    public RepositoryExtractConsolidated(IConfiguration configuration)
    {
      var options = new DbContextOptions<ConsolidadoContext>();
      _context = new ConsolidadoContext(configuration, options);
    }
    public async Task<Guid> Add(ExtractConsolidated entidade)
    {
      var result = _context.Add(entidade);
      await _context.SaveChangesAsync();
      return result.Entity.Id;
    }

    public IQueryable<ExtractConsolidated> All()
    {
      return _context.Extracts.AsNoTracking();
    }

    public async Task<int> Delete(Guid id)
    {
      var entity = _context.Extracts.Find(id);
      if (entity != null)
      {
        _context.Extracts.Remove(entity);
      }
      return await _context.SaveChangesAsync();
    }

    public async Task<ExtractConsolidated> GetById(Guid id)
    {
      var result = await _context.Extracts.FirstOrDefaultAsync(t => t.Id == id);
      return result;
    }

    public async Task<int> Update(ExtractConsolidated entidade)
    {
      var entity = _context.Extracts.Find(entidade.Id);
      if (entity != null)
      {
        entity.Date = entidade.Date;
        entity.Extract = entidade.Extract;
        _context.Extracts.Update(entity);
        return await _context.SaveChangesAsync();
      }
      return 0;
    }
  }
}
