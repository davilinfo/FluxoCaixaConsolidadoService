using Application.Interfaces;
using Application.Model.Response;
using Microsoft.Extensions.Configuration;

namespace Application.Proxy
{
  public class ProxyFluxoConsolidado : IProxy, IProxyFluxoConsolidado
  {    
    public ProxyFluxoConsolidado(IConfiguration configuration) : base(configuration, configuration.GetSection("FluxoCaixaService:Host").Value)
    { 
    }
    public async Task<ConsolidadoResponse> GetExtratoFluxoCaixaAsync(string recurso, string parametros)
    {
      try
      {
        var resource = $"{recurso}/{parametros}";
        var result = await this.GetAsync<ConsolidadoResponse>(resource);
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
