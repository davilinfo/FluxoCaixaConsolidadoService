using Application.Interfaces;
using Application.Proxy;
using Application.Services;
using Domain.Contract;
using Microsoft.OpenApi.Models;
using Persistence.Context;
using Persistence.Repository;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IRepositoryExtractConsolidated, RepositoryExtractConsolidated>();

builder.Services.AddScoped<IFluxoConsolidadoApplicationService, FluxoConsolidadoApplicationService>();
builder.Services.AddScoped<IProxyFluxoConsolidado, ProxyFluxoConsolidado>();
builder.Services.AddHostedService<ConsolidadoListenerQueueApplicationService>();

builder.Services.AddSwaggerGen(gen =>
{
  gen.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "v1",
    Title = "Mansait - Consolidado Carrefour API",
    Description = "Um Web API em ASP.NET Core Web API para geração consolidado de fluxo de caixa. É necessário você estar executando o RabbitMQ na porta padrão para retornar consolidado conta!",
    Contact = new OpenApiContact
    {
      Name = "Davi Lima Alves",
      Url = new Uri("https://linkedin.com/in/davilalves")
    }
  });
  var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  gen.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});
builder.Services.AddDbContext<ConsolidadoContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors(c => c.AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
