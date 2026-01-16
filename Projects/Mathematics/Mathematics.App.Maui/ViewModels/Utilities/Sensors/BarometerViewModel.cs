using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Mathematics.App.Maui.Systems.Application.Constants;
using Mathematics.App.Maui.Systems.Sensors.Processing;
using Mathematics.App.Maui.Systems.Sensors.Services;
using Mathematics.App.Maui.Systems.Utilities;
using Mathematics.App.Maui.UI.Base;
using SkiaSharp;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public sealed partial class BarometerViewModel : BaseViewModel
{
    private readonly BarometerService _sensor;
    private readonly BarometerProcessingService _processing;

    public BarometerViewModel(BarometerService sensor, BarometerProcessingService processing)
    {
        _sensor = sensor;
        _processing = processing;

        PressureSeries =
        [
            new LineSeries<double>
            {
                Values = [],
                GeometrySize = 0,
                Stroke = new SolidColorPaint(SKColors.DeepSkyBlue, 2),
                Fill = null
            }
        ];

        _sensor.Reading += OnReading;
    }

    public ISeries[] PressureSeries { get; }

    [ObservableProperty] public partial double Pressure { get; set; }
    [ObservableProperty] public partial double Altitude { get; set; }
    [ObservableProperty] public partial string Trend { get; set; } = "—";
    [ObservableProperty] public partial bool IsActive { get; set; }

    [RelayCommand]
    private void Toggle()
    {
        if (IsActive)
            Stop();
        else
            Start();
    }

    private void Start()
    {
        if (!BarometerService.IsSupported)
        {
            Alerts.SensorNotSupported("");
            return;
        }

        _sensor.Start();
        IsActive = true;
    }

    private void Stop()
    {
        _sensor.Stop();
        IsActive = false;
    }

    private void OnReading(object? sender, double raw)
    {
        var data = _processing.Process(raw);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Pressure = Math.Round(data.Pressure, 2);
            Altitude = Math.Round(data.Altitude, 1);
            Trend = data.Trend;

            ChartUtils.Push(PressureSeries[0], data.Pressure);
        });
    }
}

