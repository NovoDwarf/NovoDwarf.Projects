namespace SmartHome.Maui.Services;

/// <summary>
/// Persists user configuration via MAUI Preferences (Keychain on iOS, SharedPreferences on Android).
/// </summary>
public sealed class AppSettings
{
    private const string KeyServerUrl = "server_url";
    private const string KeyApiKey = "api_key";
    private const string KeyAutoSync = "auto_sync";
    private const string KeyLastSync = "last_sync";

    public string ServerUrl
    {
        get => Preferences.Default.Get(KeyServerUrl, string.Empty);
        set => Preferences.Default.Set(KeyServerUrl, value.TrimEnd('/'));
    }

    public string ApiKey
    {
        get => Preferences.Default.Get(KeyApiKey, string.Empty);
        set => Preferences.Default.Set(KeyApiKey, value);
    }

    public bool AutoSync
    {
        get => Preferences.Default.Get(KeyAutoSync, false);
        set => Preferences.Default.Set(KeyAutoSync, value);
    }

    public DateTimeOffset? LastSync
    {
        get
        {
            var raw = Preferences.Default.Get(KeyLastSync, string.Empty);
            return DateTimeOffset.TryParse(raw, out var dt) ? dt : null;
        }
        set => Preferences.Default.Set(KeyLastSync, value?.ToString("O") ?? string.Empty);
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ServerUrl) && !string.IsNullOrWhiteSpace(ApiKey);
}
