using Application.Interfaces;
using Application.Model.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Text.Json;

namespace FluxoCaixaConsolidado.Controllers
{
  /// <summary>
  /// Classe responsável por gerar e armazenar consolidado de fluxo de conta
  /// </summary>
  [Authorize]
  [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
  [ApiController]
  [Route("[controller]")]
  [Produces("application/json")]
  public class FluxoCaixaConsolidadoController : ControllerBase
  {
    
    private readonly ILogger<FluxoCaixaConsolidadoController> _logger;
    private readonly IFluxoConsolidadoApplicationService _fluxoConsolidadoApplicationService;

    /// <summary>
    /// Construtor do consolidado de fluxo de caixa
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="fluxoConsolidadoApplicationService"></param>
    public FluxoCaixaConsolidadoController(ILogger<FluxoCaixaConsolidadoController> logger, IFluxoConsolidadoApplicationService fluxoConsolidadoApplicationService)
    {
      _logger = logger;
      _fluxoConsolidadoApplicationService = fluxoConsolidadoApplicationService;
    }

    /// <summary>
    /// Para retornar o extrato diário informe o identificador da conta, o dia, mês e ano no formato ddmmaaaa
    /// </summary>
    /// <param name="request">Account id, email e dia</param>
    /// <returns></returns>
    [HttpGet("GetConsolidado", Name = "GetConsolidado")]
    public async Task<IActionResult> GetConsolidado([FromQuery]GetConsolidadoRequest request)
    {
      _logger.LogInformation($"Get consolidado request: {JsonSerializer.Serialize(request)}");
      try
      {
        Guid id;
        if (!Guid.TryParse(request.AccountId, out id))
        {
          ModelState.AddModelError("Guid", "Identificador em formato inválido => guid");
        }
        DateTime date;
        if (!DateTime.TryParse($"{int.Parse(request.DiaMesAno.Substring(4))}/{int.Parse(request.DiaMesAno.Substring(2, 2))}/{int.Parse(request.DiaMesAno.Substring(0, 2))}", out date))
        {
          ModelState.AddModelError("diamesano", "Data inválida");
        }
        if (ModelState.IsValid)
        {
          var token = HttpContext.Request.Headers.Authorization.FirstOrDefault();
          var result = await _fluxoConsolidadoApplicationService.GetConsolidado(request, token);
          if (result.IdAccount == null)
          {
            return NotFound("Conta ainda sem movimentação");
          }
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
        var internalServerError = new JsonResult($"Aconteceu o seguinte erro: Não foi possível retornar consolidado! {e.Message}");
        internalServerError.StatusCode = 500;
        return internalServerError;
      }
    }
  }
}