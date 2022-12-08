using System.Text.Json.Serialization;

namespace RemaSoftware.UtilityServices.FicResponses;

public class ClientDtoResponseFic : BaseErrorFicResponse
{
    [JsonPropertyName("data")]
    public ClientData Data { get; set; }
}

public class ClientData
{
    [JsonPropertyName("id")]
    public int FicId { get; set; }
    
    // aggiungere altre prop se ci interessano
}