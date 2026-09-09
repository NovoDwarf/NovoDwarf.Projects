using SmartHome.HomeAssistant.Interfaces;
using SmartHome.HomeAssistant.Models;

namespace SmartHome.HomeAssistant.Clients.Interfaces;

public interface IMediaPlayerClient : IEntityClient
{
    Task<IReadOnlyList<MediaPlayerWithArea>> GetMediaPlayersWithAreasAsync(CancellationToken ct = default);
    Task MediaPlayAsync(string entityId, CancellationToken ct = default);
    Task MediaPauseAsync(string entityId, CancellationToken ct = default);
    Task SetMediaVolumeAsync(string entityId, double volume, CancellationToken ct = default);
    Task SetMediaMuteAsync(string entityId, bool muted, CancellationToken ct = default);
    Task SelectMediaSourceAsync(string entityId, string source, CancellationToken ct = default);
}
