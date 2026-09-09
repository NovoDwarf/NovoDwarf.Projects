namespace SmartHome.VK;

public sealed class VkBotOptions
{
    public required string Token { get; init; }
    public required ulong GroupId { get; init; }

    /// <summary>If non-empty, only messages from these VK user IDs are processed.</summary>
    public long[] AllowedUserIds { get; init; } = [];
}
