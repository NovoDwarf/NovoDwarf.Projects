using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Mathematics.App.Maui.UI.Base;
using Mathematics.App.Maui.UI.Views.Distributions;
using Mathematics.App.Systems.Hosting.Services;
using Mathematics.Core.Base.Entities;
using Microsoft.Extensions.Logging;

namespace Mathematics.App.Maui.UI.ViewModels.Distributions;

public partial class DistributionPlotViewModel : BaseViewModel
{
	private readonly ILogger<DistributionPlotViewModel> _logger;
	private IHasDistribution? _distributionSource;

	public DistributionPlotViewModel()
	{
		_logger = InjectionService.Resolve<ILogger<DistributionPlotViewModel>>();

		PDFSeries = [new LineSeries<ObservablePoint?> { Values = PDFValues, Fill = null }];
		CDFSeries = [new LineSeries<ObservablePoint?> { Values = CDFValues, Fill = null }];
	}

	public Distribution? Distribution { get; private set; }

	public ISeries[] PDFSeries { get; }
	public ISeries[] CDFSeries { get; }

	public ObservableCollection<ObservablePoint?> PDFValues { get; private set; } = [];
	public ObservableCollection<ObservablePoint?> CDFValues { get; private set; } = [];

	[ObservableProperty]
	public partial double Start { get; set; } = 1;

	[ObservableProperty]
	public partial double End { get; set; } = 10;

	[ObservableProperty]
	public partial double Step { get; set; } = 1;

	[RelayCommand]
	private async Task Plot()
	{
		if (Distribution == null)
			throw new ArgumentNullException(nameof(Distribution));

		try
		{
			PDFValues.Clear();
			CDFValues.Clear();

			for (var x = Start; x < End; x += Step)
			{
				var pdf = Distribution.ProbabilityDensity(x);
				var cdf = Distribution.CumulativeDistribution(x);

				PDFValues.Add(pdf is double.NaN ? null : new ObservablePoint(x, pdf));
				CDFValues.Add(cdf is double.NaN ? null : new ObservablePoint(x, cdf));
			}
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlertAsync("Ошибка", ex.Message, "ОК");
			_logger.LogError(ex, "Error distributing value");
		}
	}

	public void Subscribe(IHasDistribution distributionSource)
	{
		if (_distributionSource != null) 
			_distributionSource.DistributionChanged -= OnDistributionChanged;

		_distributionSource = distributionSource;
		_distributionSource.DistributionChanged += OnDistributionChanged;

		Distribution = _distributionSource.Distribution;
	}

	public void Unsubscribe()
	{
		if (_distributionSource == null)
			return;

		_distributionSource.DistributionChanged -= OnDistributionChanged;
		_distributionSource = null;
	}

	private void OnDistributionChanged(Distribution distribution) => Distribution = distribution;
}