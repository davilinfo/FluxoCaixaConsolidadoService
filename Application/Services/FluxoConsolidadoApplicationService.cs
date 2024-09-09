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
    private readonly IRavenRepositoryExtractConsolidated _ravenRepositoryExtractConsolidated;
    public FluxoConsolidadoApplicationService(
      IProxyFluxoConsolidado proxyFluxoConsolidado, 
      IRepositoryExtractConsolidated repositoryExtractConsolidated,
      IRavenRepositoryExtractConsolidated ravenRepositoryExtractConsolidated) 
    { 
      _proxyFluxoConsolidado = proxyFluxoConsolidado;
      _repositoryExtractConsolidated = repositoryExtractConsolidated;
      _ravenRepositoryExtractConsolidated = ravenRepositoryExtractConsolidated;
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
        if (_ravenRepositoryExtractConsolidated.IsRavenDbSet()){
          var extract = _ravenRepositoryExtractConsolidated.AllRaven().OrderByDescending(c=>c.Date).FirstOrDefault(c=> c.AccountId == Guid.Parse(request.AccountId) && c.Date.Date <= date);
          if (extract == null){
            return await GetResponseFromFluxoCaixa(request, token);
          }
#pragma warning disable CS8603 // Possible null reference return.
          return JsonSerializer.Deserialize<ConsolidadoResponse>(extract.Extract);
#pragma warning restore CS8603 // Possible null reference return.
        }else{
          var extract = _repositoryExtractConsolidated.All().OrderByDescending(c=>c.Date).FirstOrDefault(c => c.AccountId == Guid.Parse(request.AccountId) && c.Date.Date <= date);
          if (extract == null){
            return await GetResponseFromFluxoCaixa(request, token);
          }
#pragma warning disable CS8603 // Possible null reference return.
          return JsonSerializer.Deserialize<ConsolidadoResponse>(extract.Extract);
#pragma warning restore CS8603 // Possible null reference return.
        }                
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
    private async Task<ConsolidadoResponse> GetResponseFromFluxoCaixa (GetConsolidadoRequest request, string token){      
        var result = await _proxyFluxoConsolidado.GetExtratoFluxoCaixaAsync("FluxoCaixa", $"GetExtrato?AccountId={request.AccountId}&DiaMesAno={request.DiaMesAno}", token);
        return result;      
    }
  }
}
