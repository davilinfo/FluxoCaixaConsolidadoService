using Application.Interfaces;
using Application.Proxy;
using Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFluxoConsolidadoApplicationService, FluxoConsolidadoApplicationService>();
builder.Services.AddScoped<IProxyFluxoConsolidado, ProxyFluxoConsolidado>();
builder.Services.AddHostedService<ConsolidadoListenerQueueApplicationService>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
