using System.Text.Json.Serialization;

namespace RemaSoftware.UtilityServices.Dtos;

public class ClientDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("vat_number")]
    public string P_Iva { get; set; }
    
    [JsonPropertyName("address_street")]
    public string Street { get; set; }
    
    [JsonPropertyName("address_postal_code")]
    public string Cap { get; set; }
    [JsonPropertyName("address_city")]
    public string City { get; set; }
    [JsonPropertyName("address_province")]
    public string Province { get; set; }
    [JsonPropertyName("country")]
    public string Nation { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("phone")]
    public string PhoneNumber { get; set; }
    [JsonPropertyName("fax")]
    public string Fax { get; set; }
}