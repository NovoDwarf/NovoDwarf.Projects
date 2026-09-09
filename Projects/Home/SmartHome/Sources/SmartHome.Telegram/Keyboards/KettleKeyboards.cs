using SmartHome.Core;
using SmartHome.HomeAssistant.Extensions;
using SmartHome.HomeAssistant.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartHome.Telegram.Keyboards;

internal static class KettleKeyboards
{
    private static readonly IPayloadCodec Codec = TelegramPayloadCodec.Instance;

    public static string GroupText(IReadOnlyList<KettleWithArea> items)
    {
        var on = items.Count(k => k.IsOn);
        return $"☕ *Чайники*\n_{Esc($"{on} из {items.Count} включено")}_\n\nВыберите устройство:";
    }

    public static InlineKeyboardMarkup GroupKeyboard(IReadOnlyList<KettleWithArea> items)
    {
        var rows = new List<InlineKeyboardButton[]>();

        var byArea = items
            .GroupBy(k => k.AreaId)
            .OrderBy(g => g.First().AreaName);

        foreach (var group in byArea)
        {
            rows.Add([InlineKeyboardButton.WithCallbackData(
                $"📍 {group.First().AreaName}", Codec.Noop())]);

            var btns = group
                .OrderBy(k => k.FriendlyName ?? k.EntityId)
                .Select(k =>
                {
                    var icon = k.IsOn ? "☕" : "○";
                    return InlineKeyboardButton.WithCallbackData(
                        $"{icon} {k.FriendlyName ?? k.EntityId}",
                        Codec.Entity(k.EntityId));
                });

            rows.AddRange(btns.Chunk(2).Select(r => r.ToArray()));
        }

        rows.Add([InlineKeyboardButton.WithCallbackData("← Назад", Codec.Home())]);
        return new InlineKeyboardMarkup(rows);
    }

    public static string EntityText(KettleWithArea k)
    {
        var status = k.IsOn ? "✅ Включён" : "⚫ Выключен";

        var sb = new System.Text.StringBuilder();
        sb.Append($"☕ *{Esc(k.FriendlyName ?? k.EntityId)}*\n");
        sb.Append($"📍 {Esc(k.AreaName)}  •  {status}");

        if (k.CurrentTemperature.HasValue)
            sb.Append($"\n🌡 Температура: *{k.CurrentTemperature:F0}°C*");

        if (k.IsOn && k.TargetTemperature.HasValue)
            sb.Append($"  →  цель *{k.TargetTemperature:F0}°C*");

        return sb.ToString();
    }

    public static InlineKeyboardMarkup EntityKeyboard(KettleWithArea k)
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(k.IsOn
            ? [InlineKeyboardButton.WithCallbackData("⚫ Выключить", Codec.TurnOff(k.EntityId))]
            : [InlineKeyboardButton.WithCallbackData("☕ Вскипятить", Codec.TurnOn(k.EntityId))]);

        if (k.IsOn)
        {
            rows.Add([
                InlineKeyboardButton.WithCallbackData("60°C", Codec.KettleTemp(k.EntityId, 60)),
                InlineKeyboardButton.WithCallbackData("70°C", Codec.KettleTemp(k.EntityId, 70)),
                InlineKeyboardButton.WithCallbackData("80°C", Codec.KettleTemp(k.EntityId, 80)),
                InlineKeyboardButton.WithCallbackData("90°C", Codec.KettleTemp(k.EntityId, 90)),
            ]);
            rows.Add([
                InlineKeyboardButton.WithCallbackData("100°C 🌊", Codec.KettleTemp(k.EntityId, 100)),
            ]);
        }

        rows.Add([InlineKeyboardButton.WithCallbackData("← Назад", Codec.Group("kettle"))]);
        return new InlineKeyboardMarkup(rows);
    }

    private static string Esc(string s) => LightKeyboards.Esc(s);
}
