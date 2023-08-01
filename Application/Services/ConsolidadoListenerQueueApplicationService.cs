using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Application.Model.Response;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
  public class ConsolidadoListenerQueueApplicationService : IHostedService
  {
    ILogger<ConsolidadoListenerQueueApplicationService> _logger;
    public ConsolidadoListenerQueueApplicationService(ILogger<ConsolidadoListenerQueueApplicationService> logger)
    {
      _logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConsolidadoListener();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    private async Task ConsolidadoListener()
    {
      var factory = new ConnectionFactory() { HostName = "localhost" };
      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {        
        channel.QueueDeclare(queue: "queueFluxo",
                          durable: false,
                          exclusive: false,
                          autoDelete: false,
                          arguments: null);

        var consumerFluxoCaixaConsolidadoDiario = new EventingBasicConsumer(channel);
        consumerFluxoCaixaConsolidadoDiario.Received += (model, ea) =>
        {          
          var body = ea.Body.ToArray();
          var message = Encoding.UTF8.GetString(body);
          _logger.LogInformation($"fluxo consolidado recebido {message}");

          var fluxoCaixaDiarioConsolidado = System.Text.Json.JsonSerializer.Deserialize<ConsolidadoResponse>(message);          
        };

        channel.BasicConsume(queue: "queueFluxo",
                              autoAck: true,
                              consumer: consumerFluxoCaixaConsolidadoDiario);        
      }
    }
  }
}
