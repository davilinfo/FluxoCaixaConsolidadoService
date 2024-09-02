using Application.Interfaces;
using Application.Model.Request;
using Application.Model.Response;
using Domain.Contract;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Application.Services
{
  [ExcludeFromCodeCoverage]
  public class FluxoConsolidadoApplicationService : IFluxoConsolidadoApplicationService
  {
    private int _tentativasRetry = 5;
    private readonly IProxyFluxoConsolidado _proxyFluxoConsolidado;
    private readonly IRepositoryExtractConsolidated _repositoryExtractConsolidated;
    public FluxoConsolidadoApplicationService(IProxyFluxoConsolidado proxyFluxoConsolidado, IRepositoryExtractConsolidated repositoryExtractConsolidated) 
    { 
      _proxyFluxoConsolidado = proxyFluxoConsolidado;
      _repositoryExtractConsolidated = repositoryExtractConsolidated;
    }
    public async Task<ConsolidadoResponse> GetConsolidado(GetConsolidadoRequest request, string token)
    {
      try
      {
        DateTime date;
        if (!DateTime.TryParse($"{int.Parse(request.DiaMesAno.Substring(4))}/{int.Parse(request.DiaMesAno.Substring(2, 2))}/{int.Parse(request.DiaMesAno.Substring(0, 2))}", out date))
        {
          throw new Exception("Data inválida");
        }

        var extract = _repositoryExtractConsolidated.All().OrderByDescending(c=>c.Date).FirstOrDefault(c => c.AccountId == Guid.Parse(request.AccountId) && c.Date.Date <= date);
        if (extract == null) {
          var result = await _proxyFluxoConsolidado.GetExtratoFluxoCaixaAsync("FluxoCaixa", $"GetExtrato?AccountId={request.AccountId}&DiaMesAno={request.DiaMesAno}", token);

          return result;
        }

        return JsonSerializer.Deserialize<ConsolidadoResponse>(extract.Extract);
        
      }
      catch (Exception e)
      {
        if (_tentativasRetry > 0)
        {
          _tentativasRetry--;
          return await GetConsolidado(request, token);
        }

        throw e;
      }
    }
  }
}
