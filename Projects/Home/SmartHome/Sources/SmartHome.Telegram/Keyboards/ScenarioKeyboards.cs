using SmartHome.Core;
using SmartHome.HomeAssistant.Extensions;
using SmartHome.HomeAssistant.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartHome.Telegram.Keyboards;

internal static class ScenarioKeyboards
{
    private static readonly IPayloadCodec Codec = TelegramPayloadCodec.Instance;

    // ── Group screen (automations + scripts combined) ─────────────────────────

    public static string GroupText(IReadOnlyList<AutomationInfo> automations, IReadOnlyList<ScriptInfo> scripts)
    {
        var enabled = automations.Count(a => a.IsEnabled);
        return $"🤖 *Сценарии*\n" +
               $"_{Esc($"{enabled} из {automations.Count} авто включено  •  {scripts.Count} скриптов")}_\n\n" +
               $"Выберите:";
    }

    public static InlineKeyboardMarkup GroupKeyboard(
        IReadOnlyList<AutomationInfo> automations, IReadOnlyList<ScriptInfo> scripts)
    {
        var rows = new List<InlineKeyboardButton[]>();

        if (automations.Count > 0)
        {
            rows.Add([InlineKeyboardButton.WithCallbackData("── Автоматизации ──", Codec.Noop())]);
            var btns = automations.Select(a =>
            {
                var icon = a.IsEnabled ? "✓" : "✗";
                return InlineKeyboardButton.WithCallbackData(
                    $"{icon} {a.FriendlyName ?? a.EntityId}",
                    Codec.Entity(a.EntityId));
            });
            rows.AddRange(btns.Chunk(2).Select(r => r.ToArray()));
        }

        if (scripts.Count > 0)
        {
            rows.Add([InlineKeyboardButton.WithCallbackData("── Скрипты ──", Codec.Noop())]);
            var btns = scripts.Select(s =>
            {
                var icon = s.IsRunning ? "⚡" : "▶";
                return InlineKeyboardButton.WithCallbackData(
                    $"{icon} {s.FriendlyName ?? s.EntityId}",
                    Codec.Entity(s.EntityId));
            });
            rows.AddRange(btns.Chunk(2).Select(r => r.ToArray()));
        }

        rows.Add([InlineKeyboardButton.WithCallbackData("← Назад", Codec.Home())]);
        return new InlineKeyboardMarkup(rows);
    }

    // ── Automation entity screen ──────────────────────────────────────────────

    public static string AutomationText(AutomationInfo a)
    {
        var status = a.IsEnabled ? "✅ Включена" : "⚫ Выключена";
        var sb = new System.Text.StringBuilder();
        sb.Append($"🤖 *{Esc(a.FriendlyName ?? a.EntityId)}*\n");
        sb.Append($"Статус: {status}");
        if (a.LastTriggered.HasValue)
            sb.Append($"\n🕐 Последний запуск: *{Esc(FormatTime(a.LastTriggered.Value))}*");
        return sb.ToString();
    }

    public static InlineKeyboardMarkup AutomationKeyboard(AutomationInfo a)
    {
        var rows = new List<InlineKeyboardButton[]>();
        rows.Add([InlineKeyboardButton.WithCallbackData("▶️ Запустить сейчас", Codec.TriggerAutomation(a.EntityId))]);
        rows.Add(a.IsEnabled
            ? [InlineKeyboardButton.WithCallbackData("⚫ Выключить", Codec.TurnOff(a.EntityId))]
            : [InlineKeyboardButton.WithCallbackData("✅ Включить",  Codec.TurnOn(a.EntityId))]);
        rows.Add([InlineKeyboardButton.WithCallbackData("← Назад", Codec.Group("scene"))]);
        return new InlineKeyboardMarkup(rows);
    }

    // ── Script entity screen ──────────────────────────────────────────────────

    public static string ScriptText(ScriptInfo s)
    {
        var status = s.IsRunning ? "⚡ Выполняется" : "⚫ Ожидает";
        return $"📜 *{Esc(s.FriendlyName ?? s.EntityId)}*\nСостояние: {status}";
    }

    public static InlineKeyboardMarkup ScriptKeyboard(ScriptInfo s)
        => new InlineKeyboardMarkup([
            [InlineKeyboardButton.WithCallbackData("▶️ Запустить", Codec.TurnOn(s.EntityId))],
            [InlineKeyboardButton.WithCallbackData("← Назад", Codec.Group("scene"))],
        ]);

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string FormatTime(DateTimeOffset dt)
    {
        var local = dt.ToLocalTime();
        var now = DateTimeOffset.Now;
        if (local.Date == now.Date) return $"сегодня в {local:HH:mm}";
        if (local.Date == now.Date.AddDays(-1)) return $"вчера в {local:HH:mm}";
        return local.ToString("dd.MM.yyyy HH:mm");
    }

    private static string Esc(string s) => LightKeyboards.Esc(s);
}
