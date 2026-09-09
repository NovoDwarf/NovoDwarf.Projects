using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.Core;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.ViewModels;

public sealed partial class DashboardViewModel : ObservableObject
{
    private readonly IHealthDataProvider _provider;
    private readonly HealthApiClient _api;
    private readonly AppSettings _settings;

    public DashboardViewModel(IHealthDataProvider provider, HealthApiClient api, AppSettings settings)
    {
        _provider = provider;
        _api = api;
        _settings = settings;
    }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int? Steps { get; set; }

    [ObservableProperty]
    public partial double? HeartRate { get; set; }

    [ObservableProperty]
    public partial double? RestingHeartRate { get; set; }

    [ObservableProperty]
    public partial double? SleepHours { get; set; }

    [ObservableProperty]
    public partial double? ActiveCalories { get; set; }

    [ObservableProperty]
    public partial double? Weight { get; set; }

    [ObservableProperty]
    public partial double? SpO2 { get; set; }

    [ObservableProperty]
    public partial DateTimeOffset? LastSync { get; set; }

    public bool HasData => Steps.HasValue || HeartRate.HasValue || SleepHours.HasValue;

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            if (!_provider.IsAvailable)
            {
                StatusMessage = "Данные о здоровье недоступны на этом устройстве.";
                return;
            }

            var granted = await _provider.RequestPermissionsAsync();
            if (!granted)
            {
                StatusMessage = "Нет разрешения на чтение данных о здоровье.";
                return;
            }

            var metrics = await _provider.ReadTodayAsync();
            Apply(metrics);
            StatusMessage = "Данные обновлены.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(HasData));
        }
    }

    [RelayCommand]
    private async Task SyncAsync()
    {
        if (IsBusy) return;
        if (!HasData)
        {
            await RefreshAsync();
            if (!HasData) return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            if (!_settings.IsConfigured)
            {
                StatusMessage = "Сервер не настроен. Перейдите в Настройки.";
                return;
            }

            var metrics = BuildMetrics();
            await _api.SendAsync(metrics);

            LastSync = _settings.LastSync = DateTimeOffset.Now;
            StatusMessage = $"Синхронизировано в {LastSync:HH:mm}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка отправки: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Apply(HealthMetrics m)
    {
        Steps            = m.Steps;
        HeartRate        = m.HeartRate;
        RestingHeartRate = m.RestingHeartRate;
        SleepHours       = m.SleepHours;
        ActiveCalories   = m.ActiveCalories;
        Weight           = m.Weight;
        SpO2             = m.SpO2;
    }

    private HealthMetrics BuildMetrics() => new()
    {
        Steps            = Steps,
        HeartRate        = HeartRate,
        RestingHeartRate = RestingHeartRate,
        SleepHours       = SleepHours,
        ActiveCalories   = ActiveCalories,
        Weight           = Weight,
        SpO2             = SpO2,
    };
}
