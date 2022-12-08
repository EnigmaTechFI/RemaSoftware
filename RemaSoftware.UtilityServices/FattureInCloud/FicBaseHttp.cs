using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RemaSoftware.UtilityServices.Exceptions;
using RemaSoftware.UtilityServices.FicResponses;

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
        if (!responseMessage.IsSuccessStatusCode)
            throw new FattureInCloudException($"Errore durante l'eliminazione del cliente su FattureInCloud. {responseMessage.ToString()}");
        return true;
    }

    public async Task<TModel> Get<TModel>(string url) where TModel : class
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
        return await _httpClient.GetFromJsonAsync<TModel>(url);
    }

    public async Task<TModel> Post<TModel>(string url, object data) where TModel : BaseErrorFicResponse
    {
        if (string.IsNullOrEmpty(url)) 
            throw new ArgumentNullException(nameof(url));
            
        var response = await _httpClient.PostAsJsonAsync(url, data);

        var contentAsString = await response.Content.ReadAsStringAsync();

        var deserialize = JsonSerializer.Deserialize<TModel>(contentAsString);
        if (deserialize == null || deserialize.Error != null)
            throw new FattureInCloudException($"Errore durante la creazione del cliente su FattureInCloud. Error: {deserialize.Error.ErrorMessage} {deserialize.Error.ValidationErrors}.");
        return deserialize;
    }

    public async Task<TModel> Put<TModel>(string url, object data) where TModel : BaseErrorFicResponse
    {
        if (string.IsNullOrEmpty(url)) 
            throw new ArgumentNullException(nameof(url));
            
        var response = await _httpClient.PutAsJsonAsync(url, data);

        var contentAsString = await response.Content.ReadAsStringAsync();
        
        var deserialize = JsonSerializer.Deserialize<TModel>(contentAsString);
        
        if (deserialize == null || deserialize.Error != null)
            throw new FattureInCloudException($"Errore durante la creazione del cliente su FattureInCloud. Error: {deserialize.Error.ErrorMessage} {deserialize.Error.ValidationErrors}.");
        return deserialize;
    }
}