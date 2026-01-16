using Mathematics.App.Maui.UI.ViewModels.Distributions;
using Mathematics.Core.Base.Entities;

namespace Mathematics.App.Maui.UI.Views.Distributions;

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