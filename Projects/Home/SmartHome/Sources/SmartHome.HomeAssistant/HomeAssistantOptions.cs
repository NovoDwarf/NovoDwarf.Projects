namespace SmartHome.HomeAssistant;

public sealed class HomeAssistantOptions
{
    public required string BaseUrl { get; init; }
    public required string Token { get; init; }
}
