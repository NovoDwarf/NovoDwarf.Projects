using SmartHome.HomeAssistant;
using SmartHome.HomeAssistant.Interfaces;
using SmartHome.Telegram.Keyboards;

namespace SmartHome.Telegram.Handlers;

/// <summary>
/// Telegram-side domain handler: extends <see cref="IDomainExecutor"/> with
/// bot-specific screen building (text + inline keyboard markup).
/// </summary>
public interface IDomainHandler : IDomainExecutor
{
    /// <summary>Fetches live state and returns the group list screen.</summary>
    Task<Screen> GetGroupScreenAsync(CancellationToken ct);

    /// <summary>
    /// Fetches live state and returns the entity detail screen,
    /// or <c>null</c> if the entity no longer exists (caller falls back to home).
    /// </summary>
    Task<Screen?> GetEntityScreenAsync(string entityId, CancellationToken ct);
}
