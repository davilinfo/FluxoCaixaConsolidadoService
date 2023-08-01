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
      var deserialized = await result.Content.ReadFromJsonAsync<T>();

      return deserialized;
    }

    public T Get<T>(string recurso)
    {      
      _httpClient.BaseAddress = new System.Uri(_url);      
      var result = _httpClient.GetAsync($"{recurso}");
      result.RunSynchronously();

      var deserialized = result.Result.Content.ReadFromJsonAsync<T>();
      deserialized.RunSynchronously();

      return deserialized.Result;
    }
  }
}
