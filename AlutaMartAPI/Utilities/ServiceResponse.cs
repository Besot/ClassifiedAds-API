using System.Text.Json.Serialization;

namespace AlutaMartAPI.Utilities;

public class ServiceResponse<T>
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("status")]
    public bool Status { get; set; }

    [JsonPropertyName("data")]
    public T Data { get; set; }

}
