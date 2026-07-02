using Mathematics.Core.Base.Entities;
using NovoDwarf.Mathematics.App.ViewModels.Probability.Distributions;

namespace NovoDwarf.Mathematics.App.Views.Modules.Probability.Distributions;

public interface IHasDistribution
{
	public Distribution Distribution { get; }

	public event Action<Distribution>? DistributionChanged;
}

public partial class DistributionPage : ContentPage
{
	private IHasDistribution ViewModel => (IHasDistribution)BindingContext;
	private DistributionPlotViewModel? _plotViewModel;

	public DistributionPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (PlotView.BindingContext is not DistributionPlotViewModel plotVm)
			return;

		_plotViewModel = plotVm;
		_plotViewModel.Subscribe(ViewModel);
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		_plotViewModel?.Unsubscribe();
	}
}