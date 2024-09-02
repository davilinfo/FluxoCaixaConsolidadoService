using Application.Interfaces;
using Application.Model.Response;
using Microsoft.Extensions.Configuration;

namespace Application.Proxy
{
  public class ProxyFluxoConsolidado : IProxy, IProxyFluxoConsolidado
  {    
    private readonly bool _requiresBearerToken = true;
    public ProxyFluxoConsolidado(IConfiguration configuration) : base(configuration, configuration.GetSection("FluxoCaixaService:Host").Value)
    { 
    }
    public async Task<ConsolidadoResponse> GetExtratoFluxoCaixaAsync(string recurso, string parametros, string token = null)
    {
      try
      {
        var resource = $"{recurso}/{parametros}";
        var result = await this.GetAsync<ConsolidadoResponse>(resource, _requiresBearerToken, token);
        return result;
      }catch(Exception e)
      {
        if (e.Message.Contains("status code:NotFound")){
          return new ConsolidadoResponse { };
        }
        throw e;
      }
    }
  }
}
