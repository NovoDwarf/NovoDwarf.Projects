namespace SmartHome.MAX;

public sealed class MaxBotOptions
{
    public required string Token { get; init; }
    public string BaseUrl { get; init; } = "https://api.max.ru";
    public string[] AllowedUserIds { get; init; } = [];
}
