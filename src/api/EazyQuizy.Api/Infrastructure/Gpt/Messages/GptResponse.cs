using System.Text.Json.Serialization;

namespace EazyQuizy.Api.Infrastructure.Gpt.Messages;

public class GptResponse
{
    [JsonPropertyName("choices")]
    public Choice[]? Choices { get; set; }
}

public class Choice
{
    [JsonPropertyName("message")]
    public GptResponseMessage? Message { get; set; }
}

public class GptResponseMessage
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
