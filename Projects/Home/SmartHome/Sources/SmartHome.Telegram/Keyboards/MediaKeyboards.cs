using SmartHome.Core;
using SmartHome.HomeAssistant;
using SmartHome.HomeAssistant.Extensions;
using SmartHome.HomeAssistant.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartHome.Telegram.Keyboards;

internal static class MediaKeyboards
{
    private static readonly IPayloadCodec Codec = TelegramPayloadCodec.Instance;

    // ── Group view ────────────────────────────────────────────────────────────

    public static string GroupText(IReadOnlyList<MediaPlayerWithArea> items)
    {
        var on = items.Count(m => m.IsActive);
        return $"📺 *Медиа*\n_{Esc($"{on} из {items.Count} активно")}_\n\nВыберите устройство:";
    }

    public static InlineKeyboardMarkup GroupKeyboard(IReadOnlyList<MediaPlayerWithArea> items)
    {
        var rows = new List<InlineKeyboardButton[]>();

        var byArea = items
            .GroupBy(m => m.AreaId)
            .OrderBy(g => g.First().AreaName);

        foreach (var group in byArea)
        {
            rows.Add([InlineKeyboardButton.WithCallbackData(
                $"📍 {group.First().AreaName}", Codec.Noop())]);

            var btns = group
                .OrderBy(m => m.FriendlyName ?? m.EntityId)
                .Select(m =>
                {
                    var icon = StateIcon(m.State);
                    return InlineKeyboardButton.WithCallbackData(
                        $"{icon} {m.FriendlyName ?? m.EntityId}",
                        Codec.Entity(m.EntityId));
                });

            rows.AddRange(btns.Chunk(2).Select(r => r.ToArray()));
        }

        rows.Add([InlineKeyboardButton.WithCallbackData("← Назад", Codec.Home())]);
        return new InlineKeyboardMarkup(rows);
    }

    // ── Entity view ───────────────────────────────────────────────────────────

    public static string EntityText(MediaPlayerWithArea m)
    {
        var icon = StateIcon(m.State);
        var status = StateLabel(m.State);

        var lines = new System.Text.StringBuilder();
        lines.Append($"📺 *{Esc(m.FriendlyName ?? m.EntityId)}*\n");
        lines.Append($"📍 {Esc(m.AreaName)}  •  {icon} {status}");

        if (m.IsActive)
        {
            if (m.Volume.HasValue)
            {
                var muteLabel = m.IsMuted ? "  🔇 заглушено" : string.Empty;
                lines.Append($"\n🔊 Громкость: *{m.VolumePercent}%*{muteLabel}");
            }
            if (m.Source is { Length: > 0 })
                lines.Append($"\n📡 Источник: *{Esc(m.Source)}*");
        }

        return lines.ToString();
    }

    public static InlineKeyboardMarkup EntityKeyboard(MediaPlayerWithArea m)
    {
        var rows = new List<InlineKeyboardButton[]>();

        if (!m.IsActive)
        {
            rows.Add([InlineKeyboardButton.WithCallbackData("▶️ Включить", Codec.TurnOn(m.EntityId))]);
        }
        else
        {
            var controlRow = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("⚫ Выключить", Codec.TurnOff(m.EntityId))
            };

            if (m.IsPlaying)
                controlRow.Add(InlineKeyboardButton.WithCallbackData("⏸ Пауза", Codec.Pause(m.EntityId)));
            else if (m.IsPaused)
                controlRow.Add(InlineKeyboardButton.WithCallbackData("▶️ Играть", Codec.Play(m.EntityId)));

            rows.Add([.. controlRow]);

            if (m.Volume.HasValue)
            {
                rows.Add([
                    InlineKeyboardButton.WithCallbackData("🔈 10%", Codec.Volume(m.EntityId, 10)),
                    InlineKeyboardButton.WithCallbackData("🔉 30%", Codec.Volume(m.EntityId, 30)),
                    InlineKeyboardButton.WithCallbackData("🔊 60%", Codec.Volume(m.EntityId, 60)),
                    InlineKeyboardButton.WithCallbackData("📢 90%", Codec.Volume(m.EntityId, 90)),
                ]);

                rows.Add(m.IsMuted
                    ? [InlineKeyboardButton.WithCallbackData("🔊 Включить звук", Codec.Mute(m.EntityId, false))]
                    : [InlineKeyboardButton.WithCallbackData("🔇 Заглушить",     Codec.Mute(m.EntityId, true))]);
            }

            if (m.Sources.Count > 0)
            {
                var srcBtns = m.Sources
                    .Take(4)
                    .Select((src, idx) =>
                    {
                        var label = src == m.Source ? $"✓ {src}" : src;
                        return InlineKeyboardButton.WithCallbackData(label, Codec.Source(m.EntityId, idx));
                    });

                rows.AddRange(srcBtns.Chunk(2).Select(r => r.ToArray()));
            }
        }

        rows.Add([InlineKeyboardButton.WithCallbackData("← Назад", Codec.Group("media"))]);
        return new InlineKeyboardMarkup(rows);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string StateIcon(string state) => state switch
    {
        "playing"   => "▶️",
        "paused"    => "⏸",
        "idle"      => "⏹",
        "buffering" => "⏳",
        "standby"   => "🌙",
        "on"        => "📺",
        _           => "○",
    };

    private static string StateLabel(string state) => state switch
    {
        "playing"   => "Воспроизводит",
        "paused"    => "Пауза",
        "idle"      => "Ожидание",
        "buffering" => "Буферизация",
        "standby"   => "Ожидание",
        "on"        => "Включён",
        _           => "Выключен",
    };

    private static string Esc(string s) => LightKeyboards.Esc(s);
}
