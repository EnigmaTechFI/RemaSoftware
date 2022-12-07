using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RemaSoftware.UtilityServices.FattureInCloud;

public class FicBaseHttp : IFicBaseHttp
{
    private readonly HttpClient _httpClient;
    
    public FicBaseHttp(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        
        var apiToken = configuration["ApiFattureInCloud:AccessToken"];
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + apiToken);
    }
    
    public async Task<bool> Delete(string url)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
        var responseMessage = await _httpClient.DeleteAsync(url);
        if (responseMessage.IsSuccessStatusCode)
            return true;
        return false;
    }

    public async Task<TModel> Get<TModel>(string url) where TModel : class
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
        return await _httpClient.GetFromJsonAsync<TModel>(url);
    }

    public async Task<TModel> Post<TModel>(string url, object data) where TModel : class
    {
        if (string.IsNullOrEmpty(url)) 
            throw new ArgumentNullException(nameof(url));
            
        var response = await _httpClient.PostAsJsonAsync(url, data);

        var contentAsString = await response.Content.ReadAsStringAsync();
            
        try
        {
            var deserialize = JsonSerializer.Deserialize<TModel>(contentAsString);
            return deserialize;
        }
        catch(Exception e)
        {
            return null;
        }
    }

    public async Task<TModel> Put<TModel>(string url, object data) where TModel : class
    {
        if (string.IsNullOrEmpty(url)) 
            throw new ArgumentNullException(nameof(url));
            
        var response = await _httpClient.PutAsJsonAsync(url, data);

        var contentAsString = await response.Content.ReadAsStringAsync();
            
        try
        {
            var deserialize = JsonSerializer.Deserialize<TModel>(contentAsString);
            return deserialize;
        }
        catch(Exception e)
        {
            return null;
        }
    }
}