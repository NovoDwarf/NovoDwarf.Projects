namespace SmartHome.Shared.Commands;

public interface ICommandRouter
{
    Task RouteAsync(BotContext context, CancellationToken ct = default);
}
