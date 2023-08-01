using Application.Interfaces;
using Application.Model.Request;
using Application.Model.Response;

namespace Application.Services
{
  public class FluxoConsolidadoApplicationService : IFluxoConsolidadoApplicationService
  {
    private int _tentativasRetry = 5;
    private readonly IProxyFluxoConsolidado _proxyFluxoConsolidado;
    public FluxoConsolidadoApplicationService(IProxyFluxoConsolidado proxyFluxoConsolidado) 
    { 
      _proxyFluxoConsolidado = proxyFluxoConsolidado;
    }
    public async Task<ConsolidadoResponse> GetConsolidado(GetConsolidadoRequest request)
    {
      try
      {
        var result = await _proxyFluxoConsolidado.GetExtratoFluxoCaixaAsync("FluxoCaixa", $"GetExtrato?AccountId={request.AccountId}&DiaMesAno={request.DiaMesAno}");

        return result;
      }
      catch (Exception e)
      {
        if (_tentativasRetry > 0)
        {
          _tentativasRetry--;
          return await GetConsolidado(request);
        }

        throw e;
      }
    }
  }
}
