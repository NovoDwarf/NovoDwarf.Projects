using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using Mathematics.App.Maui.Base;
using Mathematics.App.Maui.Services;
using Mathematics.Core.Base.Entities;
using Microsoft.Extensions.Logging;

namespace Mathematics.App.Maui.UI.ViewModels;

public partial class DistributionViewModel : BaseViewModel 
{
	private readonly ILogger<DistributionViewModel> _logger;
	private readonly Distribution _distribution;
	
	public DistributionViewModel(Distribution distribution, ILogger<DistributionViewModel> logger)
	{
		_logger = logger;
		_distribution = distribution;
		
		PDFSeries = [new LineSeries<ObservablePoint?> { Values = PDFValues, Fill = null }];
		CDFSeries = [new LineSeries<ObservablePoint?> { Values = CDFValues, Fill = null }];

		LoadParameters();
	}
	
	public ObservableCollection<ParameterViewModel> Parameters { get; } = [];
	
	public string Name => _distribution.Name;
	public string Description => _distribution.Description;
	
	public string Type => _distribution.Name;
	public string SubType => _distribution.Name;
	
	public double Expected => _distribution.Expected;
	public double Mean => _distribution.Mean;
	public double Median => _distribution.Median;
	public double Mode => _distribution.Mode;
	public double Variance => _distribution.Variance;
	public double Skewness => _distribution.Skewness;
	public double Kurtosis => _distribution.Kurtosis;
	public double StandardDeviation => _distribution.StandardDeviation;
	public double Minimum => _distribution.Minimum;
	public double Maximum => _distribution.Maximum;

	public double Value { get; private set; } = 0;
	
	public ISeries[] PDFSeries { get; private set; }
	public ISeries[] CDFSeries { get; private set; }

	public ObservableCollection<ObservablePoint?> PDFValues { get; private set; } = [];
	public ObservableCollection<ObservablePoint?> CDFValues { get; private set; } = [];

	public double Start { get; set; } = 1;
	public double End { get; set; } = 10;
	public double Step { get; set; } = 1;
	
	public void SetParameters()
	{
		var paramArray = new object[Parameters.Count];

		for (var index = 0; index < Parameters.Count; index++)
		{
			paramArray[index] = Parameters[index].Value;
		}
		
		_distribution.Set(paramArray);
	}

	private void RefreshParameters()
	{
		OnPropertyChanged(nameof(Expected));
		OnPropertyChanged(nameof(Mean));
		OnPropertyChanged(nameof(Median));
		OnPropertyChanged(nameof(Mode));
		OnPropertyChanged(nameof(Variance));
		OnPropertyChanged(nameof(Skewness));
		OnPropertyChanged(nameof(Kurtosis));
		OnPropertyChanged(nameof(StandardDeviation));
		OnPropertyChanged(nameof(Minimum));
		OnPropertyChanged(nameof(Maximum));
	}
	
	private void LoadParameters()
	{
		var parameters = ParameterService.GetParameters(_distribution.GetType());
		
		foreach (var param in parameters)
		{
			Parameters.Add(param);
		}
	}

	[RelayCommand]
	private async Task Plot()
	{
		try
		{
			SetParameters();
			
			PDFValues.Clear();
			CDFValues.Clear();
			
			for (var i = Start; i < End; i += Step)
			{
				var pdf = _distribution.ProbabilityDensity(i);
				var cdf = _distribution.CumulativeDistribution(i);
				
				PDFValues.Add(double.IsNaN(pdf) ? null : new ObservablePoint(i, pdf));
				CDFValues.Add(double.IsNaN(cdf) ? null : new ObservablePoint(i, cdf));
			}
			
			RefreshParameters();
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlertAsync("Ошибка", ex.Message, "ОК");
			_logger.LogError(ex, "Error distributing value");
		}
	}

	[RelayCommand]
	private async Task Distribute()
	{
		try
		{
			SetParameters();

			Value = _distribution.Distribute();
			
			RefreshParameters();
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlertAsync("Ошибка", ex.Message, "ОК");
			_logger.LogError(ex, "Error distributing value");
		}
	}
}