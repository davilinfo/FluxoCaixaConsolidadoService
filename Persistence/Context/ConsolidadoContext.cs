using Domain.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Context
{
  public class ConsolidadoContext : DbContext
  {
    IConfiguration _configuration;
    const string _connectionString = "DefaultConnection";

    public DbSet<ExtractConsolidated> Extracts { get; set; }

    public ConsolidadoContext(IConfiguration configuration, DbContextOptions<ConsolidadoContext> options) : base(options)
    {
      _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(_configuration.GetConnectionString(_connectionString), o=> o.EnableRetryOnFailure());
    }
  }
}