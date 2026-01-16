using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mathematics.App.Maui.Systems.Sensors.DTOs;
using Mathematics.App.Maui.Systems.Sensors.Entities;
using Mathematics.App.Maui.Systems.Sensors.Enums;
using Mathematics.App.Maui.Systems.Sensors.Services;
using Mathematics.App.Maui.UI.Base;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public sealed partial class FlashlightViewModel : BaseViewModel
{
    private readonly FlashlightService _service;
    private CancellationTokenSource? _cts;

    public FlashlightViewModel(FlashlightService service)
    {
        _service = service;

        FlashlightModes =
        [
            new FlashlightData { Type = FlashlightModeType.Constant, Name = "Постоянный", Icon = "🔦" },
            new FlashlightData { Type = FlashlightModeType.Strobe,   Name = "Стробоскоп", Icon = "⚡" },
            new FlashlightData { Type = FlashlightModeType.Sos,      Name = "SOS", Icon = "🆘" },
            new FlashlightData { Type = FlashlightModeType.Screen,   Name = "Экранный свет", Icon = "📱" }
        ];

        AvailableColors =
        [
            Colors.White,
            Color.FromHex("#FFE4B5"),
            Colors.Red,
            Colors.Blue,
            Colors.Green
        ];

        Selected = FlashlightModes[0];
        ScreenLightColor = Colors.White;
        ScreenBrightness = 1;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToggleButtonText))]
    [NotifyPropertyChangedFor(nameof(ToggleButtonColor))]
    [NotifyPropertyChangedFor(nameof(IsScreenLightActive))]
    public partial bool IsFlashlightOn { get; private set; }

    [ObservableProperty]
    public partial FlashlightData Selected { get; private set; }

    [ObservableProperty] public partial int StrobeSpeed { get; set; } = 500;
    [ObservableProperty] public partial Color ScreenLightColor { get; set; }
    [ObservableProperty] public partial double ScreenBrightness { get; set; }

    public ObservableCollection<FlashlightData> FlashlightModes { get; }
    public ObservableCollection<Color> AvailableColors { get; }

    public bool IsScreenLightActive => IsFlashlightOn && Selected.Type == FlashlightModeType.Screen;
    public string ToggleButtonText => IsFlashlightOn ? "ВЫКЛ" : "ВКЛ";
    public Color ToggleButtonColor => IsFlashlightOn ? Colors.Green : Colors.Gray;

    [RelayCommand]
    private async Task ToggleAsync()
    {
        if (!await Flashlight.IsSupportedAsync())
        {
            await Shell.Current.DisplayAlertAsync(
                "Уведомление",
                "Фонарик не поддерживается данным устройством",
                "ОК");
            return;
        }

        if (IsFlashlightOn)
            await StopAsync();
        else
            await StartAsync();
    }

    private async Task StartAsync()
    {
        _cts = new CancellationTokenSource();

        await _service.StartAsync(Selected.Type, StrobeSpeed, _cts.Token);

        IsFlashlightOn = true;
    }

    private async Task StopAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        await _service.StopAsync();

        IsFlashlightOn = false;
    }
}