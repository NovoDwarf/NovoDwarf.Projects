using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mathematics.App.Maui.Services.Views;
using Mathematics.App.Maui.UI.Base;
using Mathematics.App.Maui.UI.Views.Distributions;
using Mathematics.App.Systems.Algorithms.Services;
using Mathematics.App.Systems.Hosting.Services;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Resources;
using Mathematics.Probability.Distributions.Univariate.Discrete.Finite;
using Microsoft.Extensions.Logging;
using ParameterViewModel = Mathematics.App.ViewModels.Common.ParameterViewModel;

namespace Mathematics.App.Maui.UI.ViewModels.Distributions;

public partial class DistributionViewModel : DistributionViewModel<BernoulliDistribution>
{
	public DistributionViewModel(BernoulliDistribution distribution) : base(distribution)
	{

	}
}

public partial class DistributionViewModel<TDistribution> : BaseViewModel, IHasDistribution
	where TDistribution: Distribution
{
	private readonly ILogger<DistributionViewModel> _logger;
	private readonly ParameterService _parameterService;

	public DistributionViewModel(TDistribution distribution)
	{
		_logger = InjectionService.Resolve<ILogger<DistributionViewModel>>();
		_parameterService = InjectionService.Resolve<ParameterService>();
		Distribution = distribution;

		LoadParameters();

		DistributionChanged?.Invoke(Distribution);
	}

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(Name))]
	[NotifyPropertyChangedFor(nameof(Description))]
	[NotifyPropertyChangedFor(nameof(Type))]
	[NotifyPropertyChangedFor(nameof(SubType))]
	[NotifyPropertyChangedFor(nameof(Expected))]
	[NotifyPropertyChangedFor(nameof(Mean))]
	public partial Distribution Distribution { get; private set; }

	public ObservableCollection<ParameterViewModel> Parameters { get; } = [];


	public string Name => Distributions_Resources.ResourceManager.GetString(Distribution.Name) ?? Distribution.Name;
	public string Description => Distributions_Resources.ResourceManager.GetString(Distribution.Description) ?? Distribution.Description;

	public string Type => Distribution.Name;
	public string SubType => Distribution.Name;

	public double Expected => Distribution.Expected;
	public double Mean => Distribution.Mean;
	public double Median => Distribution.Median;
	public double Mode => Distribution.Mode;
	public double Variance => Distribution.Variance;
	public double Skewness => Distribution.Skewness;
	public double Kurtosis => Distribution.Kurtosis;
	public double StandardDeviation => Distribution.StandardDeviation;
	public double Minimum => Distribution.Minimum;
	public double Maximum => Distribution.Maximum;

	public double Value { get; private set; } = 0;

	public event Action<Distribution>? DistributionChanged;

	[RelayCommand]
	private async Task Distribute()
	{
		try
		{
			SetParameters();

			Value = Distribution.Distribute();

			RefreshParameters();
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlertAsync("Ошибка", ex.Message, "ОК");
			_logger.LogError(ex, "Error distributing value");
		}
	}

	private void SetParameters()
	{
		var paramArray = new object[Parameters.Count];

		for (var index = 0; index < Parameters.Count; index++)
		{
			var value = Parameters[index].Value;

			if (value != null)
				paramArray[index] = value;
		}

		Distribution.Set(paramArray);
		DistributionChanged?.Invoke(Distribution);
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
		var parameters = _parameterService.GetParameters(Distribution.GetType());

		foreach (var param in parameters)
		{
			Parameters.Add(param);
		}

		RefreshParameters();
	}
}