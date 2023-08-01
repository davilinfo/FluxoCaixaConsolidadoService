using Application.Interfaces;
using Application.Model.Request;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FluxoCaixaConsolidado.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class FluxoCaixaConsolidadoController : ControllerBase
  {
    
    private readonly ILogger<FluxoCaixaConsolidadoController> _logger;
    private readonly IFluxoConsolidadoApplicationService _fluxoConsolidadoApplicationService;

    public FluxoCaixaConsolidadoController(ILogger<FluxoCaixaConsolidadoController> logger, IFluxoConsolidadoApplicationService fluxoConsolidadoApplicationService)
    {
      _logger = logger;
      _fluxoConsolidadoApplicationService = fluxoConsolidadoApplicationService;
    }

    /// <summary>
    /// Para retornar o extrato diário informe o dia mês e ano no formato ddmmaaaa
    /// </summary>
    /// <param name="request">Account number, email e dia</param>
    /// <returns></returns>
    [HttpGet("GetConsolidado", Name = "GetConsolidado")]
    public async Task<IActionResult> GetConsolidado([FromQuery]GetConsolidadoRequest request)
    {
      _logger.LogInformation($"Get consolidado request: {JsonSerializer.Serialize(request)}");
      try
      {
        if (ModelState.IsValid)
        {
          var result = _fluxoConsolidadoApplicationService.GetConsolidado(request);
          return Ok(result);
        }
        foreach (var item in ModelState.Values)
        {
          foreach (var erro in item.Errors)
          {
            _logger.LogError($"Date: {DateTime.UtcNow}, Error: requisição inválida {erro.ErrorMessage}");
          }
        }
        return BadRequest(ModelState);
      }
      catch (Exception e)
      {
        _logger.LogError($"{e.Message}", e);
        var internalServerError = new JsonResult($"Aconteceu o seguinte erro: {e.Message}");
        internalServerError.StatusCode = 500;
        return internalServerError;
      }
    }
  }
}