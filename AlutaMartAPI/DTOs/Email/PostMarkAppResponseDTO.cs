using System.Text.Json.Serialization;

namespace AlutaMartAPI.DTOs;

public class PostMarkAppResponseDTO
{
    [JsonPropertyName("To")]
    public string To { get; set; }

    [JsonPropertyName("SubmittedAt")]
    public DateTimeOffset SubmittedAt { get; set; }

    [JsonPropertyName("MessageID")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("ErrorCode")]
    public long ErrorCode { get; set; }

    [JsonPropertyName("Message")]
    public string Message { get; set; }
}