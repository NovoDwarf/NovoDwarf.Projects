namespace SmartHome.Shared;

public sealed class BotContext
{
    public required string ChatId { get; init; }
    public required string Text { get; init; }

    public required Func<string, CancellationToken, Task> ReplyAsync { get; init; }
}
