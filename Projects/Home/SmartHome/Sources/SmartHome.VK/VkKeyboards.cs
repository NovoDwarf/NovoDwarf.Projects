using SmartHome.Core;
using SmartHome.HomeAssistant.Extensions;
using SmartHome.HomeAssistant.Models;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace SmartHome.VK;

/// <summary>
/// Builds plain-text message bodies and VK inline keyboards for the /home navigation.
/// VK inline keyboards are attached to the message and support callback buttons with colors.
/// </summary>
internal static class VkKeyboards
{
    private static readonly IPayloadCodec Codec = VkPayloadCodec.Instance;

    // ── Home ──────────────────────────────────────────────────────────────────

    public static (string Text, MessageKeyboard Kb) Home() => (
        "🏠 SmartHome\nВыберите раздел:",
        Inline([
            [
                Btn("💡 Свет",    Codec.Group("light"),   KeyboardButtonColor.Primary),
                Btn("💧 Климат",  Codec.Group("climate"), KeyboardButtonColor.Primary),
            ],
            [
                Btn("📺 Медиа",   Codec.Group("media"),   KeyboardButtonColor.Primary),
                Btn("☕ Чайники", Codec.Group("kettle"),  KeyboardButtonColor.Primary),
            ],
            [
                Btn("🤖 Сценарии", Codec.Group("scene"), KeyboardButtonColor.Primary),
            ],
        ])
    );

    // ── Lights ────────────────────────────────────────────────────────────────

    public static (string Text, MessageKeyboard Kb) LightGroup(IReadOnlyList<LightWithArea> lights)
    {
        var on = lights.Count(l => l.State == "on");
        var text = $"💡 Освещение\n{on} из {lights.Count} включено\n\nВыберите устройство:";

        var btns = lights
            .OrderBy(l => l.AreaName).ThenBy(l => l.FriendlyName ?? l.EntityId)
            .Select(l =>
            {
                var icon = l.State == "on" ? "💡" : "○";
                return Btn(Label($"{icon} {l.FriendlyName ?? l.EntityId}", l.AreaName),
                           Codec.Entity(l.EntityId));
            });

        return (text, DeviceKeyboard(btns, Codec.Home()));
    }

    public static (string Text, MessageKeyboard Kb) LightEntity(LightWithArea l)
    {
        var isOn = l.State == "on";
        var status = isOn ? "✅ Включена" : "⚫ Выключена";
        var pct = l.Brightness.HasValue
            ? (int)Math.Round(l.Brightness.Value / 255.0 * 100) : 0;

        var text = $"💡 {l.FriendlyName ?? l.EntityId}\n" +
                   $"📍 {l.AreaName}  •  {status}" +
                   (isOn && l.Brightness.HasValue ? $"\n🔆 Яркость: {pct}%" : "");

        var rows = new List<List<MessageKeyboardButton>>();

        rows.Add([isOn
            ? Btn("⚫ Выключить", Codec.TurnOff(l.EntityId), KeyboardButtonColor.Negative)
            : Btn("💡 Включить",  Codec.TurnOn(l.EntityId),  KeyboardButtonColor.Positive)]);

        if (isOn && l.Brightness.HasValue)
            rows.Add([
                Btn("🔅 25%",  Codec.Brightness(l.EntityId, 64)),
                Btn("🔆 50%",  Codec.Brightness(l.EntityId, 128)),
                Btn("☀️ 75%",  Codec.Brightness(l.EntityId, 191)),
                Btn("☀️ 100%", Codec.Brightness(l.EntityId, 255)),
            ]);

        rows.Add([Btn("← Назад", Codec.Group("light"))]);
        return (text, Inline(rows));
    }

    // ── Humidifiers ───────────────────────────────────────────────────────────

    public static (string Text, MessageKeyboard Kb) HumidifierGroup(IReadOnlyList<HumidifierWithArea> items)
    {
        var on = items.Count(h => h.State == "on");
        var text = $"💧 Климат\n{on} из {items.Count} включено\n\nВыберите устройство:";

        var btns = items
            .OrderBy(h => h.AreaName).ThenBy(h => h.FriendlyName ?? h.EntityId)
            .Select(h =>
            {
                var icon = h.State == "on" ? "💧" : "○";
                return Btn(Label($"{icon} {h.FriendlyName ?? h.EntityId}", h.AreaName),
                           Codec.Entity(h.EntityId));
            });

        return (text, DeviceKeyboard(btns, Codec.Home()));
    }

