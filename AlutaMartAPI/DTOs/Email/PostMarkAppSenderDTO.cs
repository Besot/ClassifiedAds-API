using System.Text.Json.Serialization;

namespace AlutaMartAPI.DTOs;

public class PostMarkAppSenderDTO
{
    [JsonPropertyName("From")]
    public string From { get; set; }

    [JsonPropertyName("To")]
    public string To { get; set; }

    [JsonPropertyName("Subject")]
    public string Subject { get; set; }

    [JsonPropertyName("TextBody")]
    public string TextBody { get; set; }

    [JsonPropertyName("HtmlBody")]
    public string HtmlBody { get; set; }

    [JsonPropertyName("MessageStream")]
    public string MessageStream { get; set; }
}