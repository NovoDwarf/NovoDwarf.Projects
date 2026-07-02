using Mathematics.Core.Base.Entities;
using NovoDwarf.Mathematics.App.ViewModels.Probability.Distributions;

namespace NovoDwarf.Mathematics.App.Views.Modules.Probability.Distributions;

public partial class DistributionPlotView : ContentView
{
	public static readonly BindableProperty DistributionProperty =
		BindableProperty.Create(nameof(Distribution), typeof(Distribution), typeof(DistributionPlotView));

	public DistributionPlotView()
	{
		BindingContext = new DistributionPlotViewModel();

		InitializeComponent();
	}

	public Distribution Distribution
	{
		get => (Distribution)GetValue(DistributionProperty);
		set => SetValue(DistributionProperty, value);
	}
}