    public static (string Text, MessageKeyboard Kb) HumidifierEntity(HumidifierWithArea h)
    {
        var isOn = h.State == "on";
        var status = isOn ? "✅ Включён" : "⚫ Выключен";

        var sb = new System.Text.StringBuilder();
        sb.Append($"💧 {h.FriendlyName ?? h.EntityId}\n");
        sb.Append($"📍 {h.AreaName}  •  {status}");
        if (h.CurrentHumidity.HasValue) sb.Append($"\n💦 Влажность: {h.CurrentHumidity}%");
        if (isOn && h.TargetHumidity.HasValue) sb.Append($"  →  цель {h.TargetHumidity}%");
        if (isOn && h.Mode is { Length: > 0 }) sb.Append($"\n🔄 Режим: {h.Mode}");

        var rows = new List<List<MessageKeyboardButton>>();

        rows.Add([isOn
            ? Btn("⚫ Выключить", Codec.TurnOff(h.EntityId), KeyboardButtonColor.Negative)
            : Btn("💧 Включить",  Codec.TurnOn(h.EntityId),  KeyboardButtonColor.Positive)]);

        if (isOn)
            rows.Add([
                Btn("40%", Codec.SetHumidity(h.EntityId, 40)),
                Btn("50%", Codec.SetHumidity(h.EntityId, 50)),
                Btn("60%", Codec.SetHumidity(h.EntityId, 60)),
                Btn("70%", Codec.SetHumidity(h.EntityId, 70)),
            ]);

        if (isOn && h.AvailableModes.Count > 0)
        {
            rows.AddRange(h.AvailableModes.Take(4)
                .Select(m => Btn(m == h.Mode ? $"✓ {m}" : m, Codec.SetMode(h.EntityId, m)))
                .Chunk(3)
                .Select(r => r.ToList()));
        }

        rows.Add([Btn("← Назад", Codec.Group("climate"))]);
        return (sb.ToString(), Inline(rows));
    }

    // ── Media players ─────────────────────────────────────────────────────────

    public static (string Text, MessageKeyboard Kb) MediaGroup(IReadOnlyList<MediaPlayerWithArea> items)
    {
        var on = items.Count(m => m.IsActive);
        var text = $"📺 Медиа\n{on} из {items.Count} активно\n\nВыберите устройство:";

        var btns = items
            .OrderBy(m => m.AreaName).ThenBy(m => m.FriendlyName ?? m.EntityId)
            .Select(m =>
            {
                var icon = StateIcon(m.State);
                return Btn(Label($"{icon} {m.FriendlyName ?? m.EntityId}", m.AreaName),
                           Codec.Entity(m.EntityId));
            });

        return (text, DeviceKeyboard(btns, Codec.Home()));
    }

    public static (string Text, MessageKeyboard Kb) MediaEntity(MediaPlayerWithArea m)
    {
        var icon = StateIcon(m.State);
        var status = StateLabel(m.State);

        var sb = new System.Text.StringBuilder();
        sb.Append($"📺 {m.FriendlyName ?? m.EntityId}\n");
        sb.Append($"📍 {m.AreaName}  •  {icon} {status}");
        if (m.IsActive && m.Volume.HasValue)
            sb.Append($"\n🔊 Громкость: {m.VolumePercent}%{(m.IsMuted ? "  🔇 заглушено" : "")}");
        if (m.IsActive && m.Source is { Length: > 0 })
            sb.Append($"\n📡 {m.Source}");

        var rows = new List<List<MessageKeyboardButton>>();

        if (!m.IsActive)
        {
            rows.Add([Btn("▶️ Включить", Codec.TurnOn(m.EntityId), KeyboardButtonColor.Positive)]);
        }
        else
        {
            var controlRow = new List<MessageKeyboardButton>
            {
                Btn("⚫ Выключить", Codec.TurnOff(m.EntityId), KeyboardButtonColor.Negative)
            };
            if (m.IsPlaying) controlRow.Add(Btn("⏸ Пауза", Codec.Pause(m.EntityId)));
            else if (m.IsPaused) controlRow.Add(Btn("▶️ Играть", Codec.Play(m.EntityId), KeyboardButtonColor.Positive));
            rows.Add(controlRow);

            if (m.Volume.HasValue)
            {
                rows.Add([
                    Btn("🔈 10%", Codec.Volume(m.EntityId, 10)),
                    Btn("🔉 30%", Codec.Volume(m.EntityId, 30)),
                    Btn("🔊 60%", Codec.Volume(m.EntityId, 60)),
                    Btn("📢 90%", Codec.Volume(m.EntityId, 90)),
                ]);
                rows.Add([m.IsMuted
                    ? Btn("🔊 Включить звук", Codec.Mute(m.EntityId, false), KeyboardButtonColor.Primary)
                    : Btn("🔇 Заглушить",     Codec.Mute(m.EntityId, true))]);
            }

            if (m.Sources.Count > 0)
            {
                rows.AddRange(m.Sources.Take(4)
                    .Select((src, idx) => Btn(src == m.Source ? $"✓ {src}" : src,
                        Codec.Source(m.EntityId, idx),
                        src == m.Source ? KeyboardButtonColor.Primary : KeyboardButtonColor.Default))
                    .Chunk(2)
                    .Select(r => r.ToList()));
            }
        }

        rows.Add([Btn("← Назад", Codec.Group("media"))]);
        return (sb.ToString(), Inline(rows));
    }

