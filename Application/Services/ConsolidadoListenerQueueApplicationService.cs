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
    IRavenRepositoryExtractConsolidated _ravenRepositoryExtractConsolidated;
    IConfiguration _config;
    public ConsolidadoListenerQueueApplicationService(
      ILogger<ConsolidadoListenerQueueApplicationService> logger, 
      IConfiguration configuration, 
      IRepositoryExtractConsolidated repositoryExtractConsolidated,
      IRavenRepositoryExtractConsolidated ravenRepositoryExtractConsolidated)
    {
      _logger = logger;
      _repositoryExtractConsolidated = repositoryExtractConsolidated;
      _ravenRepositoryExtractConsolidated = ravenRepositoryExtractConsolidated;
      _config = configuration;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
      if (bool.Parse(_config.GetSection("AMQP:Activated").Value) == true)
      {
        await ConsolidadoListener();
        await SyncRavenFromSQL();
      }      
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
    
    private async Task ConsolidadoListener()
    {
      var amqpPort = _config.GetSection("AMQP:Port").Value != null ? int.Parse(_config.GetSection("AMQP:Port").Value) : 5672;
      var factory = new ConnectionFactory() { HostName = _config.GetSection("AMQP:Hostname").Value, UserName = "guest", Password = "guest", Port = amqpPort };

      var connection = factory.CreateConnection();
      var channel = connection.CreateModel();
              
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
          if (fluxoCaixaDiarioConsolidado !=null){
            var entity = new ExtractConsolidated
            {
                AccountId = fluxoCaixaDiarioConsolidado.IdAccount.Id,
                Date = fluxoCaixaDiarioConsolidado.Created,
                Extract = message
            };

            await PersistSQLFromQueue(fluxoCaixaDiarioConsolidado, entity);
            await PersistRavenFromQueue(entity);
          }
      };

      channel.BasicConsume(queue: "queueFluxo",
                            autoAck: true,
                            consumer: consumerFluxoCaixaConsolidadoDiario);        
      
    }

    private async Task PersistSQLFromQueue(ConsolidadoResponse fluxoCaixaDiarioConsolidado, ExtractConsolidated entity)
    {
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
    }

    private async Task PersistRavenFromQueue(ExtractConsolidated entity)
    {                  
      var item = await _ravenRepositoryExtractConsolidated.GetById(entity.Id);
      if (item == null){
        item = new RavenExtractConsolidated{
          AccountId = entity.AccountId,
          Date = entity.Date,
          Extract = entity.Extract,
          ExtractConsolidatedId = entity.Id
        };
        await _ravenRepositoryExtractConsolidated.Add(item);
      }else{
        await PersistUpdateRavenFromQueue(entity);
      }
    }
    private async Task PersistUpdateRavenFromQueue(ExtractConsolidated entity)
    {            
      var item = new RavenExtractConsolidated{
          AccountId = entity.AccountId,
          Date = entity.Date,
          Extract = entity.Extract,
          ExtractConsolidatedId = entity.Id
        };
      await _ravenRepositoryExtractConsolidated.Update(item);            
    }

    private async Task SyncRavenFromSQL(){
      try{
        var sqlCount = _repositoryExtractConsolidated.All()?.Count();
        var ravenCount = _ravenRepositoryExtractConsolidated.AllRaven().Count();
        if (sqlCount != ravenCount){          
          var items = _repositoryExtractConsolidated.All()?.ToArray();
          if (items != null)
          {
            foreach(var extract in items){
              await PersistRavenFromQueue(extract);
            }
          }          
        }
      }catch(Exception e){
        _logger.LogError("Listener service: Exception {0}", e);
      }
    }
  }
}
