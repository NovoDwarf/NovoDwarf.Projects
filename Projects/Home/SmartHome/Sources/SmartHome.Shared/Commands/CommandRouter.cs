using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Interfaces;

namespace SmartHome.Shared.Commands;

public sealed class CommandRouter : ICommandRouter
{
    private readonly ILightClient _lights;
    private readonly IEntityClient _entity;

    public CommandRouter(ILightClient lights, IEntityClient entity)
    {
        _lights = lights;
        _entity = entity;
    }

    private const string Help = """
                                Доступные команды:
                                /lights — список устройств освещения
                                /on <entity_id> — включить свет
                                /off <entity_id> — выключить свет
                                /brightness <entity_id> <0-255> — установить яркость
                                """;

    public async Task RouteAsync(BotContext context, CancellationToken ct = default)
    {
        var parts = context.Text.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        var cmd = parts[0].ToLowerInvariant();

        switch (cmd)
        {
            case "/lights":
                await HandleLightsAsync(context, ct);
                break;

            case "/on" when parts.Length >= 2:
                await _lights.TurnLightOnAsync(parts[1], ct: ct);
                await context.ReplyAsync($"✅ {parts[1]} включён.", ct);
                break;

            case "/off" when parts.Length >= 2:
                await _entity.TurnOffAsync(parts[1], ct);
                await context.ReplyAsync($"✅ {parts[1]} выключен.", ct);
                break;

            case "/brightness" when parts.Length >= 3 && int.TryParse(parts[2], out var bri):
                await _lights.TurnLightOnAsync(parts[1], bri, ct);
                await context.ReplyAsync($"✅ {parts[1]}: яркость {bri}.", ct);
                break;

            case "/start":
            case "/help":
                await context.ReplyAsync(Help, ct);
                break;

            default:
                await context.ReplyAsync($"Неизвестная команда.\n\n{Help}", ct);
                break;
        }
    }

    private async Task HandleLightsAsync(BotContext context, CancellationToken ct)
    {
        var lights = await _lights.GetLightsAsync(ct);
        if (lights.Count == 0)
        {
            await context.ReplyAsync("Устройства освещения не найдены.", ct);
            return;
        }

        var lines = lights.Select(l =>
            $"• {l.FriendlyName ?? l.EntityId} [{l.State}]\n  └ {l.EntityId}");

        await context.ReplyAsync(string.Join("\n\n", lines), ct);
    }
}