    // ── Scenarios (automations + scripts) ────────────────────────────────────

    public static (string Text, MessageKeyboard Kb) ScenarioGroup(
        IReadOnlyList<AutomationInfo> automations, IReadOnlyList<ScriptInfo> scripts)
    {
        var enabled = automations.Count(a => a.IsEnabled);
        var text = $"🤖 Сценарии\n{enabled} из {automations.Count} авто включено  •  {scripts.Count} скриптов\n\nВыберите:";

        var rows = new List<List<MessageKeyboardButton>>();

        if (automations.Count > 0)
        {
            rows.Add([Btn("── Автоматизации ──", Codec.Noop())]);
            var chunks = automations
                .Select(a => Btn($"{(a.IsEnabled ? "✓" : "✗")} {Truncate(a.FriendlyName ?? a.EntityId, 18)}",
                                 Codec.Entity(a.EntityId)))
                .Chunk(2).Select(r => r.ToList());
            rows.AddRange(chunks);
        }

        if (scripts.Count > 0)
        {
            rows.Add([Btn("── Скрипты ──", Codec.Noop())]);
            var chunks = scripts
                .Select(s => Btn($"{(s.IsRunning ? "⚡" : "▶")} {Truncate(s.FriendlyName ?? s.EntityId, 18)}",
                                 Codec.Entity(s.EntityId)))
                .Chunk(2).Select(r => r.ToList());
            rows.AddRange(chunks);
        }

        rows.Add([Btn("← Назад", Codec.Home())]);
        return (text, Inline(rows));
    }

    public static (string Text, MessageKeyboard Kb) AutomationEntity(AutomationInfo a)
    {
        var status = a.IsEnabled ? "✅ Включена" : "⚫ Выключена";
        var sb = new System.Text.StringBuilder();
        sb.Append($"🤖 {a.FriendlyName ?? a.EntityId}\nСтатус: {status}");
        if (a.LastTriggered.HasValue)
        {
            var local = a.LastTriggered.Value.ToLocalTime();
            var label = local.Date == DateTime.Today ? $"сегодня в {local:HH:mm}"
                : local.Date == DateTime.Today.AddDays(-1) ? $"вчера в {local:HH:mm}"
                : local.ToString("dd.MM.yyyy HH:mm");
            sb.Append($"\n🕐 Последний запуск: {label}");
        }

        var rows = new List<List<MessageKeyboardButton>>();
        rows.Add([Btn("▶️ Запустить сейчас", Codec.TriggerAutomation(a.EntityId), KeyboardButtonColor.Positive)]);
        rows.Add([a.IsEnabled
            ? Btn("⚫ Выключить", Codec.TurnOff(a.EntityId), KeyboardButtonColor.Negative)
            : Btn("✅ Включить",  Codec.TurnOn(a.EntityId),  KeyboardButtonColor.Positive)]);
        rows.Add([Btn("← Назад", Codec.Group("scene"))]);
        return (sb.ToString(), Inline(rows));
    }

    public static (string Text, MessageKeyboard Kb) ScriptEntity(ScriptInfo s)
    {
        var status = s.IsRunning ? "⚡ Выполняется" : "⚫ Ожидает";
        var text = $"📜 {s.FriendlyName ?? s.EntityId}\nСостояние: {status}";
        var rows = new List<List<MessageKeyboardButton>>();
        rows.Add([Btn("▶️ Запустить", Codec.TurnOn(s.EntityId), KeyboardButtonColor.Positive)]);
        rows.Add([Btn("← Назад", Codec.Group("scene"))]);
        return (text, Inline(rows));
    }

