using CommunityToolkit.Mvvm.ComponentModel;

namespace Mathematics.App.Maui.Models.Entities;

public partial class CashItem(double denomination) : ObservableObject
{
	public double Denomination { get; } = denomination;

	[ObservableProperty]
	public partial int Count { get; set; }

	public double Total => Denomination * Count;
}