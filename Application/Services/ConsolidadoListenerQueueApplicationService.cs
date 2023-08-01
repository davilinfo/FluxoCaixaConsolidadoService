using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Application.Model.Response;
using Microsoft.Extensions.Logging;
using Domain.Contract;
using Domain.EF;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
  public class ConsolidadoListenerQueueApplicationService : IHostedService
  {
    ILogger<ConsolidadoListenerQueueApplicationService> _logger;
    IRepositoryExtractConsolidated _repositoryExtractConsolidated;
    IConfiguration _config;
    public ConsolidadoListenerQueueApplicationService(ILogger<ConsolidadoListenerQueueApplicationService> logger, IConfiguration configuration, IRepositoryExtractConsolidated repositoryExtractConsolidated)
    {
      _logger = logger;
      _repositoryExtractConsolidated = repositoryExtractConsolidated;
      _config = configuration;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
      if (bool.Parse(_config.GetSection("AMQP:Activated").Value) == true)
      {
        await ConsolidadoListener();
      }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    private async Task ConsolidadoListener()
    {
      var amqpPort = _config.GetSection("AMQP:Port").Value != null ? int.Parse(_config.GetSection("AMQP:Port").Value) : 5672;
      var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest", Port = amqpPort };
      
      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {        
        channel.QueueDeclare(queue: "queueFluxo",
                          durable: false,
                          exclusive: false,
                          autoDelete: false,
                          arguments: null);

        var consumerFluxoCaixaConsolidadoDiario = new EventingBasicConsumer(channel);
        consumerFluxoCaixaConsolidadoDiario.Received += async (model, ea) =>
        {          
          var body = ea.Body.ToArray();
          var message = Encoding.UTF8.GetString(body);
          _logger.LogInformation($"fluxo consolidado recebido {message}");

          var fluxoCaixaDiarioConsolidado = System.Text.Json.JsonSerializer.Deserialize<ConsolidadoResponse>(message);
          var entity = new ExtractConsolidated {
            AccountId = fluxoCaixaDiarioConsolidado.IdAccount.Id,
            Date = fluxoCaixaDiarioConsolidado.Created,
            Extract = message
          };

          var extract = _repositoryExtractConsolidated.All().FirstOrDefault(e => e.AccountId == fluxoCaixaDiarioConsolidado.IdAccount.Id && e.Date.Date == fluxoCaixaDiarioConsolidado.Created.Date);
          if (extract != null)
          {
            if (fluxoCaixaDiarioConsolidado.Created > extract.Date)
            {
              entity.Id = extract.Id;
              await _repositoryExtractConsolidated.Update(entity);
            }
          }
          else
          {
            await _repositoryExtractConsolidated.Add(entity);
          }          
        };

        channel.BasicConsume(queue: "queueFluxo",
                              autoAck: true,
                              consumer: consumerFluxoCaixaConsolidadoDiario);        
      }
    }
  }
}
