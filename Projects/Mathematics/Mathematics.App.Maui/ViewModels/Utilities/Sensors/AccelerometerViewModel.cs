using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Mathematics.App.Maui.Systems.Application.Constants;
using Mathematics.App.Maui.Systems.Sensors.Entities;
using Mathematics.App.Maui.Systems.Sensors.Processing;
using Mathematics.App.Maui.Systems.Sensors.Services;
using Mathematics.App.Maui.Systems.Utilities;
using Mathematics.App.Maui.UI.Base;
using SkiaSharp;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public sealed partial class AccelerometerViewModel : BaseViewModel
{
    private readonly AccelerometerService _sensor;
    private readonly AccelerometerProcessingService _processing;

    private readonly MinMaxTracker _xStats = new();
    private readonly MinMaxTracker _yStats = new();
    private readonly MinMaxTracker _zStats = new();

    public AccelerometerViewModel(AccelerometerService sensor, AccelerometerProcessingService processing)
    {
        _sensor = sensor;
        _processing = processing;

        XSeries = CreateSeries(SKColors.Red);
        YSeries = CreateSeries(SKColors.Green);
        ZSeries = CreateSeries(SKColors.Blue);

        XAxes = [new Axis { IsVisible = false }];
        YAxes = [new Axis { MinLimit = -2, MaxLimit = 2 }];

        _sensor.Reading += OnSensorReading;
    }

    [ObservableProperty] public partial double X { get; set; }
    [ObservableProperty] public partial double Y { get; set; }
    [ObservableProperty] public partial double Z { get; set; }
    [ObservableProperty] public partial double Magnitude { get; set; }
    [ObservableProperty] public partial bool IsActive { get; set; }

    public Color XColor => GetColorForValue(X);
    public Color YColor => GetColorForValue(Y);
    public Color ZColor => GetColorForValue(Z);

    public double XNormalized => NormalizeValue(X);
    public double YNormalized => NormalizeValue(Y);
    public double ZNormalized => NormalizeValue(Z);

    public string Orientation { get; private set; } = "—";
    public string MagnitudeDescription => DescribeMagnitude(Magnitude);
    
    public string Status => IsActive ? "Активен ●" : "Остановлен ○";
    public Color StatusColor => IsActive ? Colors.Green : Colors.Red;
    public Color ButtonColor => IsActive ? Color.FromArgb("#FF4444") : Color.FromArgb("#44AA44");
    public string ButtonText => IsActive ? "Остановить" : "Запустить";
    
    public string XStats => $"min: {_xStats.Min:F2} max: {_xStats.Max:F2}";
    public string YStats => $"min: {_yStats.Min:F2} max: {_yStats.Max:F2}";
    public string ZStats => $"min: {_zStats.Min:F2} max: {_zStats.Max:F2}";
    
    public ISeries[] XSeries { get; }
    public ISeries[] YSeries { get; }
    public ISeries[] ZSeries { get; }

    public Axis[] XAxes { get; }
    public Axis[] YAxes { get; }
    
    [RelayCommand]
    private void Toggle()
    {
        if (IsActive)
            Stop();
        else
            Start();
    }

    [RelayCommand]
    private void Reset()
    {
        _xStats.Reset(X);
        _yStats.Reset(Y);
        _zStats.Reset(Z);

        ClearHistory();
        RaiseStatsChanged();
    }
    
    private void Start()
    {
        if (!AccelerometerService.IsSupported)
        { 
            Alerts.SensorNotSupported("Accelerometer");
            return;
        }

        _sensor.Start();
        IsActive = true;
        RaiseStatusChanged();
    }

    private void Stop()
    {
        _sensor.Stop();
        IsActive = false;
        RaiseStatusChanged();
    }

    private void OnSensorReading(object? sender, Vector3 raw)
    {
        var processed = _processing.Process(raw);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            X = Math.Round(processed.Linear.X, 3);
            Y = Math.Round(processed.Linear.Y, 3);
            Z = Math.Round(processed.Linear.Z, 3);

            Magnitude = Math.Round(processed.LinearMagnitude, 3);

            _xStats.Push(X);
            _yStats.Push(Y);
            _zStats.Push(Z);

            Orientation = ResolveOrientation(processed.Gravity);

            ChartUtils.Push(XSeries[0], X);
            ChartUtils.Push(YSeries[0], Y);
            ChartUtils.Push(ZSeries[0], Z);

            RaiseDynamicChanged();
        });
    }

    private static ISeries[] CreateSeries(SKColor color) =>
    [
        new LineSeries<double>
        {
            Values = [],
            GeometrySize = 0,
            Stroke = new SolidColorPaint(color, 2),
            Fill = null
        }
    ];

    private static Color GetColorForValue(double value)
    {
        var a = Math.Abs(value);
        return a switch
        {
            < 0.05 => Colors.Gray,
            < 0.3  => Colors.Green,
            < 0.8  => Colors.Orange,
            _      => Colors.Red
        };
    }

    private static double NormalizeValue(double value)
    {
        const double uiMaxG = 1.2;
        return Math.Clamp(Math.Abs(value) / uiMaxG, 0, 1);
    }

    private static string DescribeMagnitude(double value) =>
        value switch
        {
            < 0.05 => "Покой",
            < 0.30 => "Лёгкое движение",
            < 1.00 => "Активное движение",
            < 2.00 => "Сильное движение",
            _      => "Очень сильное движение"
        };

    private static string ResolveOrientation(Vector3 g)
    {
        if (Math.Abs(g.Z) > 0.8) return "Горизонтально";
        if (Math.Abs(g.X) > 0.8) return "Вертикально (боком)";
        if (Math.Abs(g.Y) > 0.8) return "Вертикально (лицом)";
        return "Наклонное";
    }

    private void ClearHistory()
    {
        ((List<double>)XSeries[0].Values!).Clear();
        ((List<double>)YSeries[0].Values!).Clear();
        ((List<double>)ZSeries[0].Values!).Clear();
    }

    private void RaiseDynamicChanged()
    {
        OnPropertyChanged(nameof(XColor));
        OnPropertyChanged(nameof(YColor));
        OnPropertyChanged(nameof(ZColor));
        OnPropertyChanged(nameof(XNormalized));
        OnPropertyChanged(nameof(YNormalized));
        OnPropertyChanged(nameof(ZNormalized));
        OnPropertyChanged(nameof(Orientation));
        OnPropertyChanged(nameof(MagnitudeDescription));
        RaiseStatsChanged();
    }

    private void RaiseStatsChanged()
    {
        OnPropertyChanged(nameof(XStats));
        OnPropertyChanged(nameof(YStats));
        OnPropertyChanged(nameof(ZStats));
    }

    private void RaiseStatusChanged()
    {
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(StatusColor));
        OnPropertyChanged(nameof(ButtonColor));
        OnPropertyChanged(nameof(ButtonText));
    }
}