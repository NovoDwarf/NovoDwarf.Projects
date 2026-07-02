using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Systems.Algorithms.Entities;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

public partial class CashCalculatorViewModel : BaseViewModel
{
	public ObservableCollection<CashItem> Coins { get; }
	public ObservableCollection<CashItem> Bills { get; }

	[ObservableProperty] public partial double CoinsTotal { get; set; }
	[ObservableProperty] public partial double BillsTotal { get; set; }

	public double GrandTotal => CoinsTotal + BillsTotal;

	public CashCalculatorViewModel()
	{
		Coins =
		[
			new CashItem(1),
			new CashItem(2),
			new CashItem(5),
			new CashItem(10),
			new CashItem(25)
		];

		Bills =
		[
			new CashItem(5),
			new CashItem(10),
			new CashItem(50),
			new CashItem(100),
			new CashItem(200),
			new CashItem(500),
			new CashItem(2000),
			new CashItem(5000)
		];

		Subscribe(Coins);
		Subscribe(Bills);
		Recalculate();
	}

	private void Subscribe(IEnumerable<CashItem> items)
	{
		foreach (var item in items)
			item.PropertyChanged += (_, _) => Recalculate();
	}

	private void Recalculate()
	{
		CoinsTotal = Coins.Sum(x => x.Total);
		BillsTotal = Bills.Sum(x => x.Total);

		OnPropertyChanged(nameof(GrandTotal));
	}
}