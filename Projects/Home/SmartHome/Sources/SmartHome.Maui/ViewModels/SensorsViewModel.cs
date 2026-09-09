using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.Maui.Models;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.ViewModels;

public sealed partial class SensorsViewModel : ObservableObject
{
    private readonly SmartHomeApiClient _api;

    [ObservableProperty] private ObservableCollection<SensorState>       _sensors       = [];
    [ObservableProperty] private ObservableCollection<BinarySensorState> _binarySensors = [];
    [ObservableProperty] private bool   _isRefreshing;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string _filter        = string.Empty;

    public SensorsViewModel(SmartHomeApiClient api) => _api = api;

    partial void OnFilterChanged(string value) => ApplyFilter();

    [RelayCommand]
    async Task RefreshAsync()
    {
        IsRefreshing = true;
        StatusMessage = string.Empty;
        try
        {
            var sensorsTask       = _api.GetSensorsAsync();
            var binarySensorsTask = _api.GetBinarySensorsAsync();
            await Task.WhenAll(sensorsTask, binarySensorsTask);

            _allSensors       = sensorsTask.Result;
            _allBinarySensors = binarySensorsTask.Result;

            ApplyFilter();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    // ── Filter ────────────────────────────────────────────────────────────────

    private IReadOnlyList<SensorState>       _allSensors       = [];
    private IReadOnlyList<BinarySensorState> _allBinarySensors = [];

    private void ApplyFilter()
    {
        var f = Filter.Trim();

        if (string.IsNullOrEmpty(f))
        {
            Sensors       = new ObservableCollection<SensorState>(_allSensors);
            BinarySensors = new ObservableCollection<BinarySensorState>(_allBinarySensors);
            return;
        }

        Sensors = new ObservableCollection<SensorState>(
            _allSensors.Where(s =>
                s.DisplayName.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                s.AreaName.Contains(f,   StringComparison.OrdinalIgnoreCase)  ||
                (s.DeviceClass?.Contains(f, StringComparison.OrdinalIgnoreCase) ?? false)));

        BinarySensors = new ObservableCollection<BinarySensorState>(
            _allBinarySensors.Where(s =>
                s.DisplayName.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                s.AreaName.Contains(f,   StringComparison.OrdinalIgnoreCase)  ||
                (s.DeviceClass?.Contains(f, StringComparison.OrdinalIgnoreCase) ?? false)));
    }
}
