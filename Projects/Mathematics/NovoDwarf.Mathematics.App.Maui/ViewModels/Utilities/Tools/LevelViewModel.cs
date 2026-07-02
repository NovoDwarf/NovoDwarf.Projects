using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Systems.Hosting.Services;
using NovoDwarf.Mathematics.App.Systems.Sensors.Services;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

public sealed record LevelState(
    double BubbleX,
    double BubbleY,
    double AngleX,
    double AngleY,
    double TotalAngle
);

public sealed class LevelService
{
    private readonly AccelerometerService _accelerometer;

    private double _zeroX;
    private double _zeroY;

    private double _filteredX;
    private double _filteredY;

    private const double MaxOffset = 120;
    private const double Smoothing = 0.15;

    public event EventHandler<LevelState>? StateChanged;

    public LevelService(AccelerometerService accelerometer)
    {
        _accelerometer = accelerometer;
        _accelerometer.Reading += OnReadingChanged;
    }

    public void Start()
    {
        if (!AccelerometerService.IsSupported)
        {
            Shell.Current.DisplayAlertAsync("Уведомление", "Акселерометр не поддерживается данным устройством!", "Ок!");
            return;
        }

        _accelerometer.Start();
    }

    public void Stop() =>
        _accelerometer.Stop();

    public void Calibrate()
    {
        _zeroX = _filteredX;
        _zeroY = _filteredY;
    }

    private void OnReadingChanged(object? sender, Vector3 vector3)
    {
        var gx = Math.Clamp(vector3.X, -1, 1);
        var gy = Math.Clamp(vector3.Y, -1, 1);
        var gz = Math.Clamp(vector3.Z, -1, 1);

        var angleX = Math.Atan2(gx, Math.Sqrt(gy * gy + gz * gz)) * 180 / Math.PI;
        var angleY = Math.Atan2(gy, Math.Sqrt(gx * gx + gz * gz)) * 180 / Math.PI;

        var totalAngle = Math.Sqrt(angleX * angleX + angleY * angleY);

        var targetX = gx * MaxOffset;
        var targetY = -gy * MaxOffset;

        _filteredX += (targetX - _filteredX) * Smoothing;
        _filteredY += (targetY - _filteredY) * Smoothing;

        StateChanged?.Invoke(this, new LevelState(
            BubbleX: _filteredX - _zeroX,
            BubbleY: _filteredY - _zeroY,
            AngleX: angleX,
            AngleY: angleY,
            TotalAngle: totalAngle
        ));
    }
}

public sealed partial class LevelViewModel : BaseViewModel
{
    private readonly LevelService _levelService;

    public LevelViewModel()
    {
        _levelService = InjectionService.Resolve<LevelService>();
        _levelService.StateChanged += OnStateChanged;

        _levelService.Start();
    }

    [ObservableProperty] public partial double BubbleX { get; set; }
    [ObservableProperty] public partial double BubbleY { get; set; }
    [ObservableProperty] public partial double AngleX { get; set; }
    [ObservableProperty] public partial double AngleY { get; set; }
    [ObservableProperty] public partial double TotalAngle { get; set; }

    [RelayCommand]
    private void Calibrate() => _levelService.Calibrate();

    private void OnStateChanged(object? sender, LevelState state)
    {
        BubbleX = state.BubbleX;
        BubbleY = state.BubbleY;
        AngleX = state.AngleX;
        AngleY = state.AngleY;
        TotalAngle = state.TotalAngle;
    }
}