using SmartHome.Core;
using SmartHome.HomeAssistant;
using SmartHome.HomeAssistant.Extensions;
using SmartHome.HomeAssistant.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartHome.Telegram.Keyboards;

internal static class LightKeyboards
{
    private static readonly IPayloadCodec Codec = TelegramPayloadCodec.Instance;

    // ── Group view — flat list with room separators ────────────────────────────

    public static string GroupText(IReadOnlyList<LightWithArea> lights)
    {
        var on = lights.Count(l => l.State == "on");
        return $"💡 *Освещение*\n_{Esc($"{on} из {lights.Count} включено")}_\n\nВыберите устройство:";
    }

    public static InlineKeyboardMarkup GroupKeyboard(IReadOnlyList<LightWithArea> lights)
    {
        var rows = new List<InlineKeyboardButton[]>();

        var byArea = lights
            .GroupBy(l => l.AreaId)
            .OrderBy(g => g.First().AreaName);

        foreach (var group in byArea)
        {
            rows.Add([InlineKeyboardButton.WithCallbackData(
                $"📍 {group.First().AreaName}", Codec.Noop())]);

            var deviceBtns = group
                .OrderBy(l => l.FriendlyName ?? l.EntityId)
                .Select(l =>
                {
                    var icon = l.State == "on" ? "💡" : "○";
                    return InlineKeyboardButton.WithCallbackData(
                        $"{icon} {l.FriendlyName ?? l.EntityId}",
                        Codec.Entity(l.EntityId));
                });

            rows.AddRange(deviceBtns.Chunk(2).Select(r => r.ToArray()));
        }

        rows.Add([InlineKeyboardButton.WithCallbackData("← Назад", Codec.Home())]);
        return new InlineKeyboardMarkup(rows);
    }

    // ── Entity view ───────────────────────────────────────────────────────────

    public static string EntityText(LightWithArea l)
    {
        var isOn = l.State == "on";
        var status = isOn ? "✅ Включена" : "⚫ Выключена";
        var briLine = isOn && l.Brightness.HasValue
            ? $"\n🔆 Яркость: *{(int)Math.Round(l.Brightness.Value / 255.0 * 100)}%*"
            : string.Empty;

        return $"💡 *{Esc(l.FriendlyName ?? l.EntityId)}*\n" +
               $"📍 {Esc(l.AreaName)}  •  {status}" +
               briLine;
    }

    public static InlineKeyboardMarkup EntityKeyboard(LightWithArea l)
    {
        var isOn = l.State == "on";
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(isOn
            ? [InlineKeyboardButton.WithCallbackData("⚫ Выключить", Codec.TurnOff(l.EntityId))]
            : [InlineKeyboardButton.WithCallbackData("💡 Включить", Codec.TurnOn(l.EntityId))]);

        if (isOn && l.Brightness.HasValue)
        {
            rows.Add([
                InlineKeyboardButton.WithCallbackData("🔅 25%",  Codec.Brightness(l.EntityId, 64)),
                InlineKeyboardButton.WithCallbackData("🔆 50%",  Codec.Brightness(l.EntityId, 128)),
                InlineKeyboardButton.WithCallbackData("☀️ 75%",  Codec.Brightness(l.EntityId, 191)),
                InlineKeyboardButton.WithCallbackData("☀️ 100%", Codec.Brightness(l.EntityId, 255)),
            ]);
        }

        rows.Add([InlineKeyboardButton.WithCallbackData("← Назад", Codec.Group("light"))]);
        return new InlineKeyboardMarkup(rows);
    }

    // ── MarkdownV2 escaping ───────────────────────────────────────────────────

    public static string Esc(string s) => s
        .Replace("\\", "\\\\")
        .Replace("_",  "\\_") .Replace("*",  "\\*") .Replace("[",  "\\[")
        .Replace("]",  "\\]") .Replace("(",  "\\(") .Replace(")",  "\\)")
        .Replace("~",  "\\~") .Replace("`",  "\\`") .Replace(">",  "\\>")
        .Replace("#",  "\\#") .Replace("+",  "\\+") .Replace("-",  "\\-")
        .Replace("=",  "\\=") .Replace("|",  "\\|") .Replace("{",  "\\{")
        .Replace("}",  "\\}") .Replace(".",  "\\.") .Replace("!",  "\\!");
}
