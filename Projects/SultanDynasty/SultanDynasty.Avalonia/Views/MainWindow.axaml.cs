using Avalonia.Controls;
using SultanDynasty.Avalonia.ViewModels;

namespace SultanDynasty.Avalonia.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}
	
	public MainWindow(MainWindowViewModel viewModel)
	{
		InitializeComponent();
		DataContext = viewModel;
	}
}
