using System.Text.Json.Serialization;

namespace EazyQuizy.Api.Infrastructure.Gpt.Messages;

public class Message(string content, string? role)
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = role ?? "user";

    [JsonPropertyName("content")]
    public string Content { get; set; } = content;
}
