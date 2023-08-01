using Application.Model.Response;

namespace Application.Interfaces
{
  public interface IProxyFluxoConsolidado
  {
    Task<ConsolidadoResponse> GetExtratoFluxoCaixaAsync(string recurso, string parametros);
  }
}
