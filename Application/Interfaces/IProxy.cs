using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Application.Interfaces
{
  public abstract class IProxy
  {        
    private string _url;        
    private HttpClient _httpClient;  
    private readonly IConfiguration _configuration;

    public IProxy(IConfiguration configuration, string url)
    {      
      _url = url;
      _httpClient = new HttpClient();
      _configuration = configuration;
    }

    public async Task<T> GetAsync<T>(string recurso)
    {      
      _httpClient.BaseAddress = new System.Uri(_url);      
      var result = await _httpClient.GetAsync($"{recurso}");
      if (result.StatusCode == System.Net.HttpStatusCode.OK)
      {
        var deserialized = await result.Content.ReadFromJsonAsync<T>();
        return deserialized;
      }
      if (result.StatusCode == System.Net.HttpStatusCode.NotFound) {
        throw new Exception($"Conta sem movimentação status code:{result.StatusCode}");
      }
      throw new Exception("Não foi possível retornar consolidado");
    }

    public T Get<T>(string recurso)
    {      
      _httpClient.BaseAddress = new System.Uri(_url);      
      var result = _httpClient.GetAsync($"{recurso}");
      result.RunSynchronously();

      if (result.Result.StatusCode == System.Net.HttpStatusCode.OK)
      {
        var deserialized = result.Result.Content.ReadFromJsonAsync<T>();
        deserialized.RunSynchronously();

        return deserialized.Result;
      }
      if (result.Result.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        throw new Exception($"Conta sem movimentação status code:{result.Result.StatusCode}");
      }
      throw new Exception("Não foi possível retornar consolidado");      
    }
  }
}