    // ── Kettles ───────────────────────────────────────────────────────────────

    public static (string Text, MessageKeyboard Kb) KettleGroup(IReadOnlyList<KettleWithArea> items)
    {
        var on = items.Count(k => k.IsOn);
        var text = $"☕ Чайники\n{on} из {items.Count} включено\n\nВыберите устройство:";

        var btns = items
            .OrderBy(k => k.AreaName).ThenBy(k => k.FriendlyName ?? k.EntityId)
            .Select(k =>
            {
                var icon = k.IsOn ? "☕" : "○";
                return Btn(Label($"{icon} {k.FriendlyName ?? k.EntityId}", k.AreaName),
                           Codec.Entity(k.EntityId));
            });

        return (text, DeviceKeyboard(btns, Codec.Home()));
    }

    public static (string Text, MessageKeyboard Kb) KettleEntity(KettleWithArea k)
    {
        var status = k.IsOn ? "✅ Включён" : "⚫ Выключен";

        var sb = new System.Text.StringBuilder();
        sb.Append($"☕ {k.FriendlyName ?? k.EntityId}\n");
        sb.Append($"📍 {k.AreaName}  •  {status}");
        if (k.CurrentTemperature.HasValue)
            sb.Append($"\n🌡 Температура: {k.CurrentTemperature:F0}°C");
        if (k.IsOn && k.TargetTemperature.HasValue)
            sb.Append($"  →  цель {k.TargetTemperature:F0}°C");

        var rows = new List<List<MessageKeyboardButton>>();

        rows.Add([k.IsOn
            ? Btn("⚫ Выключить", Codec.TurnOff(k.EntityId), KeyboardButtonColor.Negative)
            : Btn("☕ Вскипятить", Codec.TurnOn(k.EntityId), KeyboardButtonColor.Positive)]);

        if (k.IsOn)
        {
            rows.Add([
                Btn("60°C", Codec.KettleTemp(k.EntityId, 60)),
                Btn("70°C", Codec.KettleTemp(k.EntityId, 70)),
                Btn("80°C", Codec.KettleTemp(k.EntityId, 80)),
                Btn("90°C", Codec.KettleTemp(k.EntityId, 90)),
            ]);
            rows.Add([Btn("100°C 🌊", Codec.KettleTemp(k.EntityId, 100), KeyboardButtonColor.Primary)]);
        }

        rows.Add([Btn("← Назад", Codec.Group("kettle"))]);
        return (sb.ToString(), Inline(rows));
    }

    // ── Button / keyboard builders ────────────────────────────────────────────

    private static MessageKeyboard DeviceKeyboard(
        IEnumerable<MessageKeyboardButton> deviceBtns,
        string backPayload)
    {
        const int MaxDeviceRows = 9;

        var all = deviceBtns.ToList();
        var shown = all.Take(MaxDeviceRows * 2).ToList();
        var hidden = all.Count - shown.Count;

        var rows = shown
            .Chunk(2)
            .Select(r => r.ToList())
            .ToList<List<MessageKeyboardButton>>();

        if (hidden > 0)
            rows.Add([Btn($"… и ещё {hidden}", Codec.Noop())]);

        rows.Add([Btn("← Назад", backPayload)]);
        return Inline(rows);
    }

    private static string Truncate(string s, int max)
        => s.Length <= max ? s : s[..(max - 1)] + "…";

    private static string Label(string devicePart, string roomName)
    {
        var full = $"{devicePart} · {roomName}";
        return full.Length <= 40 ? full : full[..37] + "…";
    }

    private static MessageKeyboardButton Btn(
        string label,
        string payload,
        KeyboardButtonColor color = KeyboardButtonColor.Default)
        => new()
        {
            Action = new MessageKeyboardButtonAction
            {
                Type = KeyboardButtonActionType.Callback,
                Label = label.Length <= 40 ? label : label[..37] + "…",
                Payload = payload,
            },
            Color = color,
        };

    private static MessageKeyboard Inline(IEnumerable<List<MessageKeyboardButton>> rows)
        => new() { Inline = true, Buttons = rows.ToList() };

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
}
