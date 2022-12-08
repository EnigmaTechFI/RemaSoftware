using System.Text.Json.Serialization;

namespace RemaSoftware.UtilityServices.FicResponses;

public class BaseErrorFicResponse
{
    [JsonPropertyName("error")]
    public ErrorFic Error { get; set; }
}

public class ErrorFic
{
    [JsonPropertyName("message")]
    public string ErrorMessage { get; set; }
    [JsonPropertyName("validation_result")]
    public object ValidationErrors { get; set; }
}