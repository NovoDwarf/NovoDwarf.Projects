using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.Core;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly AppSettings _settings;
    private readonly HealthApiClient _api;

    public SettingsViewModel(AppSettings settings, HealthApiClient api)
    {
        _settings = settings;
        _api = api;
        Load();
    }

    [ObservableProperty]
    public partial string ServerUrl { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ApiKey { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = string.Empty;

    [RelayCommand]
    private void Save()
    {
        _settings.ServerUrl = ServerUrl.Trim();
        _settings.ApiKey = ApiKey.Trim();
        StatusMessage = "Сохранено.";
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        if (IsBusy) return;
        Save();

        if (!_settings.IsConfigured)
        {
            StatusMessage = "Укажите URL сервера и API-ключ.";
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            await _api.SendAsync(new HealthMetrics());
            StatusMessage = "✅ Подключение успешно.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Ошибка: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Load()
    {
        ServerUrl = _settings.ServerUrl;
        ApiKey    = _settings.ApiKey;
    }
}
