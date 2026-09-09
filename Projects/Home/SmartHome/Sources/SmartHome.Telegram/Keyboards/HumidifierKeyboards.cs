using SmartHome.Core;
using SmartHome.HomeAssistant;
using SmartHome.HomeAssistant.Extensions;
using SmartHome.HomeAssistant.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartHome.Telegram.Keyboards;

internal static class HumidifierKeyboards
{
    private static readonly IPayloadCodec Codec = TelegramPayloadCodec.Instance;

    public static string GroupText(IReadOnlyList<HumidifierWithArea> items)
    {
        var on = items.Count(h => h.State == "on");
        return $"💧 *Климат*\n_{Esc($"{on} из {items.Count} включено")}_\n\nВыберите устройство:";
    }

    public static InlineKeyboardMarkup GroupKeyboard(IReadOnlyList<HumidifierWithArea> items)
    {
        var rows = new List<InlineKeyboardButton[]>();

        var byArea = items
            .GroupBy(h => h.AreaId)
            .OrderBy(g => g.First().AreaName);

        foreach (var group in byArea)
        {
            rows.Add([InlineKeyboardButton.WithCallbackData(
                $"📍 {group.First().AreaName}", Codec.Noop())]);

            var btns = group
                .OrderBy(h => h.FriendlyName ?? h.EntityId)
                .Select(h =>
                {
                    var icon = h.State == "on" ? "💧" : "○";
                    return InlineKeyboardButton.WithCallbackData(
                        $"{icon} {h.FriendlyName ?? h.EntityId}",
                        Codec.Entity(h.EntityId));
                });

            rows.AddRange(btns.Chunk(2).Select(r => r.ToArray()));
        }

        rows.Add([InlineKeyboardButton.WithCallbackData("← Назад", Codec.Home())]);
        return new InlineKeyboardMarkup(rows);
    }

    public static string EntityText(HumidifierWithArea h)
    {
        var isOn = h.State == "on";
        var status = isOn ? "✅ Включён" : "⚫ Выключен";

        var lines = new System.Text.StringBuilder();
        lines.Append($"💧 *{Esc(h.FriendlyName ?? h.EntityId)}*\n");
        lines.Append($"📍 {Esc(h.AreaName)}  •  {status}");

        if (h.CurrentHumidity.HasValue)
            lines.Append($"\n💦 Влажность: *{h.CurrentHumidity}%*");

        if (isOn && h.TargetHumidity.HasValue)
            lines.Append($"  →  цель *{h.TargetHumidity}%*");

        if (isOn && h.Mode is { Length: > 0 })
            lines.Append($"\n🔄 Режим: *{Esc(h.Mode)}*");

        return lines.ToString();
    }

    public static InlineKeyboardMarkup EntityKeyboard(HumidifierWithArea h)
    {
        var isOn = h.State == "on";
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(isOn
            ? [InlineKeyboardButton.WithCallbackData("⚫ Выключить", Codec.TurnOff(h.EntityId))]
            : [InlineKeyboardButton.WithCallbackData("💧 Включить", Codec.TurnOn(h.EntityId))]);

        if (isOn)
        {
            rows.Add([
                InlineKeyboardButton.WithCallbackData("40%", Codec.SetHumidity(h.EntityId, 40)),
                InlineKeyboardButton.WithCallbackData("50%", Codec.SetHumidity(h.EntityId, 50)),
                InlineKeyboardButton.WithCallbackData("60%", Codec.SetHumidity(h.EntityId, 60)),
                InlineKeyboardButton.WithCallbackData("70%", Codec.SetHumidity(h.EntityId, 70)),
            ]);
        }

        if (isOn && h.AvailableModes.Count > 0)
        {
            var modeBtns = h.AvailableModes
                .Take(4)
                .Select(m =>
                {
                    var label = m == h.Mode ? $"✓ {m}" : m;
                    return InlineKeyboardButton.WithCallbackData(label, Codec.SetMode(h.EntityId, m));
                });
            rows.AddRange(modeBtns.Chunk(3).Select(r => r.ToArray()));
        }

        rows.Add([InlineKeyboardButton.WithCallbackData("← Назад", Codec.Group("climate"))]);
        return new InlineKeyboardMarkup(rows);
    }

    private static string Esc(string s) => LightKeyboards.Esc(s);
}
