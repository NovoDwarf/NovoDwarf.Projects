using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using NovoDwarf.Mathematics.App.Systems.Algorithms.Services;
using NovoDwarf.Mathematics.App.Systems.Utilities;
using Plugin.Maui.Audio;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

public partial class SoundGeneratorViewModel : ObservableObject
{
	private readonly IAudioManager _audioManager;

	public SoundGeneratorViewModel(IAudioManager audioManager)
	{
		_audioManager = audioManager;

		Series = new LineSeries<ObservablePoint> { Values = SignalValues, Fill = null };
	}

	[ObservableProperty]
    public partial ObservableCollection<WaveType> WaveTypes { get; set; } = [WaveType.Sine, WaveType.Sawtooth, WaveType.Square, WaveType.Triangle];

    [ObservableProperty]
    public partial WaveType SelectedWaveType { get; set; } = WaveType.Sine;

    [ObservableProperty]
    public partial double Frequency { get; set; } = 440;

    [ObservableProperty]
    public partial double Duration { get; set; } = 5;

    [ObservableProperty]
    public partial double Amplitude { get; set; } = 1;

    [ObservableProperty]
    public partial double[] Separators { get; set; } = [];

    public ObservableCollection<ObservablePoint> SignalValues { get; set; } = [];

    public ISeries Series { get; set; }

    [RelayCommand]
    private async Task Play()
    {
	    var wave = SoundGeneratorService.GenerateWave(Frequency, Duration, SelectedWaveType, Amplitude);

	    SignalValues.Clear();

	    for (var i = 0; i < wave.Length; i += 2)
	    {
		    var sample = BitConverter.ToInt16(wave, i);
		    SignalValues.Add(new ObservablePoint
		    {
			    X = i / 2.0,
			    Y = sample / 32768.0
		    });
	    }

	    var player = _audioManager.CreatePlayer();
	    await using var stream = WavUtils.ConvertPcmToWav(wave);
	    player.SetSource(stream);
	    player.Play();
    }

    [RelayCommand]
    private void Stop()
    {

    }
}

