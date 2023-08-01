using Application.Model.Request;
using Application.Model.Response;

namespace Application.Interfaces
{
  public interface IFluxoConsolidadoApplicationService
  {
    Task<ConsolidadoResponse> GetConsolidado(GetConsolidadoRequest request);
  }
}
