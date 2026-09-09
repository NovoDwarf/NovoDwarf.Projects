using SmartHome.Core;

namespace SmartHome.Telegram.Keyboards;

/// <summary>
/// Encodes/decodes <see cref="BotAction"/> to Telegram's callback_data format.
/// Format: h:{verb}[:{arg1}[:{arg2}]]  (colon-separated, max 64 bytes)
/// </summary>
public sealed class TelegramPayloadCodec : IPayloadCodec
{
    public static readonly TelegramPayloadCodec Instance = new();

    public string Encode(BotAction action) => action.Verb switch
    {
        "noop" => "noop",
        "home" => "h:home",
        "g" => $"h:g:{action.Group}",
        "e" => $"h:e:{action.EntityId}",
        "on" => $"h:on:{action.EntityId}",
        "off" => $"h:off:{action.EntityId}",
        "bri" => $"h:bri:{action.EntityId}:{action.Value}",
        "hum" => $"h:hum:{action.EntityId}:{action.Value}",
        "mode" => $"h:mode:{action.EntityId}:{action.Mode}",
        "play" => $"h:play:{action.EntityId}",
        "pause" => $"h:pause:{action.EntityId}",
        "vol" => $"h:vol:{action.EntityId}:{action.Value}",
        "mute" => $"h:mute:{action.EntityId}:{action.State}",
        "src" => $"h:src:{action.EntityId}:{action.Index}",
        _ => "noop",
    };

    public BotAction? Decode(string? raw)
    {
        if (string.IsNullOrEmpty(raw)) return null;
        if (raw == "noop") return BotAction.Noop();

        if (!raw.StartsWith("h:", StringComparison.Ordinal)) return null;

        // ["h", "bri", "light.bedroom", "128"] from "h:bri:light.bedroom:128"
        var parts = raw.Split(':');
        if (parts.Length < 2) return null;

        var verb = parts[1];
        var rest = parts.Length > 2 ? parts[2..] : [];

        return verb switch
        {
            "home" => BotAction.Home(),
            "g" when rest is [var group] => BotAction.ToGroup(group),
            "e" when rest is [var id] => BotAction.ToEntity(id),
            "on" when rest is [var id] => BotAction.TurnOn(id),
            "off" when rest is [var id] => BotAction.TurnOff(id),
            "bri" when rest is [var id, var v] && int.TryParse(v, out var bri)
                => new BotAction { Verb = "bri", EntityId = id, Value = bri },
            "hum" when rest is [var id, var v] && int.TryParse(v, out var hum)
                => new BotAction { Verb = "hum", EntityId = id, Value = hum },
            "mode" when rest is [var id, var mode]
                => new BotAction { Verb = "mode", EntityId = id, Mode = mode },
            "play" when rest is [var id]
                => new BotAction { Verb = "play", EntityId = id },
            "pause" when rest is [var id]
                => new BotAction { Verb = "pause", EntityId = id },
            "vol" when rest is [var id, var v] && int.TryParse(v, out var vol)
                => new BotAction { Verb = "vol", EntityId = id, Value = vol },
            "mute" when rest is [var id, var s] && int.TryParse(s, out var mute)
                => new BotAction { Verb = "mute", EntityId = id, State = mute },
            "src" when rest is [var id, var i] && int.TryParse(i, out var idx)
                => new BotAction { Verb = "src", EntityId = id, Index = idx },
            _ => null,
        };
    }
}
