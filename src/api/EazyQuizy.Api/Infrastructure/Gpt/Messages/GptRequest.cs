using System.Text.Json.Serialization;

namespace EazyQuizy.Api.Infrastructure.Gpt.Messages;

public class GptRequest
{
    public GptRequest(string? systemText,string message)
    {
        if(!string.IsNullOrEmpty(systemText))
            Messages.Add(new Message(systemText, "system"));
        Messages.Add(new (message, null));
    }

    [JsonPropertyName("messages")] 
    public List<Message> Messages { get; init; } = new();

    [JsonPropertyName("model")]
    public string Model { get; set; } = "GigaChat";
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 512;
    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 1.0f;
}
