using System.Net.Http.Json;
using SmartHome.Core;
using SmartHome.Maui;

namespace SmartHome.Maui.Services;

/// <summary>
/// Sends a <see cref="HealthMetrics"/> snapshot to the SmartHome backend.
/// </summary>
public sealed class HealthApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly AppSettings _settings;

    public HealthApiClient(IHttpClientFactory factory, AppSettings settings)
    {
        _factory = factory;
        _settings = settings;
    }

    /// <summary>
    /// Posts the metrics to POST {ServerUrl}/api/health.
    /// Returns true on HTTP 204, throws <see cref="InvalidOperationException"/> if not configured.
    /// </summary>
    public async Task SendAsync(HealthMetrics metrics, CancellationToken ct = default)
    {
        if (!_settings.IsConfigured)
            throw new InvalidOperationException("Сервер не настроен. Укажите URL и API-ключ в настройках.");

        var client = _factory.CreateClient("smarthome");
        client.DefaultRequestHeaders.Remove("X-Api-Key");
        client.DefaultRequestHeaders.Add("X-Api-Key", _settings.ApiKey);

        var response = await client.PostAsJsonAsync(
            $"{_settings.ServerUrl}/api/health", metrics, ct);

        response.EnsureSuccessStatusCode();
    }
}
